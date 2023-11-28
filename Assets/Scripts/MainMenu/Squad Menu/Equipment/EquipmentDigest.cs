using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDigest : MonoBehaviour
{
    public Text m_levelText;
    public Text m_nameText;
    public Text[] m_statNameTexts;
    public Text[] m_statTexts;
    public TextMeshProUGUI m_affixText;
    public EquipmentPortrait m_portraitRef;
    public Text m_abilityTextRef;
    public Text m_itemValueTextRef;
    public EquipmentInventoryHandler m_equipmentInventoryHandler;
    public EquipmentInteractButton m_interactButtonRef;

    GameHandler m_gameHandlerRef;

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
        if (m_gameHandlerRef == null)
        {
            m_gameHandlerRef = FindObjectOfType<GameHandler>();
        }
        if (m_equipmentInventoryHandler == null)
        {
            m_equipmentInventoryHandler = FindObjectOfType<EquipmentInventoryHandler>();
        }
        bool valid = m_equipmentRef != null;
        m_interactButtonRef.Init(m_gameHandlerRef, m_equipmentInventoryHandler.m_squadOverviewHandlerRef, m_equipmentRef);
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
                m_statTexts[index].text = "" + VLib.RoundToDecimalPlaces(m_equipmentRef.m_stats[i].value, Equipment.m_statRoundedDecimals);
                m_statTexts[index].color = Color.green; //CharacterStatHandler.GetStatColor(m_equipmentRef.m_stats[i].statType);
            }
            m_abilityTextRef.text = m_equipmentRef.m_activeAbility.GetName();
            m_itemValueTextRef.text = "" + m_equipmentRef.GetSellValue();
        }
        else
        {
            m_itemValueTextRef.text = "0";
        }

        m_levelText.text = valid ? "Level: " + m_equipmentRef.m_level : "";
        m_nameText.text = valid ? m_equipmentRef.m_name : "";
        m_nameText.color = valid ? m_equipmentRef.m_rarity.color : Color.white;


        m_portraitRef.gameObject.SetActive(valid);
        m_affixText.text = "";
        m_affixText.text += m_equipmentRef.m_activeAbility.GetAbilityDescription() + '\n';
        m_interactButtonRef.SetEquipButtonStatus();
    }

    public void Close()
    {
        m_equipmentInventoryHandler.SetEquipmentDigestStatus(false);
    }

    public void InteractButtonPressed()
    {
        if (m_equipmentRef.IsBroken())
        {
            m_gameHandlerRef.AttemptToRepairEquipment(m_equipmentRef);
        }
        else
        {
            m_equipmentInventoryHandler.SetEquipStatus(m_equipmentRef);
        }
        Refresh();
        Close();
    }

    // Update is called once per frame
    void Update()
    {
        //Refresh();
    }
}
