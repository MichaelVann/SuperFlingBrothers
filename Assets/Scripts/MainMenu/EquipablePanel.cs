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
    public Text[] m_statTextRefs;

    public Text m_abilityTextRef;

    //public Text m_costTextRef;
    //public GameObject m_levelDisplayRef;
    public Text m_levelTextRef;
    public Image m_imageRef;
    public Button m_equipButtonRef;
    public Text m_equipButtonTextRef;
    //public Image m_outlineRef;


    void Start()
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

        for (int i = 0; i < m_statTextRefs.Length; i++)
        {
            m_statTextRefs[i].text = "";
        }

        for (int i = 0; i < m_equipableRef.m_stats.Count; i++)
        {
            m_statTextRefs[i].text = m_equipableRef.m_stats[i].statType.ToString() + ": " + m_equipableRef.m_stats[i].value;
        }
        
        //m_costTextRef.text = "" + m_upgradeRef.m_cost;
        m_levelTextRef.text = "" + m_equipableRef.m_level;

        SetEquipButtonStatus();
    }

    //    // Update is called once per frame
    //    void Update()
    //    {

    //    }

    void SetEquipButtonStatus()
    {
        if (m_equipableRef.m_equipped)
        {
            m_equipButtonRef.gameObject.GetComponent<Image>().color = Color.yellow;
            m_equipButtonTextRef.text = "UnEquip";
        }
    }

    public void AttemptToEquip()
    {
        if (m_equipmentScreenHandlerRef.AttemptToEquip(m_equipableRef))
        {
            Refresh();
        }
        Refresh();
    }
}
