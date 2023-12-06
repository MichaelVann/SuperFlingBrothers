﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
public class BattleNode
{
    //public enum eState
    //{
    //    locked, unlocked, beaten
    //}
    //public eState m_state;
    //public float m_invasionOwnershipPercentage = 0f;
    [SerializeField]
    public int m_maxDifficulty;
    [SerializeField]
    public int m_minDifficulty;
    [SerializeField]
    public int m_difficulty;
    [SerializeField]
    public int m_difficultyBoostTier = 0;
    [SerializeField]
    public TownConnection m_owningConnection;
    //public BattleNode[] m_connectedNodes;
    internal Vector3 m_position;
    internal bool m_positionCalculated = false;
    internal bool m_available;

    public void SetOwningConnection(TownConnection a_owningConnection) { m_owningConnection = a_owningConnection; }

    public BattleNode(int a_minDifficulty, int a_maxDifficulty, TownConnection a_owningConnection)
    {
        //m_state = a_state;
        //m_difficulty = a_difficulty;
        //m_connectedNodes = null;
        m_owningConnection = a_owningConnection;
        SetUpDifficulty(a_minDifficulty, a_maxDifficulty);
        m_available = m_difficulty <= a_owningConnection.m_humanBodyRef.m_battleMaxDifficulty;
    }

    public BattleNode(int a_difficulty, int a_boostTier, int a_minDifficulty, int a_maxDifficulty, TownConnection a_owningConnection, Vector3 a_position)
    {
        m_difficulty = a_difficulty;
        m_difficultyBoostTier = a_boostTier;
        m_minDifficulty = a_minDifficulty;
        m_maxDifficulty = a_maxDifficulty;
        m_owningConnection = a_owningConnection;
        m_position = a_position;
        m_positionCalculated = true;
    }

    internal BattleNode(SaveDataUtility.BattleNodeData a_data, TownConnection a_owningConnection)
    {
        m_difficulty = a_data.difficulty;
        m_difficultyBoostTier = a_data.difficultyBoostTier;
        m_minDifficulty = a_data.minDifficulty;
        m_maxDifficulty = a_data.maxDifficulty;
        m_owningConnection = a_owningConnection;
        m_position = a_data.position;
        m_positionCalculated = a_data.positionCalculated;
        m_available = a_data.available;
    }

    internal float GetDifficultyPercent()
    {
        float retVal = 0f;
        retVal = (((float)m_difficulty - (float)m_minDifficulty) / ((float)m_maxDifficulty - (float)m_minDifficulty));
        return retVal;
    }

    internal float GetDifficultyPercentOfMaximum()
    {
        float retVal = 0f;
        retVal = (((float)m_difficulty - (float)m_minDifficulty) / ((float)HumanBody.m_battleMaxTheoreticalDifficulty - (float)m_minDifficulty));
        return retVal;
    }

    public void SetUpDifficulty(int a_minDifficulty, int a_maxDifficulty)
    {
        m_maxDifficulty = a_maxDifficulty;
        m_minDifficulty = a_minDifficulty;
        m_difficulty = VLib.vRandom(a_minDifficulty, a_maxDifficulty);

        float roll = VLib.vRandom(0f, 1f);
        float rollCutoff = 0.1f;
        if (m_difficulty >= (int)((a_maxDifficulty - a_minDifficulty) * 0.8f + a_minDifficulty))
        {
            while (roll <= rollCutoff)
            {
                m_difficulty *= 2;
                roll = VLib.vRandom(0f, 1f);
                m_difficultyBoostTier++;
            }
        }
    }
}
public class TownConnection
{
    [SerializeField]
    public string m_name;
    [SerializeField]
    public Town m_townA;
    [SerializeField]
    public Town m_townB;
    [SerializeField]
    internal HumanBody m_humanBodyRef;

