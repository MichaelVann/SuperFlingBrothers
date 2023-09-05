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
    Town m_townA;
    Town m_townB;

    //Front
    bool m_frontActive;
    public List<BattleNode> m_battles;
    int m_battlesToSpawn = 7;
    public float m_virusBalance = 0.85f;

    public int m_frontDifficulty = 50;
    public int m_frontMinDifficulty;

    public TownConnection(Town a_townA, Town a_townB, string a_name)
    {
        m_townA = a_townA;
        m_townB = a_townB;
        m_name = a_name;
        m_battles = new List<BattleNode>();
        //m_virusBalance = 0.1f;

        m_frontMinDifficulty = 10;

        Refresh();

        if (m_frontActive)
        {
            for (int i = 0; i < m_battlesToSpawn; i++)
            {
                m_battles.Add(new BattleNode(i, m_frontMinDifficulty, m_frontDifficulty, this));
            }
        }
    }

    public void Refresh()
    {
        if (m_townA.m_overrun != m_townB.m_overrun)
        {
            m_frontActive = true;
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

    public void Refresh()
    {

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
                Town tipton = new Town("Tipton", 100f, this);
                tipton.m_overrun = true;
                m_towns.Add(tipton);
                Town teston = new Town("Teston", 100f, this);
                m_towns.Add(teston);
                m_townConnections.Add(new TownConnection(m_towns[0], m_towns[1], "TipTes"));
                Town newKhul = new Town("New Khul", 100f, this);
                //newKhul.m_overrun = true;
                m_towns.Add(newKhul);
                m_townConnections.Add(new TownConnection(m_towns[1], m_towns[2], "NK Tes"));
                Town oldKhul = new Town("Old Khul", 100f, this);
                //oldKhul.m_overrun = true;
                m_towns.Add(oldKhul);
                m_townConnections.Add(new TownConnection(m_towns[2], m_towns[3], "NK OK"));

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
    }
}
