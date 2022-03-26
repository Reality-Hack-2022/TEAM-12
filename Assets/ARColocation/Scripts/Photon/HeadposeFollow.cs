using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class HeadposeFollow : MonoBehaviour
{
    [SerializeField]
    private PhotonView _photonView;

    private Transform _mainCameraTransform;

    private void Reset()
    {
        _photonView = GetComponent<PhotonView>();
    }

    private void Awake()
    {

        if (_photonView == null)
        {
            _photonView = GetComponent<PhotonView>();
        }
    }

    // // Start is called before the first frame update
    void Start()
    {

        if (Camera.main != null)
        {
            _mainCameraTransform = Camera.main.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_mainCameraTransform == null)
        {
            if (Camera.main != null)
            {
                _mainCameraTransform = Camera.main.transform;
            }

            return;
        }

        if (_photonView.IsMine)
        {
            transform.position = _mainCameraTransform.position;
            transform.rotation = _mainCameraTransform.rotation;
        }
    }
}
