using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesScreenHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;

    public Sprite[] m_upgradeSprites;

    public GameObject m_contentRef;
    public GameObject m_upgradeItemPanelTemplate;

    public BuyEquipmentButton m_buyEquipmentButtonRef;

    List<UpgradeItemPanel> m_upgradeItemPanels;

    bool m_initialised = false;

    // Start is called before the first frame update
    void Awake()
    {
        m_upgradeItemPanels = new List<UpgradeItemPanel>();
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        for (int i = 0; i < m_gameHandlerRef.m_upgrades.Length; i++)
        {
            UpgradeItemPanel upgradeItemPanel = Instantiate<GameObject>(m_upgradeItemPanelTemplate, m_contentRef.transform).GetComponent<UpgradeItemPanel>();
            upgradeItemPanel.Init(i, this);
            m_upgradeItemPanels.Add(upgradeItemPanel);
        }
    }

    void Start()
    {
        Refresh();
        m_initialised = true;
    }

    private void OnEnable()
    {
        if (m_initialised)
        {
            Refresh();
        }
    }

    public void Refresh()
    {
        for (int i = 0; i < m_upgradeItemPanels.Count; i++)
        {
            m_upgradeItemPanels[i].Refresh();
        }
        m_buyEquipmentButtonRef.Refresh();
    }

    public bool AttemptToBuyUpgrade(int a_id)
    {
        if (m_gameHandlerRef.AttemptToBuyUpgrade(a_id))
        {
            Refresh();
            return true;
        }
        return false;
    }

    public void AttemptToBuyEquipment()
    {
        if (m_gameHandlerRef.AttemptToBuyJunkEquipment())
        {
            Refresh();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
