using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageTrackedOrigin : MonoBehaviour
{

    ARTrackedImageManager m_TrackedImageManager;

    void Awake()
    {
        m_TrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var trackedImage in eventArgs.added)
        {
            transform.position = trackedImage.transform.position;
            transform.rotation = trackedImage.transform.rotation;
        }

        foreach (var trackedImage in eventArgs.updated)
        {
            transform.position = trackedImage.transform.position;
            transform.rotation = trackedImage.transform.rotation;
        }
    }
}
