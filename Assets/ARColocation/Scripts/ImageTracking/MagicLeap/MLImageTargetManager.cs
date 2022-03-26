using System;
using System.Collections;
using System.Collections.Generic;
using Colocation.Utilities;
using UnityEngine;
using UnityEngine.Events;
#if PLATFORM_LUMIN
using UnityEngine.XR.MagicLeap;
#endif
namespace Colocation
{


    //This creates a class that exposes image target variables to the inspector
    [System.Serializable]
    public class ImageTargetInfo
    {
        [Tooltip("(Optional) unique name of this target")]
        public string Id;

        [Tooltip("Texture2D representing the Image Target. The aspect ration of the target should not be changed. Set the \"Non Power of 2\" property of Texture2D to none.")]
        public Texture2D Image;

        [Tooltip("Set this to true if the position of this Image Target in the physical world is fixed and the local geometry is planar.")]
        public bool isStationary;

        [Tooltip("Size of the longer dimension in scene units.")]
        public float LongerDimension;
    }

//This contains the four possible statuses we can encounter while trying to use the tracker.
public enum ImageTrackingStatus
{
    Inactive,
    PrivilegeDenied,
    ImageTrackingActive,
    CameraUnavailable
}



//The main class containing our image tracking functions
public class MLImageTargetManager : MonoBehaviour
{
    #if PLATFORM_LUMIN
    private static MLImageTargetManager _instance;
    public static MLImageTargetManager Instance {
        get
        {
            if (_instance == null)
            {
                var magicLeapImageTackerGameObject = new GameObject("Magic Leap Image Tracker (instance)");
                _instance = magicLeapImageTackerGameObject.AddComponent<MLImageTargetManager>();
             
            }

            return _instance;
        }
    }
    /// <summary>
    ///stored names of created targets so that we can remove them on command
    /// </summary>
    private List<string> _targetNames = new List<string>();

    /// <summary>
    /// stored visuals we can destroy when tracking is turned off
    /// </summary>
    private List<MLImageTargetBehaviour> _targetBehaviours = new List<MLImageTargetBehaviour>();

    /// <summary>
    /// stored textures that had to be created because read/write was not enabled. This is used for deletion
    /// </summary>
    private List<Texture2D> _tempReadWriteTextures = new List<Texture2D>();

    /// <summary>
    /// temp textures by target name
    /// </summary>
    private Dictionary<string, Texture2D> _tempReadWriteTexturesByName = new Dictionary<string, Texture2D>();
    
    //collection to find visual by name
    private Dictionary<string, MLImageTargetBehaviour> _targetBehavioursByName = new Dictionary<string, MLImageTargetBehaviour>();
   
    [SerializeField] [Tooltip("targets to search for")]
    private List<ImageTargetInfo> _targets;

    [Tooltip("The prefab to spawn - image target visual")]
    [SerializeField] private MLImageTargetBehaviour _MLImageTargetBehaviourPrefab;

    /// <summary>
    /// Invoked when the status of the image tracking service has changed
    /// </summary>
    public  Action<ImageTrackingStatus> ImageTrackingStatusChanged;
    
    public ImageTrackingStatus CurrentStatus = ImageTrackingStatus.Inactive;


    [Tooltip("Should image tracking be enabled at the start of the application?")]
    [SerializeField] private bool _autoStart;
   
    private bool _isImageTrackingInitialized;
    private Coroutine _getPrivilegesCoroutine;
  

    private void OnValidate()
        {
            if(_targets!= null)
            {
                foreach (var imageTargetInfo in _targets)
                {
                    if (imageTargetInfo != null && imageTargetInfo.Image != null && string.IsNullOrWhiteSpace(imageTargetInfo.Id))
                    {
                        imageTargetInfo.Id = imageTargetInfo.Image.name;
                    }
                }
            }
        }

    private void Awake()
    {
        _instance = this;
        UpdateImageTrackingStatus(ImageTrackingStatus.Inactive);
    }

    public MLImageTargetBehaviour GetImageTargetByTexture(Texture2D targetTexture)
    {
        if (_targetBehavioursByName.TryGetValue(targetTexture.name, out var targetBehaviour))
        {
            return targetBehaviour;
        }

        var targetInfo = _targets.Find((e) => e.Image == targetTexture);
        if (_targetBehavioursByName.TryGetValue(targetInfo.Id, out targetBehaviour))
        {
            return targetBehaviour;
        }

        Debug.LogError($"Could not find target by texture. Looked for texture [{targetTexture.name}] - {targetTexture}");
        return null;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
         
            PauseImageTracking();
        }
        else
        {
           ResumeImageTracking();
        }
    }

    private void OnDestroy()
    {
        _getPrivilegesCoroutine = null;
        StopImageTracking();
        foreach (var tempReadWriteTexture in _tempReadWriteTextures)
        {
            Destroy(tempReadWriteTexture);
        }
    }


    private void Start()
    {
        if (_autoStart)
        {
            ActivatePrivileges(); 
        }
    }

  
    private void ActivatePrivileges()
    {
    
        // If privilege was not already denied by User:
        if (CurrentStatus != ImageTrackingStatus.PrivilegeDenied && _getPrivilegesCoroutine == null)
        {
            // Try to get the component to request privileges
            _getPrivilegesCoroutine = StartCoroutine(RequestPrivilege());

        }
    }

