using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SquadScreenHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public EquipmentSlotUI[] m_equipmentSlotUIRefs;

    public int m_openedEquipmentSlotId = -1;


    public GameObject m_statAllocationNotifierRef;
    public Text m_statAllocationNotifierTextRef;

    //Inventory
    public GameObject m_inventoryPanelRef;

    //Subscreens
    public GameObject m_squadOverview;
    public GameObject m_attributesScreen;
    public GameObject m_skillsScreen;
    public GameObject m_storeScreen;

    private bool m_inited = false;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        RefreshEquipmentSlots();
        m_inited = true;
    }

    // Update is called once per frame
    void Update()
    {
        int allocationPoints = m_gameHandlerRef.m_xCellSquad.m_statHandler.m_RPGLevel.m_allocationPoints;
        m_statAllocationNotifierRef.SetActive(allocationPoints > 0);
        if (m_statAllocationNotifierRef.activeSelf)
        {
            if (allocationPoints > 99)
            {
                m_statAllocationNotifierTextRef.text = "99+";
            }
            else
            {
                m_statAllocationNotifierTextRef.text = "" + allocationPoints;
            }
        }
    }

    private void OnEnable()
    {
        if (m_inited)
        {
            RefreshEquipmentSlots();
        }
    }

    public void RefreshEquipmentSlots()
    {
        for (int i = 0; i < m_equipmentSlotUIRefs.Length; i++)
        {
            m_equipmentSlotUIRefs[i].SetEquipmentRef(m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_equippedEquipment[i]);
            m_equipmentSlotUIRefs[i].Refresh();
        }
    }


    public void OpenEquipmentSlot(int a_id)
    {
        SetInventoryPanelStatus(true, a_id);
    }

    public void CloseInventoryPanel()
    {
        RefreshEquipmentSlots();
        SetInventoryPanelStatus(false);
    }

    public void SetInventoryPanelStatus(bool a_open, int a_slotId = -1)
    {
        m_openedEquipmentSlotId = a_slotId;
        m_inventoryPanelRef.SetActive(a_open);
        m_squadOverview.SetActive(!a_open);
    }

    internal void SetEquipStatus(Equipment a_equipment)
    {
        m_gameHandlerRef.m_xCellSquad.m_playerXCell.EquipEquipment(a_equipment, m_openedEquipmentSlotId);
        RefreshEquipmentSlots();
    }


    void CloseAllScreens()
    {
        m_squadOverview.SetActive(false);
        m_attributesScreen.SetActive(false);
        m_skillsScreen.SetActive(false);
        m_storeScreen.SetActive(false);
    }

    public void OpenSquadOverview()
    {
        CloseAllScreens();
        m_squadOverview.SetActive(true);
    }

    public void OpenAttributesScreen()
    {
        CloseAllScreens();
        m_attributesScreen.SetActive(true);
    }

    public void OpenSkillsScreen()
    {
        CloseAllScreens();
        m_skillsScreen.SetActive(true);
    }

    public void OpenStoreScreen()
    {
        CloseAllScreens();
        m_storeScreen.SetActive(true);
    }
}
