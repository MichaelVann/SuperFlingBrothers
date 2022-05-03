using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeItemPanel : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    UpgradesScreenHandler m_upgradeScreenHandler;
    UpgradeItem m_upgradeRef;
    public int m_upgradeID;

    public Text m_nameTextRef;
    public Text m_descriptionTextRef;
    public Text m_costTextRef;
    public GameObject m_levelDisplayRef;
    public Text m_levelTextRef;
    public Image m_imageRef;
    public Button m_buyButtonRef;
    public Text m_buyButtonTextRef;
    public Image m_outlineRef;

    void Start()
    {

    }

    public void Init(int a_id, UpgradesScreenHandler a_upgradesScreenHandler)
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_upgradeScreenHandler = a_upgradesScreenHandler;

        m_upgradeID = a_id;
        m_upgradeRef = m_gameHandlerRef.m_upgrades[m_upgradeID];

        Refresh();
    }

    public void Refresh()
    {
        m_imageRef.sprite = m_upgradeScreenHandler.m_upgradeSprites[m_upgradeID];
        m_nameTextRef.text = m_upgradeRef.m_name;
        m_descriptionTextRef.text = m_upgradeRef.m_description;
        m_costTextRef.text = "" + m_upgradeRef.m_cost;
        m_levelTextRef.text = "" + m_upgradeRef.m_level;
        m_levelDisplayRef.SetActive(m_upgradeRef.m_hasLevels);

        SetBuyButtonStatus();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetBuyButtonStatus()
    {
        if (m_upgradeRef.m_owned)
        {
            m_outlineRef.color = Color.green;

            if (m_upgradeRef.m_level >= m_upgradeRef.m_maxLevel || !m_upgradeRef.m_hasLevels)
            {
                ColorBlock colorBlock = m_buyButtonRef.colors;
                colorBlock.disabledColor = Color.green;
                m_buyButtonRef.colors = colorBlock;
                m_buyButtonTextRef.text = "Owned";
            }
            else
            {
                m_buyButtonRef.gameObject.GetComponent<Image>().color = Color.blue;
                m_buyButtonTextRef.text = "Upgrade";
            }
        }

        bool interactable = (m_gameHandlerRef.GetCurrentCash() >= m_upgradeRef.m_cost);

        if (m_upgradeRef.m_hasLevels)
        {
            interactable &= (m_upgradeRef.m_level < m_upgradeRef.m_maxLevel);
        }
        else
        {
            interactable &= !m_upgradeRef.m_owned;
        }
        m_buyButtonRef.interactable = interactable;
    }

    public void AttemptToBuy()
    {
        if (m_upgradeScreenHandler.AttemptToBuyUpgrade(m_upgradeID))
        {
            Refresh();
        }
        
    }
}
