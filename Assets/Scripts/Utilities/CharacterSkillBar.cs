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
    float m_animatingMaxSpeed = 10f;

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
        bool higherLevel = m_trackedStat.m_RPGLevel.m_level > m_trackedStat.m_lastSeenRPGLevel.m_level;
        bool higherXP = m_trackedStat.m_RPGLevel.m_XP > m_trackedStat.m_lastSeenRPGLevel.m_XP;
        if (higherLevel || higherXP)
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

            oldLevel.ChangeXP((int)m_animatingMaxSpeed*Time.deltaTime);

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
