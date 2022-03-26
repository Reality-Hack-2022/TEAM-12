using MagicLeap.Core;
using UnityEngine;
#if PLATFORM_LUMIN
using System.Collections;
using UnityEngine.XR.MagicLeap;
#endif
namespace Colocation
{
	public class MLColocationImageTarget : MonoBehaviour
	{
		[SerializeField] private Texture2D _targetImage;
		
		private MLImageTargetBehaviour _targetBehaviour;
		private bool _inputReady = false;
#if PLATFORM_LUMIN

		// Start is called before the first frame update
		void Start()
		{
			
			MLInput.OnControllerButtonDown += MLInputOnOnControllerButtonDown;
			MLImageTargetManager.Instance.ImageTrackingStatusChanged += ImageTrackingStatusChanged;
		}

		private void ImageTrackingStatusChanged(ImageTrackingStatus imageTrackingStatus)
		{
			if(_targetBehaviour != null)
			{
				return;
			}

			if (imageTrackingStatus == ImageTrackingStatus.ImageTrackingActive && _inputReady)
			{
				_targetBehaviour = MLImageTargetManager.Instance.GetImageTargetByTexture(_targetImage);
				if (_targetBehaviour != null)
				{
					_targetBehaviour.TrackingUpdated+= TrackingUpdated;
				}
			}
		}

		private void TrackingUpdated()
		{
			transform.position = _targetBehaviour.transform.position;
			transform.rotation = _targetBehaviour.transform.rotation;
		}


		private void MLInputOnOnControllerButtonDown(byte controllerid, MLInput.Controller.Button button)
		{
			if (button == MLInput.Controller.Button.Bumper)
			{
				if (_inputReady && _targetBehaviour != null)
				{
					MLImageTargetManager.Instance.StopImageTracking();
					_inputReady = false;
				}
				else
				{
					MLImageTargetManager.Instance.StartImageTracking();

					_inputReady = true;
				}
			}
		}



#endif

	}
}

