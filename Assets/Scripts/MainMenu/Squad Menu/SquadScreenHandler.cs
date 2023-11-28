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

    //Inventory

    //Subscreens
    public GameObject m_squadOverview;
    public GameObject m_attributesScreen;
    public GameObject m_skillsScreen;
    public GameObject m_storeScreen;

    private bool m_inited = false;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_inited = true;
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
    }

    public void OpenAttributesScreen()
    {
        CloseAllScreens();
        m_attributesScreen.SetActive(true);
    }

    public void OpenSkillsScreen()
    {
        CloseAllScreens();
        m_skillsScreen.SetActive(true);
    }

    public void OpenStoreScreen()
    {
        CloseAllScreens();
        m_storeScreen.SetActive(true);
    }
}
