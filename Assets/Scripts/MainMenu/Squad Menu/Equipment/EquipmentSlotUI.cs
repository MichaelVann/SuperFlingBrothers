using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour
{
    public Text m_levelText;
    public TextMeshProUGUI m_healthText;
    public Text m_nameText;
    public Text[] m_statNameTexts;
    public Text[] m_statTexts;
    public ArmorSegment m_armorSegment;
    public Text m_abilityTextRef;
    public Text m_itemValueTextRef;

    Equipment m_equipmentRef;

    public void SetEquipmentRef(Equipment a_equipment) { m_equipmentRef = a_equipment; }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_statTexts.Length; i++)
        {
            m_statNameTexts[i].color = CharacterStatHandler.GetStatColor(i);
            m_statNameTexts[i].text = CharacterStatHandler.GetStatName(i);
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

        if (valid)
        {
            m_armorSegment.AssignEquipment(m_equipmentRef);
            for (int i = 0; i < m_equipmentRef.m_stats.Count; i++)
            {
                //m_statTexts[i].gameObject.SetActive(true);
                int index = (int)m_equipmentRef.m_stats[i].statType;
                m_statTexts[index].text = "" + VLib.RoundToDecimalPlaces(CharacterStat.ConvertNominalValueToEffectiveValue(m_equipmentRef.m_stats[i].value, m_equipmentRef.m_stats[i].statType), 2);
                m_statTexts[index].color = Color.green; //CharacterStatHandler.GetStatColor(m_equipmentRef.m_stats[i].statType);
            }
            m_abilityTextRef.text = m_equipmentRef.m_activeAbility.GetName();
            m_itemValueTextRef.text = "" + m_equipmentRef.GetSellValue();
            m_healthText.color = VLib.RatioToColorRGB(m_equipmentRef.m_health / m_equipmentRef.m_maxHealth);
        }
        else
        {
            m_itemValueTextRef.text = "0";
        }

        m_levelText.text = valid ? "Level: " + m_equipmentRef.m_level : "";
        m_nameText.text = valid ? m_equipmentRef.m_name : "";
        m_nameText.color = valid ? m_equipmentRef.m_rarity.color : Color.white;
        m_healthText.text = valid ? m_equipmentRef.m_health.ToString("f1") + "/" + m_equipmentRef.m_maxHealth.ToString("f1") : "";

        m_armorSegment.gameObject.SetActive(valid);
    }

    // Update is called once per frame
    void Update()
    {
        //Refresh();
    }
}
