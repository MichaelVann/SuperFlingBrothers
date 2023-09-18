using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class BattleNode
{
    public int m_id;
    //public enum eState
    //{
    //    locked, unlocked, beaten
    //}
    //public eState m_state;
    //public float m_invasionOwnershipPercentage = 0f;
    public int m_maxDifficulty;
    public int m_minDifficulty;
    public int m_difficulty;
    public int m_difficultyBoostTier = 0;
    public TownConnection m_owningConnection;
    //public BattleNode[] m_connectedNodes;

    public BattleNode(int a_id, int a_minDifficulty, int a_maxDifficulty, TownConnection a_owningConnection)
    {
        m_id = a_id;
        //m_state = a_state;
        //m_difficulty = a_difficulty;
        //m_connectedNodes = null;
        m_owningConnection = a_owningConnection;
        SetUpDifficulty(a_minDifficulty, a_maxDifficulty);
    }

    public float GetDifficultyPercent()
    {
        float retVal = 0f;
        retVal = (((float)m_difficulty - (float)m_minDifficulty) / ((float)m_maxDifficulty - (float)m_minDifficulty));
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
    public string m_name;
    public Town m_townA;
    public Town m_townB;

    //Front
    public bool m_frontActive;
    public List<BattleNode> m_battles;
    int m_battlesToSpawn = 7;
    public float m_warfrontBalance = 0.5f;

    public int m_battleMaxDifficulty = 60;
    public int m_battleMinDifficulty = 1;

    public Town GetFriendlyTown() { return m_townB.m_overrun ? m_townA : m_townB; }
    public Town GetEnemyTown() { return m_townA.m_overrun ? m_townA : m_townB; }

    public TownConnection(Town a_townA, Town a_townB, string a_name)
    {
        m_townA = a_townA;
        m_townB = a_townB;
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
            m_townA.m_owningBodyPartRef.ExchangeFront(this, false);
        }
        else if (m_warfrontBalance >= 1f)
        {
            m_townA.m_owningBodyPartRef.ExchangeFront(this, true);
        }
    }

    void SpawnBattles()
    {
        m_battles.Clear();
        for (int i = 0; i < m_battlesToSpawn; i++)
        {
            m_battles.Add(new BattleNode(i, m_battleMinDifficulty, m_battleMaxDifficulty, this));
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
    public string m_name;
    public bool m_overrun;
    public float m_health;
    public List<TownConnection> m_connectedWarFronts;
    public BodyPart m_owningBodyPartRef;

    public Town(string a_name, float a_health, BodyPart a_bodyPart)
    {
        m_name = a_name;
        m_health = a_health;
        m_connectedWarFronts = new List<TownConnection>();
        m_owningBodyPartRef = a_bodyPart;
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

public class BodyPart
{
    //public BattleNode[] m_nodes;

    public List<Town> m_towns;
    public List<TownConnection> m_townConnections;
    public HumanBody m_owningHumanBodyRef;

    public string m_name;
    public bool m_unlocked;
    internal int m_maxEnemyDifficulty;

    public int m_invaderStrength;

    bool m_lost = false;

    public enum eType
    {
        Chest,
        Shoulder,
        ForeArm,
        Hand,

        Count
    }

    public eType m_type;
    public static string GetPartName(BodyPart.eType a_type) { return Enum.GetName(typeof(BodyPart.eType), a_type); }

    public BodyPart(BodyPart.eType a_type, int a_invaderStrength, bool a_unlocked, HumanBody a_humanBody)
    {
        m_name = GetPartName(a_type);
        m_type = a_type;
        //m_health = a_health;
        m_invaderStrength = a_invaderStrength;
        m_unlocked = a_unlocked;
        m_towns = new List<Town>();
        m_townConnections = new List<TownConnection>();
        m_owningHumanBodyRef = a_humanBody;

        switch (m_type)
        {
            case eType.Chest:
                break;
            case eType.Shoulder:
                break;
            case eType.ForeArm:
                break;
            case eType.Hand:
                Town tipton0 = new Town("Tipton", 100f, this);
                tipton0.m_overrun = true;
                m_towns.Add(tipton0);

                Town teston1 = new Town("Teston", 100f, this);
                m_towns.Add(teston1);
                m_townConnections.Add(new TownConnection(m_towns[0], m_towns[1], "TipTes"));

                Town newKhul2 = new Town("New Khul", 100f, this);
                m_towns.Add(newKhul2);
                m_townConnections.Add(new TownConnection(m_towns[1], m_towns[2], "NK Tes"));

                Town oldKhul3 = new Town("Old Khul", 100f, this);
                m_towns.Add(oldKhul3);
                m_townConnections.Add(new TownConnection(m_towns[2], m_towns[3], "NK OK"));
                m_townConnections.Add(new TownConnection(m_towns[1], m_towns[3], "Tes OK"));

                Town palmSprings4 = new Town("Palm Springs", 100f, this);
                m_towns.Add(palmSprings4);
                m_townConnections.Add(new TownConnection(m_towns[4], m_towns[2], "PS NK"));
                m_townConnections.Add(new TownConnection(m_towns[4], m_towns[3], "PS OK"));

                Town wriston5 = new Town("Wriston", 100f, this);
                m_towns.Add(wriston5);
                m_townConnections.Add(new TownConnection(m_towns[5], m_towns[4], "Wris PS"));

                Town tendonsKeep6 = new Town("Tendon's Keep", 100f, this);
                m_towns.Add(tendonsKeep6);
                m_townConnections.Add(new TownConnection(m_towns[6], m_towns[5], "TK Wris"));

                Town phalangeCentral7 = new Town("Phalange Central", 100f, this);
                m_towns.Add(phalangeCentral7);
                m_townConnections.Add(new TownConnection(m_towns[7], m_towns[3], "PC OK"));

                Town nailCoast8 = new Town("Nail Coast", 100f, this);
                m_towns.Add(nailCoast8);
                m_townConnections.Add(new TownConnection(m_towns[8], m_towns[7], "PC NC"));

                Town aresEye9 = new Town("Ares Eye", 100f, this);
                m_towns.Add(aresEye9);
                m_townConnections.Add(new TownConnection(m_towns[9], m_towns[4], "Ares PS"));
                m_townConnections.Add(new TownConnection(aresEye9, wriston5, "Ares Wris"));

                Town pinkerton10 = new Town("Pinkerton", 100f, this);
                m_towns.Add(pinkerton10);
                m_townConnections.Add(new TownConnection(m_towns[10], m_towns[2], "Pink NK"));

                Town indexPoint11 = new Town("Index Point", 100f, this);
                m_towns.Add(indexPoint11);
                m_townConnections.Add(new TownConnection(indexPoint11, oldKhul3, "IP OK"));
                //m_townConnections.Add(new TownConnection(m_towns[11], m_towns[3], "IP OK"));

                Town archersNook12 = new Town("Archer's Nook", 100f, this);
                m_towns.Add(archersNook12);
                m_townConnections.Add(new TownConnection(archersNook12, palmSprings4, "PS AN"));
                //m_townConnections.Add(new TownConnection(m_towns[11], m_towns[3], "IP OK"));

                Town thoom13 = new Town("Thoom", 100f, this);
                m_towns.Add(thoom13);
                m_townConnections.Add(new TownConnection(thoom13, archersNook12, "AN Tho"));

                break;
            case eType.Count:
                break;
            default:
                break;
        }
    }
    public Town FindTownByName(string a_townName)
    {
        Town foundTown = null;
        for (int i = 0; i < m_towns.Count; i++)
        {
            if (m_towns[i].m_name == a_townName)
            {
                foundTown = m_towns[i];
                break;
            }
        }
        return foundTown;
    }

    internal TownConnection FindConnection(string a_name)
    {
        TownConnection foundConnection = null;
        for (int i = 0; i < m_townConnections.Count; i++)
        {
            if (a_name == m_townConnections[i].m_name)
            {
                foundConnection = m_townConnections[i];
            }
        }
        return foundConnection;
    }
    public void Refresh()
    {
        for (int i = 0; i < m_towns.Count; i++)
        {
            m_towns[i].Refresh();
        }

        for (int i = 0; i < m_townConnections.Count; i++)
        {
            m_townConnections[i].Refresh();
        }
        TaskPlayerToResidingTown();
    }

    public void ExchangeFront(TownConnection a_connection, bool a_won)
    {


        //m_owningHumanBodyRef.m_playerResidingTown = m_suitableTowns[VLib.vRandom(0, m_suitableTowns.Count - 1)].GetFriendlyTown();


        a_connection.m_townA.m_overrun = !a_won;
        a_connection.m_townB.m_overrun = !a_won;

        Town playersTown = null;

        if (a_connection.m_townA == m_owningHumanBodyRef.m_playerResidingTown)
        {
            playersTown = a_connection.m_townA;
        }
        else if (a_connection.m_townB == m_owningHumanBodyRef.m_playerResidingTown)
        {
            playersTown = a_connection.m_townB;
        }

        if (playersTown != null)
        {
            if (!playersTown.IsFrontLineTown() || playersTown.m_overrun)
            {
                TaskPlayerToResidingTown();
            }
        }

        Refresh();

    }

    public void TaskPlayerToResidingTown()
    {
        if (m_owningHumanBodyRef.m_playerResidingTown.m_overrun)
        {
            Town fallbackTown = m_owningHumanBodyRef.m_playerResidingTown.FindFallBackTown();
            if (fallbackTown != null)
            {
                m_owningHumanBodyRef.m_playerResidingTown = fallbackTown;
                return;
            }
        }

        List<TownConnection> m_suitableTowns = new List<TownConnection>();
        for (int i = 0; i < m_townConnections.Count; i++)
        {
            if (m_townConnections[i].m_frontActive)
            {
                m_suitableTowns.Add(m_townConnections[i]);
            }
        }

        if (m_suitableTowns.Count > 0)
        {
            m_owningHumanBodyRef.m_playerResidingTown = m_suitableTowns[VLib.vRandom(0, m_suitableTowns.Count - 1)].GetFriendlyTown();
        }
        else if (m_owningHumanBodyRef.m_playerResidingTown.m_overrun)
        {
            m_lost = true;
        }
    }
}
