﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoReadout : MonoBehaviour
{
    public Text[] m_statTexts;
    GameHandler m_gameHandlerRef;
    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
    }

    public void Refresh()
    {
        for (int i = 0; i < m_statTexts.Length; i++)
        {
            m_statTexts[i].text = "";
        }

        for (int i = 0; i < m_statTexts.Length && i < (int)eCharacterStatIndices.count; i++)
        {
            m_statTexts[i].text = m_gameHandlerRef.m_playerStatHandler.m_stats[i].name + ": " + m_gameHandlerRef.m_playerStatHandler.m_stats[i].finalValue;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Refresh();
    }
}
