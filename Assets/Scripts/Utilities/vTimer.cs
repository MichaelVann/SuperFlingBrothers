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

    private void Init(float a_maxTime, bool a_onceOff)
    {
        m_timer = 0f;
        m_timerMax = a_maxTime;
        a_onceOff = false;
    }

    public vTimer(float a_maxTime, bool a_onceOff)
    {
        Init(a_maxTime, a_onceOff);
    }

    public vTimer(float a_maxTime)
    {
        Init(a_maxTime, false);
    }

    public bool Update()
    {
        if (!m_finished)
        {
            m_timer += Time.deltaTime;
        }

        if (m_timer >= m_timerMax)
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
