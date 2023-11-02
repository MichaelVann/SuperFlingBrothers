﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentPanel : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    EquipmentScreenHandler m_equipmentScreenHandlerRef;
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
    public Button m_equipButtonRef;
    public Text m_equipButtonTextRef;
    public Button m_sellButtonRef;
    //public Image m_outlineRef;


    void Awake()
    {
        m_equipmentScreenHandlerRef = FindObjectOfType<EquipmentScreenHandler>();
    }

    public void Init(Equipment a_equipment)
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_equipmentRef = a_equipment;

        for (int i = 0; i < m_statNameTextRefs.Length; i++)
        {
            m_statNameTextRefs[i].color = CharacterStatHandler.GetStatColor(i);
        }
    }

    public void Refresh()
    {
        //m_imageRef.sprite = m_upgradeScreenHandler.m_upgradeSprites[m_upgradeID];
        float[] statDeltas = new float[4];

        m_nameTextRef.text = m_equipmentRef.m_name;
        m_healthText.text = m_equipmentRef.m_health.ToString("f0") + "/" + m_equipmentRef.m_maxHealth.ToString("f0");
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
            m_statTextRefs[index].text = "" + m_equipmentRef.m_stats[i].value;
            statDeltas[index] += m_equipmentRef.m_stats[i].value;
            m_statTextRefs[i].color = Color.white;// CharacterStatHandler.GetStatColor(m_equipmentRef.m_stats[i].statType);
        }

        Equipment openedEquipment = m_gameHandlerRef.m_xCellTeam.m_playerXCell.m_equippedEquipment[m_equipmentScreenHandlerRef.m_openedEquipmentSlotId];
        for (int i = 0; openedEquipment != null && i < openedEquipment.m_stats.Count; i++)
        {
            int index = (int)openedEquipment.m_stats[i].statType;
            statDeltas[index] -= openedEquipment.m_stats[i].value;
        }

        for (int i = 0; i < m_statDeltaTextRefs.Length; i++)
        {
            m_statDeltaTextRefs[i].text = "(" + statDeltas[i] + ")";
            m_statDeltaTextRefs[i].color = statDeltas[i] < 0 ? Color.red : (statDeltas[i] > 0 ? Color.green : Color.white);
        }

        //m_costTextRef.text = "" + m_upgradeRef.m_cost;
        m_levelTextRef.text = "" + m_equipmentRef.m_level;
        m_goldValueTextRef.text = "" + m_equipmentRef.GetGoldValue();
        m_abilityTextRef.text = m_equipmentRef.m_activeAbility.GetName();

        SetEquipButtonStatus();
        m_equipmentPortrait.SetEquipmentRef(m_equipmentRef);
        m_newEquipmentNotifierRef.SetActive(m_equipmentRef.m_newToPlayer);
    }

    void SetEquipButtonStatus()
    {
        Color equipButtonColor = Color.white;
        string equipButtonString = "";
        bool equipped = m_equipmentRef.m_equipped;
        m_sellButtonRef.interactable = !equipped;
        bool sameSlot = m_equipmentScreenHandlerRef.m_openedEquipmentSlotId == m_equipmentRef.m_equippedSlotId;
        if (m_equipmentRef.IsBroken())
        {
            equipButtonColor = Color.grey;
            equipButtonString = "Repair" + "(" +m_equipmentRef.GetRepairCost() + ")";
            m_equipButtonRef.interactable = m_gameHandlerRef.GetCurrentCash() >= m_equipmentRef.GetRepairCost();
        }
        else
        {
            if (!equipped)
            {
                equipButtonColor = Color.white;
                equipButtonString = "Equip";
            }
            else if (equipped && sameSlot)
            {
                equipButtonColor = Color.yellow;
                equipButtonString = "UnEquip";
            }
            else if (equipped && !sameSlot)
            {
                equipButtonColor = Color.red;
                equipButtonString = "ReEquip";
            }
        }
        
        m_equipButtonRef.gameObject.GetComponent<Image>().color = equipButtonColor;
        m_equipButtonTextRef.text = equipButtonString;
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
            m_equipmentScreenHandlerRef.SetEquipStatus(m_equipmentRef);
        }

        Refresh();
    }

    public void SellEquipment()
    {
        m_equipmentScreenHandlerRef.SellEquipment(this);
    }

    public void OpenEquipmentDigest()
    {
        m_equipmentScreenHandlerRef.SetEquipmentDigestStatus(true, m_equipmentRef);
    }
}
