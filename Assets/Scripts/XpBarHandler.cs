using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XpBarHandler : MonoBehaviour
{
    float m_xpBarLength = 650f;
    float m_xpBarHeight = 25f;

    public RectTransform m_XPBarMaskTransform;
    GameHandler m_gameHandlerRef;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        m_XPBarMaskTransform.sizeDelta = new Vector2(m_xpBarLength * ((float)m_gameHandlerRef.m_XP / (float)m_gameHandlerRef.m_maxXP), m_xpBarHeight);
    }
}
