using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour
{
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
        if (m_equipmentRef != null)
        {
            for (int i = 0; i < m_equipmentRef.m_stats.Count; i++)
            {
                m_statTexts[i].gameObject.SetActive(true);
                m_statTexts[i].text = m_equipmentRef.m_stats[i].statType.ToString() + ": " + m_equipmentRef.m_stats[i].value;
            }
            m_portraitRef.SetEquipmentRef(m_equipmentRef);
        }
        else
        {
            for (int i = 0; i < m_statTexts.Length; i++)
            {
                m_statTexts[i].gameObject.SetActive(false);
            }
        }
        m_portraitRef.gameObject.SetActive(m_equipmentRef != null);
    }

    // Update is called once per frame
    void Update()
    {
        //Refresh();
    }
}
