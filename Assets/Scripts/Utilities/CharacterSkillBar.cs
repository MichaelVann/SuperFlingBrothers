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
    float m_currentProgress = 0f;
    float m_totalProgress = 0f;

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
        if (m_totalProgress > 0)
        {
            m_animating = true;
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
            float speed = Mathf.Sin((m_currentProgress / m_totalProgress) * Mathf.PI) + 0.2f;
            speed *= m_animatingSpeed;
            float xpChange = (int)speed * Time.deltaTime;
            oldLevel.ChangeXP(xpChange);
            m_currentProgress += xpChange;

            bool higherLevel = level.m_level > oldLevel.m_level;
            bool higherXP = level.m_XP >= oldLevel.m_XP;

            if (!higherLevel && !higherXP)
            {
                oldLevel.m_XP = level.m_XP;
                m_animating = false;
            }
            RefreshValues();
        }
    }
}
