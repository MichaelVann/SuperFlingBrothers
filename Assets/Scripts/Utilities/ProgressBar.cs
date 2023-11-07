using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public float m_progressMax = 1f;
    public float m_postProcess = 1f;
    public float m_progress = 0f;
    public float m_preProgress = 0f;
    public bool m_lagEffect = true;
    vTimer m_lagTimer;
    bool m_lagging = false;
    const float m_maxLagIncrement = 300f;
    const float m_lagDelay = 0.23f;

    bool m_whiteLagWindup = false;

    public SpriteRenderer m_progressBarRef;
    public SpriteRenderer m_barOccluder;
    public GameObject m_whiteLagBar;
    Vector3 m_originalScale;

    bool m_healthColoring = true;

    public void SetMaxProgressValue(float a_value) { m_progressMax = a_value; }
    public void SetHealthColoring(bool a_value) { m_healthColoring = a_value; }
    public void SetProgressValue(float a_value)
    {
        if (m_lagEffect && (a_value <= m_preProgress))
        {
            if (a_value != m_preProgress)
            {
                m_preProgress = a_value;
                m_lagTimer.Reset();
                m_lagTimer.SetActive(true);
                m_lagging = true;
            }
        }
        else
        {
            m_postProcess = a_value;
            m_progress = a_value;
            m_preProgress = a_value;
        }
    }

    void Awake()
    {
        m_originalScale = m_barOccluder.transform.localScale;
        if (m_lagEffect)
        {
            m_lagTimer = new vTimer(m_lagDelay,true,false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_lagEffect)
        {
            if (m_whiteLagWindup)
            {
                m_progress -= Mathf.Clamp(m_progress - m_preProgress * Time.deltaTime, 0f, m_maxLagIncrement * Time.deltaTime);
                m_progress = Mathf.Clamp(m_progress, m_preProgress, m_progressMax);
            }
            else
            {
                m_progress = m_preProgress;
            }

            if (m_lagTimer.Update())
            {
                m_lagging = false;
            }
            if (!m_lagging && m_postProcess > m_progress)
            {
                m_postProcess -= Mathf.Clamp(m_postProcess - m_progress * Time.deltaTime, 0f, m_maxLagIncrement * Time.deltaTime);
                m_postProcess = Mathf.Clamp(m_postProcess, m_progress, m_progressMax);
            }
        }

        if (m_progressMax != 0)
        {
            float healthPercentage = m_postProcess / m_progressMax;
            float preHealthPercentage = m_progress / m_progressMax;
            //m_progressBarRef.gameObject.transform.localScale = new Vector3(m_originalScale.x * healthPercentage, m_originalScale.y,1f);
            m_barOccluder.gameObject.transform.localScale = new Vector3(m_originalScale.x * (1f - healthPercentage), m_originalScale.y, 1f);
            m_whiteLagBar.transform.localScale = new Vector3(m_originalScale.x * (1f - preHealthPercentage), m_originalScale.y, 1f);
            if (m_healthColoring)
            {
                Color barColor = VLib.PercentageToColor(healthPercentage);
                m_progressBarRef.color = barColor;
            }
        }
        else
        {
        }
    }
}
