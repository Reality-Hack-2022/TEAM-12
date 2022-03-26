using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
#if PLATFORM_LUMIN
using UnityEngine.XR.MagicLeap;
#endif
[RequireComponent(typeof(PhotonView))]
    public class ControllerFollow : MonoBehaviour
    {
        
        private PhotonView _networkView;

        #region Unity Methods
        /// <summary>
        /// Initialize variables, callbacks and check null references.
        /// </summary>
        void OnEnable()
        {
            _networkView = GetComponent<PhotonView>();

        }
        


        /// <summary>
        /// Update controller input based feedback.
        /// </summary>
        void Update()
        {
#if PLATFORM_LUMIN
            if (!_networkView.IsMine || !PhotonNetwork.IsConnected || !MLInput.IsStarted)
                return;

            MLInput.Controller controller = MLInput.GetController(0);
            if( (controller != null) && controller.Connected)
            {
                if (controller.Type == MLInput.Controller.ControlType.Control)
                {
                    // For Control, raw input is enough
                    transform.position = controller.Position;
                    transform.rotation = controller.Orientation;
                }
            }
#endif
        }
        
        #endregion


    }
