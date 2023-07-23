using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour
{
    public Text m_levelText;
    public Text m_nameText;
    public Text[] m_statNameTexts;
    public Text[] m_statTexts;
    public EquipmentPortrait m_portraitRef;
    public Text m_abilityTextRef;

    Equipment m_equipmentRef;

    public void SetEquipmentRef(Equipment a_equipment) { m_equipmentRef = a_equipment; }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_statTexts.Length; i++)
        {
            m_statNameTexts[i].color = CharacterStatHandler.GetStatColor(i);
        }
    }

    public void Refresh()
    {

        bool valid = m_equipmentRef != null;

        for (int i = 0; i < m_statTexts.Length; i++)
        {
            m_statTexts[i].text = "0";
            m_statTexts[i].color = Color.white;
        }

        if (m_equipmentRef != null)
        {
            m_portraitRef.SetEquipmentRef(m_equipmentRef);
            for (int i = 0; i < m_equipmentRef.m_stats.Count; i++)
            {
                //m_statTexts[i].gameObject.SetActive(true);
                int index = (int)m_equipmentRef.m_stats[i].statType;
                m_statTexts[index].text = "" + m_equipmentRef.m_stats[i].value;
                m_statTexts[index].color = Color.green; //CharacterStatHandler.GetStatColor(m_equipmentRef.m_stats[i].statType);
            }
            m_abilityTextRef.text = m_equipmentRef.m_activeAbility.GetName();
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
