using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipableSlotUI : MonoBehaviour
{
    public Text[] m_statTexts;

    Equipable m_equipableRef;

    public void SetEquipableRef(Equipable a_equipable) { m_equipableRef = a_equipable; }

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
        if (m_equipableRef != null)
        {
            for (int i = 0; i < m_equipableRef.m_stats.Count; i++)
            {
                m_statTexts[i].gameObject.SetActive(true);
                m_statTexts[i].text = m_equipableRef.m_stats[i].statType.ToString() + ": " + m_equipableRef.m_stats[i].value;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Refresh();
    }
}
