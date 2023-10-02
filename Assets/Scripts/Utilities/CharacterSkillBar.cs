using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterSkillBar : MonoBehaviour
{
    UIBar m_UIBarRef;
    public TextMeshProUGUI m_skillNameText;
    internal CharacterStat m_trackedStat;
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
        m_skillNameText.text = m_trackedStat.name;
        if (m_UIBarRef != null)
        {
            m_UIBarRef.Init(m_trackedStat.m_RPGLevel.m_XP, m_trackedStat.m_RPGLevel.m_maxXP);
            m_UIBarRef.SetLabeltext("Level " + m_trackedStat.m_RPGLevel.m_level);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
