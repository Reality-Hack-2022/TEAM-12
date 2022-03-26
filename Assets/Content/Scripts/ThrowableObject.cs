using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class ThrowableObject : MonoBehaviour
{
    [SerializeField]
    private ObjectManipulator m_objectManipulator = null;

    [SerializeField, Range(0, 10)]
    private float m_speed = 1;

    private Vector3? Direction { get; set; } = null;
    private Coroutine ManipulationCoroutine { get; set; } = null;

    private void Awake()
    {
        Assert.IsNotNull(m_objectManipulator);

        m_objectManipulator.OnManipulationStarted.AddListener(OnManipulationStartedHandler);
        m_objectManipulator.OnManipulationEnded.AddListener(OnManipulationEndedHandler);
    }

    private void OnManipulationStartedHandler(ManipulationEventData _data)
    {
        Assert.IsNull(ManipulationCoroutine);
        ManipulationCoroutine = StartCoroutine(ManipulationAsync(_data));
    }

    private void OnManipulationEndedHandler(ManipulationEventData _data)
    {
        Assert.IsNotNull(ManipulationCoroutine);
        StopCoroutine(ManipulationCoroutine);
        ManipulationCoroutine = null;
    }

    private void Update()
    {
        if (ManipulationCoroutine != null)
        {
            return;
        }
        if (!Direction.HasValue)
        {
            return;
        }

        transform.position += Direction.Value * m_speed * Time.deltaTime;
    }

    private IEnumerator ManipulationAsync(ManipulationEventData _data)
    {
        while (true)
        {
            Direction = (_data.Pointer.Rotation * Vector3.forward).normalized;
            yield return null;
        }
    }
}
