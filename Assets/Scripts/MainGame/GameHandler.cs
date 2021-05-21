using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    //UIHandler m_uiHandlerRef;

    public float m_score = 0f;
    public int m_XP = 0;
    public int m_maxXP = 83/4;
    public int m_playerLevel = 1;

    public enum eGameMode
    {
        TurnLimit,
        Health,
        Pockets,
        Hunger,
        ModeCount
    }
    public eGameMode m_currentGameMode;

    public enum eStatIndices
    {
        flingStrength,
        health,
        maxHealth,
        minHealth,
        strength,
        count
    }

    public struct Stat
    {
        public string name;
        public float value;
        public float originalCost;
        public float cost;
        public float costIncreaseRate;
    }

    public Stat[] m_playerStats;


    public void SetDefaultStats(ref Stat[] a_stats)
    {

        a_stats[(int)eStatIndices.flingStrength].value = 259f;
        a_stats[(int)eStatIndices.maxHealth].value = 4f;
        a_stats[(int)eStatIndices.health].value = a_stats[(int)eStatIndices.maxHealth].value;
        a_stats[(int)eStatIndices.strength].value = 1f;

        for (int i = 0; i < (int)eStatIndices.count; i++)
        {
            a_stats[i].originalCost = 10f;
            a_stats[i].cost = a_stats[i].originalCost;
            a_stats[i].costIncreaseRate = 1.8f;
        }
    }

    public void ChangeScore(float a_change) { m_score += a_change; }
    public void SetGameMode(eGameMode a_gameMode) { m_currentGameMode = a_gameMode; }

    public void ChangeStatValue(eStatIndices a_index, float a_change)
    {
        m_playerStats[(int)a_index].value += a_change;
    }

    public void AttemptToIncreaseStat(eStatIndices a_index)
    {
        if (m_XP >= (int)m_playerStats[(int)a_index].cost)
        {
            ChangeStatValue(a_index, 1f);
            ChangeXP(-(int)m_playerStats[(int)a_index].cost);
            m_playerStats[(int)a_index].cost *= m_playerStats[(int)a_index].costIncreaseRate;

        }
    }

    public float GetStatValue(int a_index)
    {
        return m_playerStats[a_index].value;
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
        m_playerStats = new Stat[(int)eStatIndices.count];
        SetDefaultStats(ref m_playerStats);

        m_playerStats[(int)eStatIndices.flingStrength].value = 500f;
        m_playerStats[(int)eStatIndices.strength].value = 1f;
    }


    public void ChangeGameMode(int a_change)
    {
        m_currentGameMode = (eGameMode)VLib.SafeMod((int)(m_currentGameMode + a_change),(int)(eGameMode.ModeCount));
    }

    public void ChangeXP(int a_xpReward)
    {
        m_XP += a_xpReward;
        UpdatePlayerLevel();
    }

    public void UpdatePlayerLevel()
    {
        if (m_XP >= m_maxXP)
        {
            m_XP -= m_maxXP;
            int levelPlusOne = m_playerLevel + 1;
            int firstPass = (int)(levelPlusOne + 300 * Mathf.Pow(2f, (float)(levelPlusOne) / 7f));
            m_maxXP += (int)((firstPass)/12f);
            m_playerLevel++;
        }
    }

    public void CalculateFinishedGame()
    {

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.K))
        {
            ChangeXP(1);
        }
    }
}
