using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public enum eCharacterStatIndices
{
    strength,
    dexterity,
    constitution,
    protection,
    //ward,
    count
}

public class RPGLevel
{
    public float m_XP = 0;
    public float m_maxXP = 83;
    public int m_level = 1;
    public int m_allocationPoints = 0;

    public void ChangeXP(float a_xpReward)
    {
        m_XP += a_xpReward;
        UpdateLevel();
    }

    public RPGLevel()
    {

    }

    public void Copy(RPGLevel a_level)
    {
        m_XP = a_level.m_XP;
        m_maxXP = a_level.m_maxXP;
        m_level = a_level.m_level;
        m_allocationPoints = a_level.m_allocationPoints;
    }

    public void UpdateLevel()
    {
        while (m_XP >= m_maxXP)
        {
            m_XP -= m_maxXP;
            m_maxXP = GetXpNeededForLevelUP(m_level+1);
            if (m_level >= 40)
            {
                m_level = m_level;
                Debug.Log("Test");
            }
            m_level++;
            m_allocationPoints++;
        }
    }

    static float GetXpNeededForLevelUP(int a_level)
    {
        float xpNeeded = 0f;
        int levelPlusOne = a_level;
        float exponent = Mathf.Pow(2f, (levelPlusOne) / 7f);
        xpNeeded = (levelPlusOne + 300f * exponent) / 4f;
        xpNeeded = Mathf.Floor(xpNeeded);
        return xpNeeded;
    }

    public float GetXpDifference(RPGLevel a_lowerLevel, RPGLevel a_upperLevel)
    {
        float xpDiff = 0f;
        for (int i = a_lowerLevel.m_level; i < a_upperLevel.m_level; i++)
        {
            xpDiff += GetXpNeededForLevelUP(i);
        }
        xpDiff -= a_lowerLevel.m_XP;
        xpDiff += a_upperLevel.m_XP;
        return xpDiff;
    }
}

[Serializable]
public class CharacterStat
{
    [SerializeField]
    public string name;
    [SerializeField]
    public float m_value;
    [SerializeField]
    public float m_scale;
    [SerializeField]
    public float postAddedValue;
    [SerializeField]
    public float m_equipmentAddedValue;
    [SerializeField]
    public float m_parentAddedValue;
    [SerializeField]
    public float m_attributeEffectiveValue; // = ((stat.value-1f) * stat.scale);
    [SerializeField]
    public float m_effectiveValue; // = ((stat.value-1f) * stat.scale);
    [SerializeField]
    public float m_totalValue; // = ((stat.value-1f) * stat.scale);
    [SerializeField]
    public float m_finalValue; // = effectiveValue + stat.postAddedValue + equipmentAddedValue; 
    [SerializeField]
    public float originalCost;
    [SerializeField]
    public float cost;
    [SerializeField]
    public float costIncreaseRate;

    public RPGLevel m_RPGLevel;
    public RPGLevel m_lastSeenRPGLevel;
    bool m_usingRPGLevel = true;

    public CharacterStat(bool a_usingRPGLevel)
    {
        m_usingRPGLevel = a_usingRPGLevel;
        m_RPGLevel = new RPGLevel();
        m_lastSeenRPGLevel = new RPGLevel();
    }

    public void UpdateStat()
    {
        if (m_usingRPGLevel)
        {
            m_value = m_RPGLevel.m_level - 1;
        }

        m_attributeEffectiveValue = (m_value - 1f) * m_scale;
        //m_equipmentEffectiveValue = m_equipmentAddedValue * m_scale;
        //m_effectiveValue = m_equipmentEffectiveValue + m_attributeEffectiveValue;
        m_totalValue = m_value + m_equipmentAddedValue + m_parentAddedValue;
        m_finalValue = (m_totalValue)* m_scale + postAddedValue;
        //m_finalValue += m_parentAddedValue;
    }

    public void ChangeXP(float a_value)
    {
        a_value /= 3f;
        m_RPGLevel.ChangeXP(a_value);
        UpdateStat();
    }


}

[Serializable]
public class CharacterStatHandler
{
    public RPGLevel m_RPGLevel;

    public int m_reSpecCost = 100;

    //[SerializeReference]
    public CharacterStat[] m_stats;

    const float m_baseHealthScale = 4f;

    public bool m_usingRPGLevel = true;

    public void Copy(CharacterStatHandler a_statHandler)
    {
        Init(a_statHandler.m_usingRPGLevel);
        m_RPGLevel = a_statHandler.m_RPGLevel;

        m_reSpecCost = a_statHandler.m_reSpecCost;

        for (int i = 0; i < a_statHandler.m_stats.Length; i++)
        {
            m_stats[i] = a_statHandler.m_stats[i];

        }
    }

    public float GetStatFinalValue(int a_index)
    {
        return m_stats[a_index].m_finalValue;
    }

