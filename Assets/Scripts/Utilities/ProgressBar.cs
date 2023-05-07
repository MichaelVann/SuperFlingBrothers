﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    public float m_progressMax = 1f;
    float m_progress = 0f;

    public SpriteRenderer m_progressBarRef;
    Vector3 m_originalScale;

    bool m_healthColoring = true;


    public void SetMaxProgressValue(float a_value) { m_progressMax = a_value; }
    public void SetHealthColoring(bool a_value) { m_healthColoring = a_value; }
    public void SetProgressValue(float a_value)
    {
        m_progress = a_value;
    }


    void Start()
    {
        m_originalScale = new Vector3(0.95f, 0.6f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        float healthPercentage = m_progress / m_progressMax;
        m_progressBarRef.gameObject.transform.localScale = new Vector3(m_originalScale.x * healthPercentage, m_originalScale.y,1f);
        if (m_healthColoring)
        {
            Color barColor = VLib.PercentageToColor(healthPercentage);
            m_progressBarRef.color = barColor;
        }
    }
}