    //Front
    [SerializeField]
    public bool m_frontActive;
    [SerializeField]
    public List<BattleNode> m_battles;
    const int m_battlesToSpawn = 14;
    [SerializeField]
    public float m_warfrontBalance = 0.5f;

    public Town GetFriendlyTown() { return m_townB.m_overrun ? m_townA : m_townB; }
    public Town GetEnemyTown() { return m_townA.m_overrun ? m_townA : m_townB; }

    public static int BattleDifficultySortComparisonHighToLow(BattleNode a_nodeA, BattleNode a_nodeB) { return  a_nodeB.m_difficulty - a_nodeA.m_difficulty; }

    public TownConnection(Town a_townA, Town a_townB, string a_name)
    {
        m_townA = a_townA;
        m_townB = a_townB;
        m_humanBodyRef = m_townA.m_humanBodyRef;
        m_name = a_name;
        m_battles = new List<BattleNode>();

        Refresh();
        m_warfrontBalance = 0.9f;
    }

    public void ChangeWarfrontBalance(float a_change)
    {
        m_warfrontBalance += a_change;
        m_warfrontBalance = Mathf.Clamp(m_warfrontBalance, 0f, 1f);
        m_warfrontBalance = (float)Math.Round(m_warfrontBalance, 4);
        if (m_warfrontBalance <= 0f)
        {
            m_humanBodyRef.ExchangeFront(this, false);
        }
        else if (m_warfrontBalance >= 1f)
        {
            m_humanBodyRef.ExchangeFront(this, true);
        }
    }

    void SpawnBattles()
    {
        m_battles.Clear();
        for (int i = 0; i < m_battlesToSpawn; i++)
        {
            bool available = i <= m_battlesToSpawn / 2;
            int maxDifficulty = available ? m_humanBodyRef.m_battleMaxDifficulty : HumanBody.m_battleMaxTheoreticalDifficulty;
            int minDifficulty = available ? m_humanBodyRef.m_battleMinDifficulty : m_humanBodyRef.m_battleMaxDifficulty;

            m_battles.Add(new BattleNode(minDifficulty, maxDifficulty, this));
        }
    }

    public void Refresh()
    {
        if (m_townA.m_overrun != m_townB.m_overrun)
        {
            m_frontActive = true;
            SpawnBattles();
        }
        else
        {
            m_frontActive = false;
        }
    }
}

public class Town
{
    [SerializeField]
    public string m_name;
    [SerializeField]
    public bool m_overrun;
    [SerializeField]
    public float m_health;
    [SerializeField]
    public List<TownConnection> m_connectedWarFronts;
    public HumanBody m_humanBodyRef;

    public Town(string a_name, float a_health, HumanBody a_humanBodyRef)
    {
        m_name = a_name;
        m_health = a_health;
        m_connectedWarFronts = new List<TownConnection>();
        m_humanBodyRef = a_humanBodyRef;
    }

    public bool IsFrontLineTown()
    {
        bool retVal = false;
        for (int i = 0; i < m_connectedWarFronts.Count; i++)
        {
            if (m_connectedWarFronts[i].m_frontActive)
            {
                retVal = true;
                break;
            }
        }
        return retVal;
    }

    public void Refresh()
    {

    }

    public List<Town> FindConnectedTowns()
    {
        List<Town> connectedTowns = new List<Town>();
        for (int i = 0; i < m_connectedWarFronts.Count; i++)
        {
            TownConnection connection = m_connectedWarFronts[i];
            Town opposingTown = connection.m_townA != this ? connection.m_townA : connection.m_townB;
            connectedTowns.Add(opposingTown);
        }
        return connectedTowns;
    }

    public Town FindFallBackTown()
    {
        Town fallbackTown = null;
        List<Town> connectedTowns = FindConnectedTowns();
        for (int i = 0; i < connectedTowns.Count; i++)
        {
            if (!connectedTowns[i].m_overrun)
            {
                fallbackTown = connectedTowns[i];
                break;
            }
        }
        return fallbackTown;
    }
}
