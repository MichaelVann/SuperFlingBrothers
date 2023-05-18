﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentScreenHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public EquipableSlotUI[] m_equipableSlotUIRefs;
    int m_openedEquipableSlotId = -1;

    public GameObject m_inventoryPanelRef;

    public GameObject m_inventoryContentRef;
    public GameObject m_equipablePanelTemplate;

    List<EquipablePanel> m_equipableItemPanels;


    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_equipableItemPanels = new List<EquipablePanel>();
        for (int i = 0; i < m_equipableSlotUIRefs.Length; i++)
        {
            m_equipableSlotUIRefs[i].SetEquipableRef(m_gameHandlerRef.m_equipablesEquipped[i]);
        }

        for (int i = 0; i < m_gameHandlerRef.m_equipablesInventory.Length; i++)
        {
            if (m_gameHandlerRef.m_equipablesInventory[i] != null)
            {
                EquipablePanel equipablePanel = Instantiate<GameObject>(m_equipablePanelTemplate, m_inventoryContentRef.transform).GetComponent<EquipablePanel>();
                equipablePanel.Init(m_gameHandlerRef.m_equipablesInventory[i]);
                equipablePanel.Refresh();
                m_equipableItemPanels.Add(equipablePanel);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void RefreshInventory()
    {
        for (int i = 0; i < m_equipableItemPanels.Count; i++)
        {
            m_equipableItemPanels[i].Refresh();
        }
    }


    public void RefreshEquipableSlots()
    {
        for (int i = 0; i < m_equipableItemPanels.Count; i++)
        {
            m_equipableSlotUIRefs[i].SetEquipableRef(m_gameHandlerRef.m_equipablesEquipped[i]);
            m_equipableSlotUIRefs[i].Refresh();
        }
    }

    public bool AttemptToEquip(Equipable a_equipable)
    {
        bool returnVal = false;
        m_gameHandlerRef.m_equipablesEquipped[m_openedEquipableSlotId] = a_equipable;
        a_equipable.m_equipped = true;
        CloseInventoryPanel();
        RefreshEquipableSlots();
        return returnVal;
    }

    public void OpenEquipableSlot(int a_id)
    {
        SetInventoryPanelStatus(true, a_id);
    }

    public void CloseInventoryPanel()
    {
        SetInventoryPanelStatus(false);
    }

    public void SetInventoryPanelStatus(bool a_open, int a_slotId = -1)
    {
        m_inventoryPanelRef.SetActive(a_open);
        m_openedEquipableSlotId = a_slotId;
    }
}
