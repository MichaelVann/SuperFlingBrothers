using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SquadScreenHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;

    public GameObject m_statAllocationNotifierRef;
    public Text m_statAllocationNotifierTextRef;

    public GameObject m_equipmentAllocationNotifierRef;
    public Text m_equipmentAllocationNotifierTextRef;
    [SerializeField] TextMeshProUGUI m_titleTextRef;

    //Inventory

    //Subscreens
    [SerializeField] GameObject m_squadOverview;
    [SerializeField] GameObject m_attributesScreen;
    [SerializeField] GameObject m_skillsScreen;
    [SerializeField] GameObject m_storeScreen;
    [SerializeField] NavigationBar m_navigationBarRef;

    private bool m_inited = false;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_inited = true;
        OpenSquadOverview();
    }

    // Update is called once per frame
    void Update()
    {
        int allocationPoints = m_gameHandlerRef.m_xCellSquad.m_statHandler.m_RPGLevel.m_allocationPoints;
        m_statAllocationNotifierRef.SetActive(allocationPoints > 0);
        if (m_statAllocationNotifierRef.activeSelf)
        {
            if (allocationPoints > 99)
            {
                m_statAllocationNotifierTextRef.text = "99+";
            }
            else
            {
                m_statAllocationNotifierTextRef.text = "" + allocationPoints;
            }
        }

        int newEquipmentCount = m_gameHandlerRef.m_lastGameStats.m_equipmentCollectedLastGame;
        m_equipmentAllocationNotifierRef.SetActive(newEquipmentCount > 0);
        if (newEquipmentCount > 0)
        {
            m_equipmentAllocationNotifierTextRef.text = newEquipmentCount.ToString();
        }
    }


    void CloseAllScreens()
    {
        m_squadOverview.SetActive(false);
        m_attributesScreen.SetActive(false);
        m_skillsScreen.SetActive(false);
        m_storeScreen.SetActive(false);
    }

    public void OpenSquadOverview()
    {
        CloseAllScreens();
        m_squadOverview.SetActive(true);
        m_navigationBarRef.SetSelected(0);
        m_titleTextRef.text = "Overview";
    }

    public void OpenAttributesScreen()
    {
        CloseAllScreens();
        m_attributesScreen.SetActive(true);
        m_navigationBarRef.SetSelected(1);
        m_titleTextRef.text = "Squad Attributes";
    }

    public void OpenSkillsScreen()
    {
        CloseAllScreens();
        m_skillsScreen.SetActive(true);
        m_navigationBarRef.SetSelected(2);
        m_titleTextRef.text = "Character Skills";
    }

    public void OpenStoreScreen()
    {
        CloseAllScreens();
        m_storeScreen.SetActive(true);
        m_navigationBarRef.SetSelected(3);
        m_titleTextRef.text = "Upgrades";
    }
}
