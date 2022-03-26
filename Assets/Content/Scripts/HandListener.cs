using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.Assertions;

public class HandListener : MonoBehaviour
{
    [SerializeField]
    private Handedness m_handedness = Handedness.None;

    [SerializeField]
    private ParticleSystem m_particleSystem = null;
    [SerializeField]
    private ParticleSystem m_heartParticleSystem = null;

    [SerializeField]
    private Animator m_animator = null;

    public bool IsActive { get; set; } = false;

    private void Awake()
    {
        Assert.IsTrue(m_handedness == Handedness.Left || m_handedness == Handedness.Right);
        Assert.IsNotNull(m_particleSystem);
        Assert.IsNotNull(m_heartParticleSystem);
        Assert.IsNotNull(m_animator);

        SetEmissionRate(0);
    }

    private void Update()
    {
        if (HandJointUtils.TryGetJointPose(TrackedHandJoint.Palm, m_handedness, out MixedRealityPose pose))
        {
            transform.position = pose.Position;
            Activate(true);
        }
        else
        {
            Activate(false);
        }
    }

    private void Activate(bool _value)
    {
        IsActive = _value;
        m_animator.SetBool("Active", _value);
        SetEmissionRate(_value ? 10 : 0);
    }

    public void UpdateParticleSystemColor(Color _color)
    {
        var main = m_particleSystem.main;
        main.startColor = _color;
    }

    private void SetEmissionRate(float _value)
    {
        var emission = m_particleSystem.emission;
        emission.rateOverTime = _value;
    }

    public void UpdateHeartBeatTransparency(Color _color, float _value)
    {
        var main = m_heartParticleSystem.main;
        Color color = _color;
        _value = Mathf.Clamp(_value, 0, 100);

        if (_value <= 30)
        {
            color.a = 0.2f;
        }
        else if (_value <= 60)
        {
            color.a = 0.4f;
        }
        else if (_value <= 80)
        {
            color.a = 0.7f;
        }
        else
        {
            color.a = 1f;
        }

        main.startColor = color;
    }
}
