using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ZoomExpandComponent;

public class ZoomExpandComponent : MonoBehaviour
{
    float m_startScale = 1f;
    float m_endScale = 1f;
    float m_zoomTime = 1f;
    float m_exponent = 1f;
    vTimer m_timer;

    internal delegate void FinishZoomDelegate();
    internal FinishZoomDelegate m_finishZoomDelegate;

    // Start is called before the first frame update
    void Start()
    {
        m_timer = new vTimer(m_zoomTime, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_timer.Update())
        {
            if (m_finishZoomDelegate != null)
            {
                m_finishZoomDelegate.Invoke();
            }
            Destroy(this);
            return;
        }
        float scale = VLib.Eerp(m_startScale, m_endScale, m_timer.GetCompletionPercentage(), m_exponent);
        gameObject.transform.localScale = new Vector3 (scale, scale, scale);
    }

    internal void SetUp(float a_startScale = 0f, float a_endScale = 1f, float a_zoomTime = 0.3f, float a_exponent = 2f, FinishZoomDelegate a_finishZoomDelegate = null)
    {
        m_startScale = a_startScale;
        m_endScale = a_endScale;
        m_zoomTime = a_zoomTime;
        m_exponent = a_exponent;
        m_finishZoomDelegate = a_finishZoomDelegate;
    }
}
