using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
#if PLATFORM_LUMIN
using UnityEngine.XR.MagicLeap;
#endif
[System.Serializable]
public class ImageTargetInfo
{
    public string Name;
    public Texture2D Image;
    [Tooltip("The size of the long edge of the image target in meters.")]
    public float LongerDimension;
}


/// <summary>
/// Transforms Magic Leap's PCFs into generic Coordinates that can be used for co-location experiences.
/// </summary>
public class MLGenericCoordinateProvider : MonoBehaviour, IGenericCoordinateProvider
{
#if !PLATFORM_LUMIN
#pragma warning disable 414
#endif
    //Image Tracking

#if PLATFORM_LUMIN
    // The image target built from the ImageTargetInfo object
    private MLImageTracker.Target _imageTarget;
#endif

    //The inspector field where we assign our target images
    public ImageTargetInfo TargetInfo;
    private Coroutine _searchForImageCoroutine;

#if PLATFORM_LUMIN
    private MLImageTracker.Target.Result _imageTargetResult;
#endif

    [Tooltip("How long to search for an image, in seconds")]
    public float ImageTargetSearchTime = 60;



    [Tooltip("If true, image tracking will start any time a user requests an anchor. Note: This increases the time it takes to localize when no image target is present.")]
    public bool _autoSearchForImage = true;

    //These allow us to see the position and rotation of the detected image from the inspector
    private Vector3 _imagePos = Vector3.zero;
    private Quaternion _imageRot = Quaternion.identity;

    [SerializeField]
    [Tooltip("Displays over the image target.")]
    private GameObject _imageTargetVisual;
    
    private bool _isImageTrackingInitialized = false;

    private bool _imagePrefabUpdated = false;

    //Only supports one image at the moment.
    private List<GenericCoordinateReference> _genericImageCoordinates = new List<GenericCoordinateReference>();

    private Coroutine _getGenericCoordinatesEnumerator;

    private TaskCompletionSource<List<GenericCoordinateReference>> _coordinateReferencesCompletionSource;

    private const int RequestTimeoutMs = 20000;

#if !PLATFORM_LUMIN
#pragma warning restore 414
#endif

    void Awake()
    {
        if (_imageTargetVisual != null)
        {
            _imageTargetVisual.SetActive(false);
        }
    }

    public async Task<List<GenericCoordinateReference>> RequestCoordinateReferences(bool refresh)
    {
        try {
            CancelTasks();

            _coordinateReferencesCompletionSource = new TaskCompletionSource<List<GenericCoordinateReference>>();
            _getGenericCoordinatesEnumerator = StartCoroutine(DoGetGenericCoordinates());


            if (await Task.WhenAny(_coordinateReferencesCompletionSource.Task,
                                   Task.Delay(RequestTimeoutMs * 100)) != _coordinateReferencesCompletionSource.Task)
            {
                Debug.LogError("Could not get coordinates");
                return new List<GenericCoordinateReference>();
            }

            return _coordinateReferencesCompletionSource.Task.Result;
        }
        catch (Exception e)
        {
            Debug.LogException(e);

        }

        return null;
    }

    private void CancelTasks()
    {
        if (_getGenericCoordinatesEnumerator != null)
        {
            StopCoroutine(_getGenericCoordinatesEnumerator);
            _getGenericCoordinatesEnumerator = null;
        }
        _coordinateReferencesCompletionSource?.TrySetResult(null);

    }

    public void SearchForImage()
    {
        if (_searchForImageCoroutine == null)
        {
            _searchForImageCoroutine = StartCoroutine(DoSearchForImage());
        }
    }

    public void InitializeGenericCoordinates()
    {

    }

    public void DisableGenericCoordinates()
    {
        CancelTasks();

    }

    private IEnumerator DoGetGenericCoordinates()
    {
#if PLATFORM_LUMIN
        //system start up


        List<GenericCoordinateReference> genericCoordinateReferences = new List<GenericCoordinateReference>();



        if (_autoSearchForImage)
            yield return DoSearchForImage();

        genericCoordinateReferences.AddRange(_genericImageCoordinates);

        Debug.Log("Returning "+ genericCoordinateReferences.Count + " genericPcfReferences.");

        _coordinateReferencesCompletionSource?.TrySetResult(genericCoordinateReferences);
#else
        yield return new WaitForEndOfFrame();

        _coordinateReferencesCompletionSource?.TrySetResult(null);

#endif
        _getGenericCoordinatesEnumerator = null;

    }




