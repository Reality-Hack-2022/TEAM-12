using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TrackedImage : MonoBehaviour
{

    private List<ARTrackedImage> _arTrackedImages = new List<ARTrackedImage>();
    private ARTrackedImageManager ARTrackedImageManager
    {
        get
        {
            if (_arTrackedImageManager == null)
            {
                _arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();
            }

            return _arTrackedImageManager;
        }
    }



    private void TrackImage(bool value)
    {
        this.enabled = value;
    }
    [SerializeField] private ARTrackedImageManager _arTrackedImageManager;

    // Start is called before the first frame update
    void OnEnable()
    {
        if (ARTrackedImageManager)
            ARTrackedImageManager.trackedImagesChanged += ARTrackedImageManager_trackedImagesChanged;
    }

    // Start is called before the first frame update
    void OnDisable()
    {
        if(ARTrackedImageManager)
            ARTrackedImageManager.trackedImagesChanged -= ARTrackedImageManager_trackedImagesChanged;
    }

    private void ARTrackedImageManager_trackedImagesChanged(ARTrackedImagesChangedEventArgs obj)
    {

        _arTrackedImages.Clear();

        for (int i = 0; i < obj.added.Count; i++)
        {
            _arTrackedImages.Add(obj.added[i]);
        }

        for (int i = 0; i < obj.updated.Count; i++)
        {
            if (!_arTrackedImages.Contains(obj.updated[i]))
            {
                _arTrackedImages.Add(obj.updated[i]);
            }
        }

        for (int i = 0; i < _arTrackedImages.Count; i++)
        {
            var image = _arTrackedImages[i];
            if (image.trackingState == TrackingState.Tracking)
            {
                transform.position = image.transform.position;
                transform.rotation = image.transform.rotation;
                break;
            }
        }

    }
}
