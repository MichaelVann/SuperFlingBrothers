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


    bool m_initialised = false;

    // Start is called before the first frame update
    void Awake()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();

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
        m_buyEquipmentButtonRef.Refresh();
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
