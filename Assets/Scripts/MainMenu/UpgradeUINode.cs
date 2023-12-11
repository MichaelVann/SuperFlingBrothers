using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpgradeUINode : MonoBehaviour
{
    internal UpgradeItem m_upgradeItemRef;
    [SerializeField] TextMeshProUGUI m_nameText;
    internal float m_availableSpace = -1f;
    UpgradeTreeUIHandler m_upgradeTreeUIHandler;

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
