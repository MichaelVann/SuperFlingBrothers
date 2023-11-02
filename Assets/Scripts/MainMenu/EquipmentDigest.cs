using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDigest : MonoBehaviour
{
    public Text m_levelText;
    public Text m_nameText;
    public Text[] m_statNameTexts;
    public Text[] m_statTexts;
    public Text[] m_affixTexts;
    public EquipmentPortrait m_portraitRef;
    public Text m_abilityTextRef;
    public Text m_itemValueTextRef;
    public EquipmentScreenHandler m_equipmentScreenHandlerRef;

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
            m_itemValueTextRef.text = "" + m_equipmentRef.GetGoldValue();
        }
        else
        {
            m_itemValueTextRef.text = "0";
        }

        m_levelText.text = valid ? "Level: " + m_equipmentRef.m_level : "";
        m_nameText.text = valid ? m_equipmentRef.m_name : "";
        m_nameText.color = valid ? m_equipmentRef.m_rarity.color : Color.white;


        m_portraitRef.gameObject.SetActive(valid);

        for (int i = 0; i < m_affixTexts.Length && i < m_equipmentRef.m_activeAbility.m_affixes.Count; i++)
        {
            m_affixTexts[i].text = VLib.GetEnumName<EquipmentAbility.eAffix>(m_equipmentRef.m_activeAbility.m_affixes[i]);
        }

    }

    public void Close()
    {
        m_equipmentScreenHandlerRef.SetEquipmentDigestStatus(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Refresh();
    }
}
