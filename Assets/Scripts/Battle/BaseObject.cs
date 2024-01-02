using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour
{
    internal Rigidbody2D m_rigidBody;
    public SpriteRenderer m_spriteRenderer;
    Vector3 m_spriteOriginalPosition;
    float m_shakeAmount = 0;
    Vector3 m_shakeOffset = Vector3.zero;
    float m_shakeDecay = 0.9f;

    protected void SetShakeAmount(float a_shakeAmount) { m_shakeAmount = a_shakeAmount; }

    public virtual void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_spriteOriginalPosition = m_spriteRenderer.gameObject.transform.localPosition;
        //m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void ShakeUpdate()
    {
        if (m_shakeAmount > 0)
        {
            m_shakeOffset += new Vector3(VLib.vRandom(-m_shakeAmount, m_shakeAmount), VLib.vRandom(-m_shakeAmount, m_shakeAmount), 0f);
        }
        if (m_shakeOffset != Vector3.zero)
        {

            m_shakeOffset *= m_shakeDecay;
        }
        m_spriteRenderer.transform.localPosition = m_spriteOriginalPosition + m_shakeOffset;
    }

    public virtual void Update()
    {
        ShakeUpdate();
    }
}