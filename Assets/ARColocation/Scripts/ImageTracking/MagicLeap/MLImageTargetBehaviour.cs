using System;
using UnityEngine;

namespace Colocation
{
	public class MLImageTargetBehaviour: MonoBehaviour
	{
		public enum ImageTrackingState
		{
			Tracked,
			Unreliable,
			NotTracked,
		}
		public string TargetId;
		public Action TrackingUpdated;
		public Action<string> Destroyed;
		public ImageTrackingState CurrentTrackingState = ImageTrackingState.NotTracked;
		public bool Initialized { get; private set; }
		public void Initialize(string targetName)
		{
			TargetId = targetName;
			gameObject.name = $"Image Target - "+targetName;
			Initialized = true;
		}

		public void UpdateTracking(ImageTrackingState trackingState, Vector3 position, Quaternion rotation)
		{
			CurrentTrackingState = trackingState;
			if (trackingState == ImageTrackingState.Tracked)
			{
				transform.position = position;
				transform.rotation = rotation;
			}

			Initialized = true;
			TrackingUpdated?.Invoke();
		}

		private void OnDestroy()
		{
			Destroyed?.Invoke(TargetId);
		}
	}
}
