using System;
using UnityEngine;
using UnityEngine.Assertions;

public class HandsListener : MonoBehaviour
{
    private const string ANIMATOR_BOOL_ACTIVE = "Active";

    [SerializeField]
    private HandListener m_leftListener = null;
    [SerializeField]
    private HandListener m_rightListener = null;

    [SerializeField]
    private GameObject m_root = null;
    [SerializeField]
    private Animator m_animator = null;

    [SerializeField, Range(0, 1)]
    private float m_thresholdDistance = 0.3f;

    [SerializeField]
    private Emotion m_emotion = Emotion.ZONE;
    [SerializeField, Range(0, 100)]
    private float m_heartBeat = 0;

    private void Awake()
    {
        Assert.IsNotNull(m_leftListener);
        Assert.IsNotNull(m_rightListener);
    
        Assert.IsNotNull(m_animator);
    }

    private void Update()
    {
        UpdateHandsEmotion();

        if (!m_leftListener.IsActive || !m_rightListener.IsActive)
        {
            //Debug.Log($"At least one of both hand is not detected | Left: {m_leftListener.IsActive} | Right: {m_rightListener.IsActive}");
            SetCenterObjectActivated(false);
            return;
        }

        m_root.transform.position = (m_leftListener.transform.position + m_rightListener.transform.position) / 2;
        
        float distance = Vector3.Distance(m_leftListener.transform.position, m_rightListener.transform.position);
        SetCenterObjectActivated(distance < m_thresholdDistance);
    }

    private void UpdateHandsEmotion()
    {
        Color color = m_emotion switch
        {
            Emotion.FOCUS => new Color(0.8705883f, 0.8156863f, 0.3960785f),
            Emotion.ZONE => new Color(0.2784314f, 0.4470589f, 0.8196079f),
            Emotion.ENJOYMENT => new Color(0.7764707f, 0.1411765f, 0.1294118f),
            _ => Color.white
        };

        m_leftListener.UpdateParticleSystemColor(color);
        m_leftListener.UpdateHeartBeatTransparency(color, m_heartBeat);

        m_rightListener.UpdateParticleSystemColor(color);
        m_rightListener.UpdateHeartBeatTransparency(color, m_heartBeat);
    }

    //private void UpdateHandsEmotion()
    //{
    //    Color color = m_emotion switch
    //    {
    //        Emotion.FOCUS => new Color(0.8705883f, 0.8156863f, 0.3960785f),
    //        Emotion.ZONE => new Color(0.2784314f, 0.4470589f, 0.8196079f),
    //        Emotion.ENJOYMENT => new Color(0.7764707f, 0.1411765f, 0.1294118f),
    //        _ => Color.white
    //    };

    //    m_leftListener.UpdateParticleSystemColor(color);
    //    m_leftListener.UpdateHeartBeatTransparency(m_heartBeat);

    //    m_rightListener.UpdateParticleSystemColor(color);
    //    m_rightListener.UpdateHeartBeatTransparency(m_heartBeat);
    //}

    private void SetCenterObjectActivated(bool _value)
    {
        m_animator.SetBool(ANIMATOR_BOOL_ACTIVE, _value);
    }
}