    IEnumerator RequestPrivilege()
    {
        bool privilegesGranted = false;
        bool hasPrivilegesResult = false;
        MLPrivileges.RequestPrivilegesAsync(MLPrivileges.Id.CameraCapture)
                 .ContinueWith((x) =>
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

        if (privilegesGranted)
        {
            StartImageTracking();
        }
        else
        {
            UpdateImageTrackingStatus(ImageTrackingStatus.PrivilegeDenied);
        }

        _isImageTrackingInitialized = true;
        _getPrivilegesCoroutine = null;
    }





    private void UpdateImageTrackingStatus(ImageTrackingStatus status)
    {
        CurrentStatus = status;
        ImageTrackingStatusChanged?.Invoke(CurrentStatus);
   
    }

    private void RemoveAllTargets()
    {
        foreach (var target in _targetNames)
        {
            Destroy(_targetBehavioursByName[target].gameObject);
        }
    }

    public void TrackAllTargets()
    {
        foreach (var imageTargetInfo in _targets)
        {
            if(_targetNames.Contains(imageTargetInfo.Id))
            {
                continue;
            }

            var targetBehaviour = Instantiate(_MLImageTargetBehaviourPrefab);
      
            targetBehaviour.Initialize(imageTargetInfo.Id);
            _targetNames.Add(imageTargetInfo.Id);
            _targetBehaviours.Add(targetBehaviour);
            _targetBehavioursByName.Add(imageTargetInfo.Id,targetBehaviour);
            targetBehaviour.Destroyed += OnTargetDestroyed;
            Texture2D image = imageTargetInfo.Image;
            if (!image.isReadable)
            {
                if (_tempReadWriteTexturesByName.TryGetValue(imageTargetInfo.Id, out var cachedTexture))
                {
                    image = cachedTexture;
                }
                else
                {
                    image = ImageUtilities.DuplicateTexture(imageTargetInfo.Image);
                    _tempReadWriteTextures.Add(image);
                    _tempReadWriteTexturesByName.Add(imageTargetInfo.Id, image);
                }

            }
            // Add the target image to the tracker and set the callback
             MLImageTracker.AddTarget(imageTargetInfo.Id, image, imageTargetInfo.LongerDimension, HandleImageTracked, imageTargetInfo.isStationary);



        }

        UpdateImageTrackingStatus(ImageTrackingStatus.ImageTrackingActive);
    }
    
    void HandleImageTracked(MLImageTracker.Target imageTarget,  MLImageTracker.Target.Result imageTargetResult)
    {
        if (_targetBehavioursByName.TryGetValue(imageTarget.TargetSettings.Name, out var targetBehaviour))
        {
            var state = (MLImageTargetBehaviour.ImageTrackingState)imageTargetResult.Status;
            targetBehaviour.UpdateTracking(state, imageTargetResult.Position, imageTargetResult.Rotation);
        }

    }
    
    

    private void OnTargetDestroyed(string targetName)
    {
        var targetIndex = _targetNames.IndexOf(targetName);
        if (targetIndex >= 0)
        {
            _targetNames.RemoveAt(targetIndex);
            _targetBehaviours.RemoveAt(targetIndex);
            _targetBehavioursByName.Remove(targetName);
            MLImageTracker.RemoveTarget(targetName);
        }
        if (MLImageTracker.IsStarted && _targetNames.Count == 0)
        {
            MLImageTracker.Stop();
            UpdateImageTrackingStatus(ImageTrackingStatus.Inactive);
        }
       

    }

    public void StartImageTracking()
    {
        if (!_isImageTrackingInitialized && _getPrivilegesCoroutine == null)
        {
            _getPrivilegesCoroutine = StartCoroutine(RequestPrivilege());
        }
        // Only start Image Tracking if privilege wasn't denied
        if (CurrentStatus != ImageTrackingStatus.PrivilegeDenied)
        {
            // Is not already started, and failed to start correctly, this is likely due to the camera already being in use:
            if (!MLImageTracker.IsStarted && !MLImageTracker.Start().IsOk)
            {
                Debug.LogError("Image Tracker Could Not Start");
                RemoveAllTargets();
                UpdateImageTrackingStatus(ImageTrackingStatus.CameraUnavailable);
                return;
            }

            // MLImageTracker would have been started by previous If statement at this point, so enable it. 
            if (MLImageTracker.Enable().IsOk)
            {
                TrackAllTargets();
            }
            else
            {
                Debug.LogError("Image Tracker Could Not Start");
                UpdateImageTrackingStatus(ImageTrackingStatus.CameraUnavailable);
                return;
            }
        }
    }

    public void ResumeImageTracking()
    {
        if (_isImageTrackingInitialized)
        {
            MLImageTracker.Enable();
        }
    }
    public void PauseImageTracking()
    {
        if (MLImageTracker.IsStarted)
        {
            MLImageTracker.Disable();
        }
    }
    public void StopImageTracking()
    {
        RemoveAllTargets();
    }



#endif
}
}
