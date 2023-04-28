using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class vTimer
{
    float m_timer;
    float m_timerMax;
    bool m_onceOff;
    bool m_finished;
    bool m_active;

    public void SetActive(bool a_active) { m_active = a_active; }

    private void Init(float a_maxTime, bool a_onceOff, bool a_active)
    {
        m_timer = 0f;
        m_timerMax = a_maxTime;
        m_onceOff = a_onceOff;
        m_active = a_active;
    }

    public vTimer(float a_maxTime, bool a_onceOff = false, bool a_active = true)
    {
        Init(a_maxTime, a_onceOff, a_active);
    }

    public void Reset()
    {
        m_timer = 0f;
    }

    public bool Update()
    {
        if (!m_finished && m_active)
        {
            m_timer += Time.deltaTime;
        }

        if (m_timer >= m_timerMax && m_active)
        {
            if (m_onceOff)
            {
                m_finished = true;
            }
            m_timer -= m_timerMax;
            return true;
        }
        return false;
    }
}
