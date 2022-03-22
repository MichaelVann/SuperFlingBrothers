using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesScreenHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public Text m_cashCounterTextRef;

    public Sprite[] m_upgradeSprites;

    public GameObject m_contentRef;
    public GameObject m_upgradeItemPanelTemplate;

    List<UpgradeItemPanel> m_upgradeItemPanels;

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

    public void Refresh()
    {
        m_cashCounterTextRef.text = "" + VLib.TruncateFloatsDecimalPlaces(m_gameHandlerRef.GetCurrentCash(),2);
        for (int i = 0; i < m_upgradeItemPanels.Count; i++)
        {
            m_upgradeItemPanels[i].Refresh();
        }
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
