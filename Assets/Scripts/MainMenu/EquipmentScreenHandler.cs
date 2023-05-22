using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentScreenHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public EquipmentSlotUI[] m_equipmentSlotUIRefs;
    public int m_openedEquipmentSlotId = -1;

    public GameObject m_inventoryPanelRef;

    public GameObject m_inventoryContentRef;
    public GameObject m_equipmentPanelTemplate;

    List<EquipmentPanel> m_equipmentItemPanels;

    public GameObject m_noEquipmentText;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_equipmentItemPanels = new List<EquipmentPanel>();
        RefreshEquipmentSlots();
        for (int i = 0; i < m_gameHandlerRef.m_equipmentInventory.Length; i++)
        {
            if (m_gameHandlerRef.m_equipmentInventory[i] != null)
            {
                EquipmentPanel equipmentPanel = Instantiate<GameObject>(m_equipmentPanelTemplate, m_inventoryContentRef.transform).GetComponent<EquipmentPanel>();
                equipmentPanel.Init(m_gameHandlerRef.m_equipmentInventory[i]);
                //equipablePanel.Refresh();
                m_equipmentItemPanels.Add(equipmentPanel);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void RefreshInventory()
    {
        m_gameHandlerRef.m_equipmentCollectedLastGame = 0;
        m_noEquipmentText.SetActive(m_equipmentItemPanels.Count < 1);
        for (int i = 0; i < m_equipmentItemPanels.Count; i++)
        {
            m_equipmentItemPanels[i].Refresh();
        }
    }


    public void RefreshEquipmentSlots()
    {
        for (int i = 0; i < m_equipmentSlotUIRefs.Length; i++)
        {
            m_equipmentSlotUIRefs[i].SetEquipmentRef(m_gameHandlerRef.m_playerStatHandler.m_equippedEquiment[i]);
            m_equipmentSlotUIRefs[i].Refresh();
        }
    }

    public void SetEquipStatus(Equipment a_equipment)
    {
        m_gameHandlerRef.m_playerStatHandler.EquipEquipment(a_equipment, m_openedEquipmentSlotId);
        CloseInventoryPanel();
        RefreshEquipmentSlots();
    }

    public void OpenEquipmentSlot(int a_id)
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
        m_openedEquipmentSlotId = a_slotId;
        if (a_open)
        {
            RefreshInventory();
        }
    }

    public void SellEquipment(EquipmentPanel a_equipmentPanel)
    {
        for (int i = 0; i < m_equipmentItemPanels.Count; i++)
        {
            if (m_equipmentItemPanels[i] == a_equipmentPanel)
            {
                GameObject.Destroy(a_equipmentPanel.gameObject);
                m_equipmentItemPanels.RemoveAt(i);
                m_gameHandlerRef.SellEquipment(a_equipmentPanel.m_equipmentRef);
                break;
            }
        }
        RefreshInventory();
    }
}
