﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBattleNode : MonoBehaviour
{
    // Start is called before the first frame update

    GameHandler m_gameHandlerRef;
    public List<GameObject> m_connectionList;
    //BodyPartSelectionHandler m_bodySelectionHandlerRef;
    internal BattleNode m_battleNodeRef;
    MapNodeConnection m_owningConnection;

    public int m_invaders = 0;
    public int m_id = 0;
    public int m_parentBodyPartID;
    public int m_difficulty = 5;
    public int m_difficultyBoostTier = 0;

    public GameObject m_selectionRingRef;
    public GameObject m_lockIconRef;

    internal bool m_enabled;

    public void SetSelectionRingActive(bool a_active) { m_selectionRingRef.SetActive(a_active); }


    void Start()
    {
        m_selectionRingRef.SetActive(false);
    }

    public void SetUp(MapNodeConnection a_connection, BattleNode a_battleNodeRef)
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_owningConnection = a_connection;
        m_battleNodeRef = a_battleNodeRef;
        m_difficulty = m_battleNodeRef.m_difficulty;
        m_difficultyBoostTier = m_battleNodeRef.m_difficultyBoostTier;
        m_enabled = m_battleNodeRef.m_available;

        if (m_enabled)
        {
            m_gameHandlerRef.m_humanBody.m_availableBattles++;
        }

        m_lockIconRef.SetActive(!m_enabled);
        SetNodeDifficultyColor();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Select()
    {
        m_owningConnection.SelectBattleNode(this);
    }

    internal void SetNodeDifficultyColor()
    {
        float difficultyPercentage = m_battleNodeRef.GetDifficultyPercentOfMaximum();
        Color nodeColor = VLib.RatioToColorRarity(difficultyPercentage);

        if (!m_enabled)
        {
            nodeColor *= 0.5f;
        }
        GetComponent<SpriteRenderer>().color = nodeColor;
    }


    void OnMouseUpAsButton()
    {
        //Select();
    }

    public void NodePressed()
    {
        if (m_enabled)
        {
            Select();
        }
    }

}
