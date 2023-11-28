using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentPanel : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    EquipmentInventoryHandler m_equipmentInventoryHandlerRef;

    public Equipment m_equipmentRef;

    public Text m_nameTextRef;
    public TextMeshProUGUI m_healthText;
    public Text m_rarityTextRef;
    public Text[] m_statNameTextRefs;
    public Text[] m_statTextRefs;
    public Text[] m_statDeltaTextRefs;
    public Text m_abilityTextRef;
    public Text m_goldValueTextRef;
    public Image m_outline;

    public GameObject m_newEquipmentNotifierRef;

    //public Text m_costTextRef;
    //public GameObject m_levelDisplayRef;
    public Text m_levelTextRef;
    public EquipmentPortrait m_equipmentPortrait;
    public EquipmentInteractButton m_equipButtonRef;
    public Text m_equipButtonTextRef;
    public Button m_sellButtonRef;
    //public Image m_outlineRef;

    void Awake()
    {

    }

    public void Init(Equipment a_equipment, EquipmentInventoryHandler a_equipmentScreenHandler)
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_equipmentInventoryHandlerRef = a_equipmentScreenHandler;
        m_equipmentRef = a_equipment;

        for (int i = 0; i < m_statNameTextRefs.Length; i++)
        {
            m_statNameTextRefs[i].color = CharacterStatHandler.GetStatColor(i);
            m_statNameTextRefs[i].text = CharacterStatHandler.GetStatName(i, true);
        }
        m_equipButtonRef.Init(m_gameHandlerRef, a_equipmentScreenHandler.m_squadOverviewHandlerRef, m_equipmentRef);
    }

    public void Refresh()
    {
        //m_imageRef.sprite = m_upgradeScreenHandler.m_upgradeSprites[m_upgradeID];
        float[] statDeltas = new float[4];

        m_nameTextRef.text = m_equipmentRef.m_name;
        m_healthText.text = VLib.RoundToDecimalPlaces(m_equipmentRef.m_health,1) + "/" + VLib.RoundToDecimalPlaces(m_equipmentRef.m_maxHealth,1);
        m_healthText.color = VLib.PercentageToColor(m_equipmentRef.m_health / m_equipmentRef.m_maxHealth);
        m_rarityTextRef.text = m_equipmentRef.m_rarity.name;
        m_rarityTextRef.color = m_equipmentRef.m_rarity.color;
        m_nameTextRef.color = m_equipmentRef.m_rarity.color;

        for (int i = 0; i < m_statTextRefs.Length; i++)
        {
            m_statTextRefs[i].text = "0";
        }

        for (int i = 0; i < m_equipmentRef.m_stats.Count; i++)
        {
            int index = (int)m_equipmentRef.m_stats[i].statType;

            float statEffectiveValue = CharacterStat.ConvertNominalValueToEffectiveValue(m_equipmentRef.m_stats[i].value, m_equipmentRef.m_stats[i].statType);

            m_statTextRefs[index].text = "" + VLib.RoundToDecimalPlaces(statEffectiveValue, Equipment.m_statRoundedDecimals);
            statDeltas[index] += statEffectiveValue;
            m_statTextRefs[i].color = Color.white;// CharacterStatHandler.GetStatColor(m_equipmentRef.m_stats[i].statType);
        }

        Equipment openedEquipment = m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_equippedEquipment[m_equipmentInventoryHandlerRef.m_squadOverviewHandlerRef.m_openedEquipmentSlotId];
        for (int i = 0; openedEquipment != null && i < openedEquipment.m_stats.Count; i++)
        {

            int index = (int)openedEquipment.m_stats[i].statType;
            float statEffectiveValue = CharacterStat.ConvertNominalValueToEffectiveValue(openedEquipment.m_stats[i].value, openedEquipment.m_stats[i].statType);
            statDeltas[index] -= statEffectiveValue;
        }

        for (int i = 0; i < m_statDeltaTextRefs.Length; i++)
        {
            m_statDeltaTextRefs[i].text = "(" + VLib.RoundToDecimalPlaces(statDeltas[i], Equipment.m_statRoundedDecimals) + ")";
            m_statDeltaTextRefs[i].color = statDeltas[i] < 0 ? Color.red : (statDeltas[i] > 0 ? Color.green : Color.white);
        }

        //m_costTextRef.text = "" + m_upgradeRef.m_cost;
        m_levelTextRef.text = "" + m_equipmentRef.m_level;
        m_goldValueTextRef.text = "" + m_equipmentRef.GetSellValue();
        m_abilityTextRef.text = m_equipmentRef.m_activeAbility.GetName();

        SetEquipButtonStatus();
        m_equipmentPortrait.SetEquipmentRef(m_equipmentRef);
        m_newEquipmentNotifierRef.SetActive(m_equipmentRef.m_newToPlayer);
    }

    void SetEquipButtonStatus()
    {
        m_equipButtonRef.SetEquipButtonStatus();
        bool equipped = m_equipmentRef.m_equipped;
        m_sellButtonRef.interactable = !equipped;
    }

    internal void SetSelected(bool a_selected)
    {
        m_outline.color = a_selected ? Color.yellow : Color.black;
        Refresh();
    }

    public void InteractButtonPressed()
    {
        if (m_equipmentRef.IsBroken())
        {
            m_gameHandlerRef.AttemptToRepairEquipment(m_equipmentRef);
        }
        else
        {
            m_equipmentInventoryHandlerRef.SetEquipStatus(m_equipmentRef);
        }

        Refresh();
    }

    public void SellEquipment()
    {
        m_equipmentInventoryHandlerRef.SellEquipment(this);
    }

    public void OpenEquipmentDigest()
    {
        m_equipmentInventoryHandlerRef.SetEquipmentDigestStatus(true, m_equipmentRef);
    }
}
