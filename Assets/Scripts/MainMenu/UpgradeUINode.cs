using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeUINode : MonoBehaviour
{
    internal UpgradeItem m_upgradeItemRef;
    [SerializeField] TextMeshProUGUI m_nameText;
    internal float m_availableSpace = -1f;
    UpgradeTreeUIHandler m_upgradeTreeUIHandler;
    [SerializeField] Image m_backdropRef;
    [SerializeField] Image m_iconRef;
    [SerializeField] Sprite[] m_possibleIconRefs;
    [SerializeField] Sprite m_lockIconRef;
    [SerializeField] Button m_buttonRef;

    internal void SetNameText(string a_name) { m_nameText.text = a_name;}
    internal void SetAvailableSpace(float a_space) { m_availableSpace = a_space;}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    internal void SetUp(UpgradeItem a_upgradeItem, UpgradeTreeUIHandler a_upgradeTreeUIHandler)
    {
        m_upgradeItemRef = a_upgradeItem;
        m_upgradeTreeUIHandler = a_upgradeTreeUIHandler;
        Refresh();
    }

    internal void Refresh()
    {
        m_iconRef.sprite = m_possibleIconRefs[(int)m_upgradeItemRef.m_ID];

        Color nodeColor = Color.white;
        bool interactable = true;

        if (m_upgradeItemRef.m_owned)
        {
            nodeColor = Color.yellow;
        }
        else if (!m_upgradeItemRef.m_unlocked)
        {
            m_iconRef.sprite = m_lockIconRef;
            interactable = false;
            nodeColor = Color.gray;
        }
        else if (m_upgradeItemRef.m_cost > GameHandler.m_staticAutoRef.GetCurrentCash())
        {
            nodeColor = Color.gray;
        }
        else
        {
            nodeColor = Color.white;
        }

        m_backdropRef.color = nodeColor;
        m_buttonRef.interactable = interactable;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonPressed()
    {
        m_upgradeTreeUIHandler.SelectUpgrade(this);
    }
}
