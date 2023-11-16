using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterSkillBar : MonoBehaviour
{
    UIBar m_UIBarRef;
    public TextMeshProUGUI m_skillNameText;
    internal CharacterStat m_trackedStat;

    internal bool m_animating = false;
    float m_animatingSpeed = 100f;
    float m_lastProgress = 0f;
    float m_currentProgress = 0f;
    float m_totalProgress = 0f;
    vTimer m_lerpTimer;
    float m_lerpTotalTime = 5f;
    float m_lerpExponent = 0.1f;
    int m_lerpSensitivity = 3;

    // Start is called before the first frame update
    void Awake()
    {
        m_UIBarRef = GetComponent<UIBar>();
    }

    internal void Init(CharacterStat a_stat)
    {
        m_trackedStat = a_stat;
        Refresh();
    }

    internal void Refresh()
    {
        m_totalProgress = m_trackedStat.m_RPGLevel.GetXpDifference(m_trackedStat.m_lastSeenRPGLevel, m_trackedStat.m_RPGLevel);

        m_lerpTotalTime = Mathf.Log10(m_totalProgress+1f);
        if (m_totalProgress > 0.01f)
        {
            m_animating = true;
            m_lerpTimer = new vTimer(m_lerpTotalTime, true, true, false);
        }
        else
        {
            m_trackedStat.m_lastSeenRPGLevel.Copy(m_trackedStat.m_RPGLevel);
        }
        m_skillNameText.text = m_trackedStat.name;
        if (m_UIBarRef != null)
        {
            m_UIBarRef.SetBarColor(VLib.COLOR_yellow);
            m_UIBarRef.SetValueTextColor(Color.white);
        }
        RefreshValues();
    }

    void RefreshValues()
    {
        if (m_UIBarRef != null)
        {
            m_UIBarRef.Init(m_trackedStat.m_lastSeenRPGLevel.m_XP, m_trackedStat.m_lastSeenRPGLevel.m_maxXP);
            m_UIBarRef.SetLabeltext("Level " + m_trackedStat.m_lastSeenRPGLevel.m_level);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_animating)
        {
            RPGLevel oldLevel = m_trackedStat.m_lastSeenRPGLevel;
            RPGLevel level = m_trackedStat.m_RPGLevel;
            float m_lerpProgress = 0f;
            m_lerpTimer.Update();
            m_lerpProgress = m_lerpTimer.GetCompletionPercentage();
            m_lerpProgress = VLib.SigmoidLerp(0f, m_totalProgress,m_lerpProgress, m_lerpSensitivity);// VLib.Eerp(0f, m_totalProgress, m_lerpProgress, m_lerpExponent);
            oldLevel.ChangeXP(m_lerpProgress - m_currentProgress);

            m_currentProgress = m_lerpProgress;
            

            int system = 2;

            if (system == 0)
            {
                float remainingXp = m_trackedStat.m_RPGLevel.GetXpDifference(m_trackedStat.m_lastSeenRPGLevel, m_trackedStat.m_RPGLevel);
                float speed = remainingXp * 0.99f;// Mathf.Pow(remainingXp,1.15f) /1f;
                speed = VLib.LilClamp(speed, 1f);
                float xpChange = speed * Time.deltaTime;
                oldLevel.ChangeXP(xpChange);
            }
            else if (system == 1)
            {
                float x = (m_currentProgress / m_totalProgress) * 0.6f + 0.4f;
                float speed = Mathf.Sin(x * Mathf.PI) + 0.05f;
                speed *= m_animatingSpeed;
                float xpChange = speed * Time.deltaTime * (m_totalProgress / 100f);
                oldLevel.ChangeXP(xpChange);
                m_currentProgress += xpChange;
            }



            bool higherLevel = level.m_level > oldLevel.m_level;
            bool higherXP = level.m_XP > oldLevel.m_XP;

            if (!higherLevel && !higherXP)
            {
                oldLevel.m_XP = level.m_XP;
                m_animating = false;
            }
            if (m_currentProgress > m_totalProgress)
            {
                //Debug.Break();
            }
            RefreshValues();
        }
    }
}