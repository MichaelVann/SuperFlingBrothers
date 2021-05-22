using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eStatIndices
{
    strength,
    dexterity,
    constitution,
    flingStrength,
    health,
    maxHealth,
    minHealth,
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


public class StatHandler : MonoBehaviour
{
    public int m_XP = 0;
    public int m_maxXP = 83 / 4;
    public int m_level = 1;
    public int m_allocationPoints = 0;


    public Stat[] m_stats;


    public float GetStatValue(int a_index)
    {
        return m_stats[a_index].value;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_stats = new Stat[(int)eStatIndices.count];
        SetDefaultStats(ref m_stats);

        m_stats[(int)eStatIndices.flingStrength].value = 500f;
        m_stats[(int)eStatIndices.strength].value = 1f;
    }

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

    public void ChangeStatValue(eStatIndices a_index, float a_change)
    {
        m_stats[(int)a_index].value += a_change;
    }

    public void AttemptToIncreaseStat(eStatIndices a_index)
    {
        if (m_allocationPoints >= 1)
        {
            ChangeStatValue(a_index, 1f);
            m_allocationPoints--;
        }
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
            int levelPlusOne = m_level + 1;
            int firstPass = (int)(levelPlusOne + 300 * Mathf.Pow(2f, (float)(levelPlusOne) / 7f));
            m_maxXP += (int)((firstPass) / 12f);
            m_level++;
            m_allocationPoints++;
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
