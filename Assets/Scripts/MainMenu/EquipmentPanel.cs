using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentPanel : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    EquipmentScreenHandler m_equipmentScreenHandlerRef;
    Equipment m_equipmentRef;
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

    public void Init(Equipment a_equipment)
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_equipmentRef = a_equipment;
    }

    public void Refresh()
    {
        //m_imageRef.sprite = m_upgradeScreenHandler.m_upgradeSprites[m_upgradeID];
        m_nameTextRef.text = m_equipmentRef.m_name;
        m_rarityTextRef.text = m_equipmentRef.m_rarityTier.name;
        m_rarityTextRef.color = m_equipmentRef.m_rarityTier.color;
        m_nameTextRef.color = m_equipmentRef.m_rarityTier.color;

        for (int i = 0; i < m_statTextRefs.Length; i++)
        {
            m_statTextRefs[i].text = "";
        }

        for (int i = 0; i < m_equipmentRef.m_stats.Count; i++)
        {
            m_statTextRefs[i].text = CharacterStatHandler.GetStatName(m_equipmentRef.m_stats[i].statType) + ": " + m_equipmentRef.m_stats[i].value;
            Color textColor = new Color();
            switch (m_equipmentRef.m_stats[i].statType)
            {
                case eCharacterStatIndices.strength:
                    textColor = Color.yellow;
                    break;
                case eCharacterStatIndices.dexterity:
                    textColor = Color.green;
                    break;
                case eCharacterStatIndices.constitution:
                    textColor = Color.red;
                    break;
                case eCharacterStatIndices.protection:
                    textColor = Color.gray;
                    break;
                case eCharacterStatIndices.count:
                    break;
                default:
                    break;
            }
            m_statTextRefs[i].color = textColor;

        }

        //m_costTextRef.text = "" + m_upgradeRef.m_cost;
        m_levelTextRef.text = "" + m_equipmentRef.m_level;

        SetEquipButtonStatus();
        m_equipmentPortrait.SetEquipmentRef(m_equipmentRef);
    }

    //    // Update is called once per frame
    //    void Update()
    //    {

    //    }

    void SetEquipButtonStatus()
    {
        Color equipButtonColor = Color.white;
        string equipButtonString = "";
        bool equipped = m_equipmentRef.m_equipped;
        bool sameSlot = m_equipmentScreenHandlerRef.m_openedEquipmentSlotId == m_equipmentRef.m_equippedSlotId;
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
        m_equipmentScreenHandlerRef.SetEquipStatus(m_equipmentRef);
        //Refresh();
    }
}
