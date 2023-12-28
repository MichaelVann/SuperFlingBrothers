using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentInventoryHandler : MonoBehaviour
{
    public SquadScreenHandler m_squadScreenHandlerRef;
    public SquadOverviewHandler m_squadOverviewHandlerRef;
    GameHandler m_gameHandlerRef;

    //Top panel ref
    public EquipmentSlotUI[] m_inventoryEquipmentSlotUIRefs;
    [SerializeField] Button m_inspectEquipmentSlotButtonRef;
    [SerializeField] Button m_unequipEquipmentSlotButtonRef;


    public GameObject m_equipmentDigestRef;

    public GameObject m_inventoryContentRef;
    public GameObject m_equipmentPanelTemplate;

    public GameObject m_noEquipmentText;

    public GameObject m_confirmationBoxPrefab;

    public GameObject m_popUpCanvasRef;

    List<EquipmentPanel> m_equipmentItemPanels;

    [SerializeField] private ScrollRect m_inventoryView;
    internal int m_openedEquipmentSlotId = 0;

    bool m_initialised = false;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Init()
    {
        if (!m_initialised)
        {
            m_equipmentItemPanels = new List<EquipmentPanel>();
            m_gameHandlerRef = FindObjectOfType<GameHandler>();
            InstantiateEquipmentInventory();
            m_initialised = true;
            SelectFirstEmptyEquipmentSlot();
        }
    }

    private void OnEnable()
    {
        if (!m_initialised)
        {
            Init();
        }
        SelectFirstEmptyEquipmentSlot();
        Refresh();
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
            if (m_gameHandlerRef.m_equipmentInventory[i] != null && !m_gameHandlerRef.m_equipmentInventory[i].m_equipped)
            {
                EquipmentPanel equipmentPanel = Instantiate<GameObject>(m_equipmentPanelTemplate, m_inventoryContentRef.transform).GetComponent<EquipmentPanel>();
                equipmentPanel.Init(m_gameHandlerRef.m_equipmentInventory[i], this);
                //equipablePanel.Refresh();
                m_equipmentItemPanels.Add(equipmentPanel);
            }
        }
    }

    public void UnEquipEquipmentSlot()
    {
        SetEquipStatus(m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_equippedEquipment[m_openedEquipmentSlotId]);
    }

    public void InspectEquipmentSlot()
    {
        SetEquipmentDigestStatus(true, m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_equippedEquipment[m_openedEquipmentSlotId]);
    }

    public void Refresh()
    {
        RefreshTopPanel();
        m_gameHandlerRef.SortEquipmentInventory();
        InstantiateEquipmentInventory();
        m_gameHandlerRef.m_lastGameStats.m_equipmentCollectedLastGame = 0;
        m_noEquipmentText.SetActive(m_equipmentItemPanels.Count < 1);
        for (int i = 0; i < m_equipmentItemPanels.Count; i++)
        {
            m_equipmentItemPanels[i].Refresh();
        }

        //Reset scroll bar to top
        m_inventoryView.verticalNormalizedPosition = 1;


        GameHandler.AutoSaveCheck();
    }

    void SelectFirstEmptyEquipmentSlot()
    {
        for (int i = 0; i < m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_equippedEquipment.Length; i++)
        {
            if (m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_equippedEquipment[i] == null)
            {
                m_openedEquipmentSlotId = i;
                break;
            }
        }
    }

    void RefreshTopPanel()
    {
        SetTopPanelEquipmentRefs();
        Equipment selectedEquipment = m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_equippedEquipment[m_openedEquipmentSlotId];

        for (int i = 0; i < m_equipmentItemPanels.Count; i++)
        {
            m_equipmentItemPanels[i].SetSelected(m_equipmentItemPanels[i].m_equipmentRef == selectedEquipment);
        }
        for (int i = 0; i < m_inventoryEquipmentSlotUIRefs.Length; i++)
        {
            m_inventoryEquipmentSlotUIRefs[i].m_selectedOutline.SetActive(i == m_openedEquipmentSlotId);
        }
        m_inspectEquipmentSlotButtonRef.interactable = m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_equippedEquipment[m_openedEquipmentSlotId] != null;
        m_unequipEquipmentSlotButtonRef.interactable = m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_equippedEquipment[m_openedEquipmentSlotId] != null;
    }

    public void SelectEquipmentSlot(int a_selectedSlot)
    {
        m_openedEquipmentSlotId = a_selectedSlot;
        RefreshTopPanel();
    }

    internal void SetTopPanelEquipmentRefs()
    {
        for (int i = 0; i < m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_equippedEquipment.Length; i++)
        {
            m_inventoryEquipmentSlotUIRefs[i].Init(i,m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_equippedEquipment[i]);
            m_inventoryEquipmentSlotUIRefs[i].Refresh();
        }
    }
    public void SetEquipStatus(Equipment a_equipment)
    {
        bool closingInventory = false;
        if (!a_equipment.m_equipped)
        {
            closingInventory = true;
        }
        m_gameHandlerRef.m_xCellSquad.m_playerXCell.EquipEquipment(a_equipment, m_openedEquipmentSlotId);
        SelectFirstEmptyEquipmentSlot();
        Refresh();
        
        if (closingInventory)
        {
            //m_squadOverviewHandlerRef.CloseInventoryPanel();
        }
    }

    internal void SetEquipmentDigestStatus(bool a_open, Equipment a_equipment = null)
    {
        //SetTopPanelEquipmentRef(a_equipment);
        m_equipmentDigestRef.SetActive(a_open);

        if (a_open)
        {
            m_equipmentDigestRef.GetComponent<EquipmentDigest>().SetEquipmentRef(a_equipment);
            m_equipmentDigestRef.GetComponent<EquipmentDigest>().Refresh();
            a_equipment.m_newToPlayer = false;
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
            Refresh();
        }
    }

    public void ConfirmSellAllItems()
    {
        ConfirmationBox confirmationBox = Instantiate(m_confirmationBoxPrefab, m_popUpCanvasRef.transform).GetComponent<ConfirmationBox>();
        confirmationBox.SetMessageText("Are you sure you want to sell all items?");
        confirmationBox.SetConfirmationResponseDelegate(new ConfirmationBox.ConfirmationResponseDelegate(SellAllUnequippedEquipment));
    }

    void SellAllUnequippedEquipment()
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
            SellEquipment(equipmentPanelList[0], false);
            equipmentPanelList.RemoveAt(0);
        }
        Refresh();
    }
}
