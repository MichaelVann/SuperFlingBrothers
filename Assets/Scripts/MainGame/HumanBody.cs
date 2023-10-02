using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBody
{
    public GameHandler m_gameHandlerRef;

    public string m_firstName;
    public string m_lastName;
    public Town m_playerResidingTown;
    //float m_basePartHealth = 100;
    public int m_maxEnemyDifficulty = 12;

    public int m_battleMaxDifficulty;
    public int m_battleMinDifficulty;
    public int m_startingBattleMaxDifficulty = 20;
    public int m_startingBattleMinDifficulty = 1;

    public bool m_bodyInitialised = false;

    public List<Town> m_towns;
    public List<TownConnection> m_townConnections;

    public string m_name;
    public bool m_unlocked;

    internal int m_battlesCompleted = 0;

    //public int m_invaderStrength;

    internal bool m_lost = false;

    internal string GetHumansName() {return m_firstName + " " + m_lastName; }

    public HumanBody(GameHandler a_gameHandlerRef)
    {
        m_gameHandlerRef = a_gameHandlerRef;
        string[] names = VLib.GenerateRandomPersonsName();
        m_firstName = names[0];
        m_lastName = names[1];
        UpdateMaxAndMindDifficulties();
        SetupTowns();
    }

    void SetupTowns()
    {
        //m_health = a_health;
        m_towns = new List<Town>();
        m_townConnections = new List<TownConnection>();

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


        m_playerResidingTown = teston1;
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

    internal void Refresh()
    {
        UpdateMaxAndMindDifficulties();
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

        if (a_connection.m_townA == m_playerResidingTown)
        {
            playersTown = a_connection.m_townA;
        }
        else if (a_connection.m_townB == m_playerResidingTown)
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
        if (m_playerResidingTown.m_overrun)
        {
            Town fallbackTown = m_playerResidingTown.FindFallBackTown();
            if (fallbackTown != null)
            {
                m_playerResidingTown = fallbackTown;
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
            m_playerResidingTown = m_suitableTowns[VLib.vRandom(0, m_suitableTowns.Count - 1)].GetFriendlyTown();
        }
        else if (m_playerResidingTown.m_overrun)
        {
            m_lost = true;
        }
    }

    public void UpdateMaxAndMindDifficulties()
    {
        //int playerLevel = m_gameHandlerRef.m_xCellTeam.level;
        int playerLevel = m_gameHandlerRef.m_xCellTeam.m_playerXCell.m_statHandler.m_RPGLevel.m_level;
        m_battleMaxDifficulty = m_startingBattleMaxDifficulty + playerLevel * 2;
        m_battleMinDifficulty = m_startingBattleMinDifficulty + playerLevel;
    }
}
