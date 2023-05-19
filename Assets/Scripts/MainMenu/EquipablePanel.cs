using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipablePanel : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    EquipmentScreenHandler m_equipmentScreenHandlerRef;
    Equipable m_equipableRef;
    //public int m_upgradeID;
    //
    public Text m_nameTextRef;
    public Text m_rarityTextRef;
    public Text[] m_statTextRefs;
    public Text m_abilityTextRef;

    //public Text m_costTextRef;
    //public GameObject m_levelDisplayRef;
    public Text m_levelTextRef;
    public EquipmentPortrait m_equipmentPortrait;
    public Button m_equipButtonRef;
    public Text m_equipButtonTextRef;
    //public Image m_outlineRef;


    void Awake()
    {
        m_equipmentScreenHandlerRef = FindObjectOfType<EquipmentScreenHandler>();
    }

    public void Init(Equipable a_equipable)
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_equipableRef = a_equipable;
    }

    public void Refresh()
    {
        //m_imageRef.sprite = m_upgradeScreenHandler.m_upgradeSprites[m_upgradeID];
        m_nameTextRef.text = m_equipableRef.m_name;
        m_rarityTextRef.text = m_equipableRef.m_rarityTier.name;
        m_rarityTextRef.color = m_equipableRef.m_rarityTier.color;
        m_nameTextRef.color = m_equipableRef.m_rarityTier.color;

        for (int i = 0; i < m_statTextRefs.Length; i++)
        {
            m_statTextRefs[i].text = "";
        }

        for (int i = 0; i < m_equipableRef.m_stats.Count; i++)
        {
            m_statTextRefs[i].text = CharacterStatHandler.GetStatName(m_equipableRef.m_stats[i].statType) + ": " + m_equipableRef.m_stats[i].value;
        }
        
        //m_costTextRef.text = "" + m_upgradeRef.m_cost;
        m_levelTextRef.text = "" + m_equipableRef.m_level;

        SetEquipButtonStatus();
        m_equipmentPortrait.SetEquipableRef(m_equipableRef);
    }

    //    // Update is called once per frame
    //    void Update()
    //    {

    //    }

    void SetEquipButtonStatus()
    {
        Color equipButtonColor = Color.white;
        string equipButtonString = "";
        bool equipped = m_equipableRef.m_equipped;
        bool sameSlot = m_equipmentScreenHandlerRef.m_openedEquipableSlotId == m_equipableRef.m_equippedSlotId;
        if (!equipped)
        {
            equipButtonColor = Color.white;
            equipButtonString = "Equip";
        }
        else if (equipped && sameSlot)
        {
            equipButtonColor = Color.yellow;
            equipButtonString = "UnEquip";
        }
        else if (equipped && !sameSlot)
        {
            equipButtonColor = Color.red;
            equipButtonString = "ReEquip";
        }
        m_equipButtonRef.gameObject.GetComponent<Image>().color = equipButtonColor;
        m_equipButtonTextRef.text = equipButtonString;
    }

    public void AttemptToEquip()
    {
        m_equipmentScreenHandlerRef.SetEquipStatus(m_equipableRef);
        //Refresh();
    }
}