    private IEnumerator DoSearchForImage()
    {
#if PLATFORM_LUMIN
        Debug.Log("Initializing Image Scan");

#pragma warning disable 618
        if (!MLImageTracker.IsStarted)
        {
            MLImageTracker.Start();
        }
#pragma warning restore 618
        yield return new WaitForEndOfFrame();
        MLImageTracker.Enable();

        if (string.IsNullOrEmpty(TargetInfo.Name) == false)
        {
            yield return new WaitForEndOfFrame();

            bool privilegesGranted = false;
            bool hasPrivilegesResult = false;


            MLPrivileges.RequestPrivilegesAsync(MLPrivileges.Id.CameraCapture).ContinueWith((x) =>
            {
                if (x.Result.IsOk == false && x.Result != MLResult.Code.PrivilegeGranted)
                {
                    privilegesGranted = false;
                    Debug.LogError("Image capture privileges not granted. Reason: " + x.Result);
                }
                else
                {
                    privilegesGranted = true;
                }

                hasPrivilegesResult = true;
            });

            while (hasPrivilegesResult == false)
            {
                yield return null;
            }

            if (privilegesGranted == true && !_isImageTrackingInitialized)
            {
                _imageTarget = MLImageTracker.AddTarget(TargetInfo.Name, TargetInfo.Image, TargetInfo.LongerDimension, HandleImageTracked, false);
            }

            if (_imageTarget == null)
            {
                Debug.LogError("Cannot add image target");
            }
            else
            {
                Debug.Log("Image Target Added");
                _isImageTrackingInitialized = true;
            }

        }

        yield return new WaitForEndOfFrame();

        if (_isImageTrackingInitialized)
        {
            Debug.Log("Searching for image target");
            float imageRequestedTime = Time.time;
            while (Time.time - imageRequestedTime < ImageTargetSearchTime &&
                   (_imageTargetResult.Status != MLImageTracker.Target.TrackingStatus.Tracked
                    || _imagePos.x < .01f && _imagePos.x > -0.01f))
            {
                yield return null;
            }

            if (_imageTargetResult.Status == MLImageTracker.Target.TrackingStatus.Tracked)
            {
                //We only support one image target so remove any existing ones.
                _genericImageCoordinates.Clear();

                //Wait for tracker to update position before removing target and associated update function.
                while (!_imagePrefabUpdated)
                {
                    yield return null;
                }

                Debug.Log("Image target found, adding as generic coordinate reference.");
                var imageCoordinate = new GenericCoordinateReference()
                {
                    CoordinateId = TargetInfo.Name,
                    Position = _imagePos,
                    Rotation = _imageRot
                };

                _genericImageCoordinates.Add(imageCoordinate);
            }
        }

        MLImageTracker.Disable();
        MLImageTracker.RemoveTarget(TargetInfo.Name);
        yield return new WaitForEndOfFrame();
#pragma warning disable 618
        MLImageTracker.Stop();
#pragma warning restore 618

        _searchForImageCoroutine = null;
        _isImageTrackingInitialized = false;
        _imagePrefabUpdated = false;

#else
        yield return null;
#endif
    }



    private void OnDisable()
    {
        CancelTasks();
    }

    private void OnDestroy()
    {
        CancelTasks();
    }

    public GenericCoordinateReference GetImageCoordinateReference()
    {
        if (_genericImageCoordinates.Count > 1)
        {
            return _genericImageCoordinates[1];
        }

        return null;
    }

#if PLATFORM_LUMIN

    private void HandleImageTracked(MLImageTracker.Target imageTarget,
                                    MLImageTracker.Target.Result imageTargetResult)
    {
        if (imageTargetResult.Status == MLImageTracker.Target.TrackingStatus.Tracked)
        {
            _imageTargetResult = imageTargetResult;
            _imagePos = imageTargetResult.Position;
            _imageRot = imageTargetResult.Rotation;
            if (_imageTargetVisual != null)
            {
                _imageTargetVisual.transform.position = imageTargetResult.Position;
                _imageTargetVisual.transform.rotation = imageTargetResult.Rotation;
                _imageTargetVisual.SetActive(true);
                _imagePrefabUpdated = true;
            }
          
        }
    }

#endif
}