    public static string GetStatName(eCharacterStatIndices a_index, bool a_shortName = true)
    {
        string m_returnString = "";

        switch (a_index)
        {
            case eCharacterStatIndices.strength:
                m_returnString = !a_shortName ? "Strength" : "STR";
                break;
            case eCharacterStatIndices.dexterity:
                m_returnString = !a_shortName ? "Dexterity" : "DEX";
                break;
            case eCharacterStatIndices.constitution:
                m_returnString = !a_shortName ? "Constitution" : "CON";
                break;
            case eCharacterStatIndices.protection:
                m_returnString = !a_shortName ? "Protection" : "PROT";
                break;
            case eCharacterStatIndices.count:
                break;
            default:
                break;
        }

        return m_returnString;
    }

    public static Color GetStatColor(eCharacterStatIndices a_index)
    {
        Color returnColor = new Color();
        switch (a_index)
        {
            case eCharacterStatIndices.strength:
                returnColor = Color.yellow;
                break;
            case eCharacterStatIndices.dexterity:
                returnColor = new Color(0f, 0.75f, 0f);
                break;
            case eCharacterStatIndices.constitution:
                returnColor = Color.red;
                break;
            case eCharacterStatIndices.protection:
                returnColor = Color.cyan;
                break;
            case eCharacterStatIndices.count:
                break;
            default:
                break;
        }
        return returnColor;
    }

    public static Color GetStatColor(int a_index)
    {
        return GetStatColor((eCharacterStatIndices)a_index);
    }

    public static string GetStatName(int a_index)
    {
        return GetStatName((eCharacterStatIndices)a_index);
    }

    // Start is called before the first frame update
    public void Init(bool a_usingRPGLevel)
    {
        m_usingRPGLevel = a_usingRPGLevel;
        m_RPGLevel = new RPGLevel();
        m_stats = new CharacterStat[(int)eCharacterStatIndices.count];
        for (int i = 0; i < m_stats.Length; i++)
        {
            m_stats[i] = new CharacterStat(m_usingRPGLevel);
        }
        SetDefaultStats();
    }

    public void ReSpec()
    {
        SetDefaultStats();
        m_RPGLevel.m_allocationPoints = m_RPGLevel.m_level - 1;
        m_reSpecCost *= 10;
    }

    public void UpdateStat(eCharacterStatIndices a_index)
    {
        UpdateStat((int)a_index);
    }

    public void UpdateStat(int a_index)
    {
        m_stats[a_index].UpdateStat();
    }

    public void SetParentValues(CharacterStatHandler a_handler)
    {
        for (int i = 0;i < m_stats.Length;i++)
        {
            m_stats[i].m_parentAddedValue = a_handler.m_stats[i].m_totalValue;
            UpdateStat(i);
        }
    }

    public void SetStatValue(eCharacterStatIndices a_index, float a_value)
    {
        m_stats[(int)a_index].m_value = a_value;
        UpdateStat(a_index);

        switch (a_index)
        {
            case eCharacterStatIndices.strength:
                break;
            case eCharacterStatIndices.dexterity:
                break;
            case eCharacterStatIndices.constitution:
                break;
            case eCharacterStatIndices.count:
                break;
            default:
                break;
        }
    }

    public void ChangeStatValue(eCharacterStatIndices a_index, float a_change)
    {
        SetStatValue(a_index, m_stats[(int)a_index].m_value + a_change);
    }

    public void SetStatScale(eCharacterStatIndices a_index, float a_value)
    {
        m_stats[(int)a_index].m_scale = a_value;
        UpdateStat(a_index);
    }

    public void SetStatPostAddedValue(eCharacterStatIndices a_index, float a_value)
    {
        m_stats[(int)a_index].postAddedValue = a_value;
        UpdateStat(a_index);
    }

    public void SetDefaultStats()
    {
        for (int i = 0; i < (int)eCharacterStatIndices.count; i++)
        {
            SetStatScale((eCharacterStatIndices)i,1f);
            SetStatValue((eCharacterStatIndices)i, 1f);
            m_stats[i].originalCost = 10f;
            m_stats[i].cost = m_stats[i].originalCost;
            m_stats[i].costIncreaseRate = 1.8f;
            m_stats[i].postAddedValue = 0f;

            m_stats[i].name = GetStatName(i);
        }

        SetStatPostAddedValue(eCharacterStatIndices.dexterity, 0f);
        SetStatScale(eCharacterStatIndices.dexterity, 0.01f);

        m_stats[(int)eCharacterStatIndices.constitution].m_scale = 10f;
        SetStatPostAddedValue(eCharacterStatIndices.constitution, 100f);
        SetStatScale(eCharacterStatIndices.strength, 5f);
        SetStatPostAddedValue(eCharacterStatIndices.strength, 50f);
    }

    public void AttemptToIncreaseStat(eCharacterStatIndices a_index)
    {
        if (m_RPGLevel.m_allocationPoints >= 1)
        {
            ChangeStatValue(a_index, 1f);
            m_RPGLevel.m_allocationPoints--;
        }
    }

    internal void ClearPostAddedValues()
    {
        for (int i = 0; i < m_stats.Length; i++)
        {
            SetStatPostAddedValue((eCharacterStatIndices)i, 0f);
        }
    }
}
