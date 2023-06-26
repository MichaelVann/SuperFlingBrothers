using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour
{
    public Text m_levelText;
    public Text m_nameText;
    public Text[] m_statTexts;
    public EquipmentPortrait m_portraitRef;

    Equipment m_equipmentRef;

    public void SetEquipmentRef(Equipment a_equipment) { m_equipmentRef = a_equipment; }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_statTexts.Length; i++)
        {
            m_statTexts[i].gameObject.SetActive(false);
        }
    }

    public void Refresh()
    {
        bool valid = m_equipmentRef != null;
        if (m_equipmentRef != null)
        {
            m_portraitRef.SetEquipmentRef(m_equipmentRef);
            for (int i = 0; i < m_equipmentRef.m_stats.Count; i++)
            {
                m_statTexts[i].gameObject.SetActive(true);
                m_statTexts[i].text = m_equipmentRef.m_stats[i].statType.ToString() + ": " + m_equipmentRef.m_stats[i].value;
            }
        }
        else
        {
            for (int i = 0; i < m_statTexts.Length; i++)
            {
                m_statTexts[i].text = "";
            }
        }

        m_levelText.text = valid ? "Level: " + m_equipmentRef.m_level : "";
        m_nameText.text = valid ? m_equipmentRef.m_name : "";
        m_nameText.color = valid ? m_equipmentRef.m_rarityTier.color : Color.white;

        m_portraitRef.gameObject.SetActive(valid);
    }

    // Update is called once per frame
    void Update()
    {
        //Refresh();
    }
}
