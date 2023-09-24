using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentScreenHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public EquipmentSlotUI[] m_equipmentSlotUIRefs;
    public EquipmentSlotUI m_inventoryEquipmentSlotUIRef;
    public Text m_equipmentAbilityReadoutText;
    public Text m_equipmentAbilityNameText;
    public int m_openedEquipmentSlotId = -1;
    public GameObject m_equipmentDigestRef;

    public GameObject m_inventoryPanelRef;

    public GameObject m_inventoryContentRef;
    public GameObject m_equipmentPanelTemplate;
    public GameObject m_equipmentOverview;

    List<EquipmentPanel> m_equipmentItemPanels;

    public GameObject m_noEquipmentText;

    private bool m_inited = false;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_equipmentItemPanels = new List<EquipmentPanel>();
        RefreshEquipmentSlots();
        InstantiateEquipmentInventory();
        m_inited = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        if (m_inited)
        {
            RefreshEquipmentSlots();
        }
    }

    void InstantiateEquipmentInventory()
    {
        for (int i = 0; i < m_equipmentItemPanels.Count; i++)
        {
            GameObject.Destroy(m_equipmentItemPanels[i].gameObject);
        }
        m_equipmentItemPanels.Clear();
        for (int i = 0; i < m_gameHandlerRef.m_equipmentInventory.Count; i++)
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

    public void RefreshInventory()
    {
        m_gameHandlerRef.SortEquipmentInventory();
        InstantiateEquipmentInventory();
        m_gameHandlerRef.m_equipmentCollectedLastGame = 0;
        m_noEquipmentText.SetActive(m_equipmentItemPanels.Count < 1);
        for (int i = 0; i < m_equipmentItemPanels.Count; i++)
        {
            m_equipmentItemPanels[i].Refresh();
        }

        //Top Panel
        SetTopPanelEquipmentRef(m_gameHandlerRef.m_playerXCell.m_equippedEquipment[m_openedEquipmentSlotId]);
        //m_inventoryEquipmentSlotUIRef.SetEquipmentRef(m_gameHandlerRef.m_playerStatHandler.m_equippedEquipment[m_openedEquipmentSlotId]);
        //m_inventoryEquipmentSlotUIRef.Refresh();
        //if (m_gameHandlerRef.m_playerStatHandler.m_equippedEquipment[m_openedEquipmentSlotId] != null)
        //{
        //    m_equipmentAbilityNameText.text = m_gameHandlerRef.m_playerStatHandler.m_equippedEquipment[m_openedEquipmentSlotId].m_activeAbility.GetName();
        //    m_equipmentAbilityReadoutText.text = m_gameHandlerRef.m_playerStatHandler.m_equippedEquipment[m_openedEquipmentSlotId].m_activeAbility.GetAbilityDescription();
        //}
    }

    internal void SetTopPanelEquipmentRef(Equipment a_equipment)
    {
        m_inventoryEquipmentSlotUIRef.SetEquipmentRef(a_equipment);
        m_inventoryEquipmentSlotUIRef.Refresh();
        if (a_equipment != null)
        {
            m_equipmentAbilityNameText.text = a_equipment.m_activeAbility.GetName();
            m_equipmentAbilityReadoutText.text = a_equipment.m_activeAbility.GetAbilityDescription();
        }
        else
        {
            m_equipmentAbilityNameText.text = "";
            m_equipmentAbilityReadoutText.text = "";
        }
        for (int i = 0; i < m_equipmentItemPanels.Count; i++)
        {
            m_equipmentItemPanels[i].SetSelected(m_equipmentItemPanels[i].m_equipmentRef == a_equipment);
        }
    }

    internal void SetEquipmentDigestStatus(bool a_open, Equipment a_equipment = null)
    {
        SetTopPanelEquipmentRef(a_equipment);
        //m_equipmentDigestRef.SetActive(a_open);

        if (a_open)
        {
            m_equipmentDigestRef.GetComponent<EquipmentDigest>().SetEquipmentRef(a_equipment);
            m_equipmentDigestRef.GetComponent<EquipmentDigest>().Refresh();
        }
    }

    public void RefreshEquipmentSlots()
    {
        for (int i = 0; i < m_equipmentSlotUIRefs.Length; i++)
        {
            m_equipmentSlotUIRefs[i].SetEquipmentRef(m_gameHandlerRef.m_playerXCell.m_equippedEquipment[i]);
            m_equipmentSlotUIRefs[i].Refresh();
        }
    }

    public void SetEquipStatus(Equipment a_equipment)
    {
        m_gameHandlerRef.m_playerXCell.EquipEquipment(a_equipment, m_openedEquipmentSlotId);
        //CloseInventoryPanel();
        RefreshInventory();
        RefreshEquipmentSlots();
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
        m_inventoryPanelRef.SetActive(a_open);
        m_equipmentOverview.SetActive(!a_open);
        m_openedEquipmentSlotId = a_slotId;
        if (a_open)
        {
            RefreshInventory();
        }
    }

    public void SellEquipment(EquipmentPanel a_equipmentPanel, bool a_refreshInventory = true)
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
        if (a_refreshInventory)
        {
            RefreshInventory();
        }
    }

    public void SellAllUnequippedEquipment()
    {
        List<EquipmentPanel> equipmentPanelList = new List<EquipmentPanel>();

        for (int i = 0; i < m_equipmentItemPanels.Count; i++)
        {
            if (!m_equipmentItemPanels[i].m_equipmentRef.m_equipped)
            {
                equipmentPanelList.Add(m_equipmentItemPanels[i]);
            }
        }

        while (equipmentPanelList.Count > 0)
        {
            SellEquipment(equipmentPanelList[0],false);
            equipmentPanelList.RemoveAt(0);
        }
        RefreshInventory();
    }
}
