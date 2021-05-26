using System;
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
    public float scale;
    public float postAddedValue;
    public float effectiveValue;
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

    public int m_DNA = 0;

    public Stat[] m_stats;

    const float m_baseHealthScale = 4f;

    public float GetStatValue(int a_index)
    {
        return m_stats[a_index].effectiveValue;
    }

    public void ChangeScore(int a_change) { m_DNA += a_change; }

    // Start is called before the first frame update
    public void Init()
    {
        m_stats = new Stat[(int)eStatIndices.count];
        SetDefaultStats();
    }
    public void UpdateEffectiveStat(eStatIndices a_index)
    {
        m_stats[(int)a_index].effectiveValue = (m_stats[(int)a_index].value * m_stats[(int)a_index].scale) + m_stats[(int)a_index].postAddedValue;
    }

    public void SetStatValue(eStatIndices a_index, float a_value)
    {
        m_stats[(int)a_index].value = a_value;
        UpdateEffectiveStat(a_index);

        switch (a_index)
        {
            case eStatIndices.strength:
                break;
            case eStatIndices.dexterity:
                break;
            case eStatIndices.constitution:
                SetStatScale(eStatIndices.maxHealth, m_baseHealthScale * a_value);
                break;
            case eStatIndices.flingStrength:
                break;
            case eStatIndices.health:
                break;
            case eStatIndices.maxHealth:
                break;
            case eStatIndices.minHealth:
                break;
            case eStatIndices.count:
                break;
            default:
                break;
        }
    }

    public void ChangeStatValue(eStatIndices a_index, float a_change)
    {
        SetStatValue(a_index, m_stats[(int)a_index].value + a_change);
    }

    public void SetStatScale(eStatIndices a_index, float a_value)
    {
        m_stats[(int)a_index].scale = a_value;
        UpdateEffectiveStat(a_index);
    }

    public void SetStatPostAddedValue(eStatIndices a_index, float a_value)
    {
        m_stats[(int)a_index].postAddedValue = a_value;
        UpdateEffectiveStat(a_index);
    }

    public void SetDefaultStats()
    {
        for (int i = 0; i < (int)eStatIndices.count; i++)
        {
            SetStatScale((eStatIndices)i,1f);
            SetStatValue((eStatIndices)i, 1f);
            m_stats[i].originalCost = 10f;
            m_stats[i].cost = m_stats[i].originalCost;
            m_stats[i].costIncreaseRate = 1.8f;
            m_stats[i].postAddedValue = 0f;
        }

        SetStatPostAddedValue(eStatIndices.flingStrength, 259f);
        SetStatScale(eStatIndices.flingStrength, 20f);

        m_stats[(int)eStatIndices.health].scale = m_stats[(int)eStatIndices.maxHealth].scale = 4f;
        SetStatValue(eStatIndices.maxHealth, 1f);
        SetStatValue(eStatIndices.health, m_stats[(int)eStatIndices.maxHealth].value);
        SetStatScale(eStatIndices.strength, 0.1f);
        SetStatPostAddedValue(eStatIndices.strength, 1f);
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
