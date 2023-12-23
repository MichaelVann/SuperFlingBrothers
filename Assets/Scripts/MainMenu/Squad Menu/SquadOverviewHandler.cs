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

    public GameObject m_newEquipmentNotifier;
    public Text m_newEquipmentNotifierText;

    public GameObject m_inventoryPanelRef;

    [SerializeField] ArmorSegment[] m_armorSegmentsRef;
    [SerializeField] GameObject[] m_armorSegmentSlotRefs;

    [SerializeField] GameObject m_confirmationBoxPrefab;

    bool m_inited = false;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        RefreshEquipmentSlots();
        m_xCellNameText.text = "ID: " + m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_name.ToUpper();
        m_playerStatueRef.GetComponent<Image>().color = m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_colorShade;
        m_inited = true;
        RefreshNewEquipmentNotifiers();
    }

    private void OnEnable()
    {
        if (m_gameHandlerRef == null)
        {
            m_gameHandlerRef = FindObjectOfType<GameHandler>();
        }
        if (m_inited)
        {
            RefreshEquipmentSlots();
            RefreshNewEquipmentNotifiers();
        }
        if (m_gameHandlerRef != null && m_gameHandlerRef.m_squadRenameNotificationPending)
        {
            m_gameHandlerRef.m_squadRenameNotificationPending = false;
            ConfirmationBox confirmationBox = Instantiate(m_confirmationBoxPrefab, transform).GetComponent<ConfirmationBox>();
            confirmationBox.SetMessageText("Congratulations, due to your success your team has begun to be refered to as <color=red>" + m_gameHandlerRef.m_xCellSquad.m_name + "</color>.");
            confirmationBox.SetToAcknowledgeOnlyMode();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RefreshNewEquipmentNotifiers()
    {
        int newEquipmentCount = m_gameHandlerRef.m_lastGameStats.m_equipmentCollectedLastGame;
        m_newEquipmentNotifier.SetActive(newEquipmentCount > 0);
        m_newEquipmentNotifierText.text = newEquipmentCount.ToString();
    }


    public void SetInventoryPanelStatus(bool a_open)
    {
        m_inventoryPanelRef.SetActive(a_open);
        this.gameObject.SetActive(!a_open);
    }

    public void RefreshEquipmentSlots()
    {
        for (int i = 0; i < m_armorSegmentsRef.Length; i++)
        {
            Equipment equipment = m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_equippedEquipment[i];
            if (equipment != null)
            {
                m_armorSegmentsRef[i].AssignEquipment(equipment);
                m_armorSegmentsRef[i].gameObject.SetActive(true);
                m_armorSegmentSlotRefs[i].gameObject.SetActive(false);
            }
            else
            {
                m_armorSegmentsRef[i].gameObject.SetActive(false);
                m_armorSegmentSlotRefs[i].gameObject.SetActive(true);
            }
        }
    }

    public void OpenEquipmentInventory()
    {
        SetInventoryPanelStatus(true);
    }

    public void CloseInventoryPanel()
    {
        RefreshEquipmentSlots();
        SetInventoryPanelStatus(false);
    }
}
