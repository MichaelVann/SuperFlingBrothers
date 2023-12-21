using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentInventoryHandler : MonoBehaviour
{
    public SquadScreenHandler m_squadScreenHandlerRef;
    public SquadOverviewHandler m_squadOverviewHandlerRef;
    GameHandler m_gameHandlerRef;
    public EquipmentSlotUI m_inventoryEquipmentSlotUIRef;
    public Text m_equipmentAbilityReadoutText;
    public Text m_equipmentAbilityNameText;
    public GameObject m_equipmentDigestRef;

    public GameObject m_inventoryContentRef;
    public GameObject m_equipmentPanelTemplate;

    public GameObject m_noEquipmentText;

    public GameObject m_confirmationBoxPrefab;

    public GameObject m_popUpCanvasRef;

    List<EquipmentPanel> m_equipmentItemPanels;

    [SerializeField] private ScrollRect m_inventoryView;

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
        }
    }


    private void OnEnable()
    {
        if (!m_initialised)
        {
            Init();
        }
        RefreshInventory();
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
                equipmentPanel.Init(m_gameHandlerRef.m_equipmentInventory[i], this);
                //equipablePanel.Refresh();
                m_equipmentItemPanels.Add(equipmentPanel);
            }
        }
    }

    public void RefreshInventory()
    {
        SetTopPanelEquipmentRef(m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_equippedEquipment[m_squadOverviewHandlerRef.m_openedEquipmentSlotId]);
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

        //Top Panel
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
            a_equipment.m_newToPlayer = false;
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
    public void SetEquipStatus(Equipment a_equipment)
    {
        bool closingInventory = false;
        if (!a_equipment.m_equipped)
        {
            closingInventory = true;
        }
        m_squadOverviewHandlerRef.SetEquipStatus(a_equipment);

        RefreshInventory();
        
        if (closingInventory)
        {
            m_squadOverviewHandlerRef.CloseInventoryPanel();
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
            RefreshInventory();
        }
    }

    public void ConfirmSellAllItems()
    {
        ConfirmationBox confirmationBox = Instantiate(m_confirmationBoxPrefab, m_popUpCanvasRef.transform).GetComponent<ConfirmationBox>();
        confirmationBox.SetMessageText("Are you sure you want to sell all items?");
        confirmationBox.m_confirmationResponseDelegate = new ConfirmationBox.ConfirmationResponseDelegate(SellAllUnequippedEquipment);
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
        RefreshInventory();
    }
}
