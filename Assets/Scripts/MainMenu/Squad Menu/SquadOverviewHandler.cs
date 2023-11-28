using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SquadOverviewHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public GameObject m_playerStatueRef;
    public TextMeshProUGUI m_xCellNameText;
    public int m_openedEquipmentSlotId = -1;

    public GameObject m_inventoryPanelRef;

    public EquipmentSlotUI[] m_equipmentSlotUIRefs;

    bool m_inited = false;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        RefreshEquipmentSlots();
        m_xCellNameText.text = "ID: " + m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_name.ToUpper();
        m_playerStatueRef.GetComponent<Image>().color = m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_colorShade;
        m_inited = true;
    }

    private void OnEnable()
    {
        if (m_inited)
        {
            RefreshEquipmentSlots();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetInventoryPanelStatus(bool a_open, int a_slotId = -1)
    {
        m_openedEquipmentSlotId = a_slotId;
        m_inventoryPanelRef.SetActive(a_open);
        this.gameObject.SetActive(!a_open);
    }

    internal void SetEquipStatus(Equipment a_equipment)
    {
        m_gameHandlerRef.m_xCellSquad.m_playerXCell.EquipEquipment(a_equipment, m_openedEquipmentSlotId);
        RefreshEquipmentSlots();
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
}
