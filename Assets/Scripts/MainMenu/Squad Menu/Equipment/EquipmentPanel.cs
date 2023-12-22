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

    public TextMeshProUGUI m_abilityTypeText;
    public TextMeshProUGUI m_healthText;
    public TextMeshProUGUI m_rarityTextRef;
    [SerializeField] TextMeshProUGUI m_affixTextRef;
    public Text m_goldValueTextRef;
    public Image m_outline;

    public GameObject m_newEquipmentNotifierRef;

    //public Text m_costTextRef;
    //public GameObject m_levelDisplayRef;
    public TextMeshProUGUI m_levelTextRef;
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


        m_equipButtonRef.Init(m_gameHandlerRef, a_equipmentScreenHandler.m_squadOverviewHandlerRef, m_equipmentRef);
    }

    public void Refresh()
    {
        //m_imageRef.sprite = m_upgradeScreenHandler.m_upgradeSprites[m_upgradeID];
        float[] statDeltas = new float[4];

        m_abilityTypeText.text = m_equipmentRef.m_activeAbility.GetName();
        m_abilityTypeText.color = m_equipmentRef.m_rarity.color;
        m_healthText.text = VLib.RoundToDecimalPlaces(m_equipmentRef.m_health,1) + "/" + VLib.RoundToDecimalPlaces(m_equipmentRef.m_maxHealth,1);
        m_healthText.color = VLib.RatioToColorRGB(m_equipmentRef.m_health / m_equipmentRef.m_maxHealth);
        m_rarityTextRef.text = m_equipmentRef.m_rarity.name;
        m_rarityTextRef.color = m_equipmentRef.m_rarity.color;
        if (m_equipmentRef.m_name != "")
        {
            m_rarityTextRef.text += "\"" + m_equipmentRef.m_name + "\"";
        }
        if (m_equipmentRef.m_activeAbility.m_affixes.Count > 0)
        {
            m_affixTextRef.text ="";

        }
        else
        {
            m_affixTextRef.text =  "No affixes";
            m_affixTextRef.color = new Color(0.8f,0.8f,0.8f);
        }

        for (int i = 0; i < m_equipmentRef.m_activeAbility.m_affixes.Count; i++)
        {
            if (i > 0)
            {
                m_affixTextRef.text += ", ";
            }
            m_affixTextRef.text += m_equipmentRef.m_activeAbility.m_affixes[i].ToString();
            m_affixTextRef.color = Color.red;

        }


        Equipment openedEquipment = m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_equippedEquipment[m_equipmentInventoryHandlerRef.m_squadOverviewHandlerRef.m_openedEquipmentSlotId];


        //m_costTextRef.text = "" + m_upgradeRef.m_cost;
        m_levelTextRef.text = "" + m_equipmentRef.m_level;
        m_goldValueTextRef.text = "" + m_equipmentRef.GetSellValue();
        //m_abilityTextRef.text = m_equipmentRef.m_activeAbility.GetName();

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

        if (m_equipmentInventoryHandlerRef.gameObject.activeSelf)
        {
            Refresh();
        }
    }

    public void SellEquipment()
    {
        m_equipmentInventoryHandlerRef.SellEquipment(this);
    }

    public void OpenEquipmentDigest()
    {
        m_equipmentInventoryHandlerRef.SetEquipmentDigestStatus(true, m_equipmentRef);
        Refresh();
    }
}
