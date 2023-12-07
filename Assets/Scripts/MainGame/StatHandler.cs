using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public enum eCharacterStatType
{
    strength,
    dexterity,
    constitution,
    protection,
    //ward,
    count
}

[Serializable]
public class RPGLevel
{
    [SerializeField]
    public float m_XP = 0;
    [SerializeField]
    public float m_maxXP = 83;
    [SerializeField]
    public int m_level = 0;
    [SerializeField]
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
    public string m_name;
    [SerializeField]
    public eCharacterStatType m_type;
    [SerializeField]
    public float m_value;
    [SerializeField]
    public float m_scale;
    [SerializeField]
    public float m_postAddedValue;
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
    public float m_originalCost;
    [SerializeField]
    public float m_cost;
    [SerializeField]
    public float m_costIncreaseRate;
    [SerializeField]
    public bool m_reductive = false;
    [SerializeField]
    public float m_orginalReducedValue;

    public RPGLevel m_RPGLevel;
    public RPGLevel m_lastSeenRPGLevel;
    bool m_usingRPGLevel = true;

    internal static readonly float[] m_statScales = {
    /*Strength*/ 5f,
    /*Dexterity*/ 0.03f,
    /*Constitution*/ 10f,
    /*Protection*/ 0.25f
    };

    public CharacterStat(bool a_usingRPGLevel, eCharacterStatType a_type)
    {
        m_usingRPGLevel = a_usingRPGLevel;
        m_RPGLevel = new RPGLevel();
        m_lastSeenRPGLevel = new RPGLevel();
        m_type = a_type;
        m_scale = m_statScales[(int)a_type];
    }

    public static float ConvertNominalValueToEffectiveValue(float a_nominalValue, eCharacterStatType a_type)
    {
        float result = 0f;
        result = a_nominalValue * m_statScales[(int)a_type];
        return result;
    }

    public void UpdateStat()
    {
        if (m_usingRPGLevel)
        {
            m_value = m_RPGLevel.m_level;
        }

        m_attributeEffectiveValue = (m_value) * m_scale;
        //m_equipmentEffectiveValue = m_equipmentAddedValue * m_scale;
        //m_effectiveValue = m_equipmentEffectiveValue + m_attributeEffectiveValue;
        m_totalValue = m_value + m_equipmentAddedValue + m_parentAddedValue;
        m_finalValue = (m_totalValue)* m_scale + m_postAddedValue;
        if (m_reductive)
        {
            m_finalValue = m_orginalReducedValue - m_finalValue;
        }
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
    [SerializeField]
    public RPGLevel m_RPGLevel;

    [SerializeField]
    public int m_reSpecCost = 100;

    //[SerializeReference]
    public CharacterStat[] m_stats;

    const float m_baseHealthScale = 4f;

    [SerializeField]
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

    public float GetStatFinalValue(eCharacterStatType a_index)
    {
        return m_stats[(int)a_index].m_finalValue;
    }

    public static string GetStatName(eCharacterStatType a_index, bool a_shortName = false)
    {
        string m_returnString = "";

        switch (a_index)
        {
            case eCharacterStatType.strength:
                m_returnString = !a_shortName ? "Damage" : "DMG";
                break;
            case eCharacterStatType.dexterity:
                m_returnString = !a_shortName ? "Fling Interval" : "SPD";
                break;
            case eCharacterStatType.constitution:
                m_returnString = !a_shortName ? "Health" : "HLT";
                break;
            case eCharacterStatType.protection:
                m_returnString = !a_shortName ? "Protection" : "PROT";
                break;
            case eCharacterStatType.count:
                break;
            default:
                break;
        }

        return m_returnString;
    }

    public static Color GetStatColor(eCharacterStatType a_index)
    {
        Color returnColor = new Color();
        switch (a_index)
        {
            case eCharacterStatType.strength:
                returnColor = Color.yellow;
                break;
            case eCharacterStatType.dexterity:
                returnColor = new Color(0f, 0.75f, 0f);
                break;
            case eCharacterStatType.constitution:
                returnColor = Color.red;
                break;
            case eCharacterStatType.protection:
                returnColor = Color.cyan;
                break;
            case eCharacterStatType.count:
                break;
            default:
                break;
        }
        return returnColor;
    }

    public static Color GetStatColor(int a_index)
    {
        return GetStatColor((eCharacterStatType)a_index);
    }

    public static string GetStatName(int a_index, bool a_shortName = false)
    {
        return GetStatName((eCharacterStatType)a_index, a_shortName);
    }

    // Start is called before the first frame update
    public void Init(bool a_usingRPGLevel)
    {
        m_usingRPGLevel = a_usingRPGLevel;
        m_RPGLevel = new RPGLevel();
        m_stats = new CharacterStat[(int)eCharacterStatType.count];
        for (int i = 0; i < m_stats.Length; i++)
        {
            m_stats[i] = new CharacterStat(m_usingRPGLevel, (eCharacterStatType)i);
        }
        SetDefaultStats();
    }

    public void ReSpec()
    {
        SetDefaultStats();
        m_RPGLevel.m_allocationPoints = m_RPGLevel.m_level - 1;
        m_reSpecCost *= 10;
    }

    public void UpdateStat(eCharacterStatType a_index)
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

    public void SetStatValue(eCharacterStatType a_index, float a_value)
    {
        m_stats[(int)a_index].m_value = a_value;
        UpdateStat(a_index);

        switch (a_index)
        {
            case eCharacterStatType.strength:
                break;
            case eCharacterStatType.dexterity:
                break;
            case eCharacterStatType.constitution:
                break;
            case eCharacterStatType.count:
                break;
            default:
                break;
        }
    }

    public void ChangeStatValue(eCharacterStatType a_index, float a_change)
    {
        SetStatValue(a_index, m_stats[(int)a_index].m_value + a_change);
    }

    public void SetStatScale(eCharacterStatType a_index, float a_value)
    {
        m_stats[(int)a_index].m_scale = a_value;
        UpdateStat(a_index);
    }

    public void SetStatPostAddedValue(eCharacterStatType a_index, float a_value)
    {
        m_stats[(int)a_index].m_postAddedValue = a_value;
        UpdateStat(a_index);
    }

    public void SetDefaultStats()
    {
        for (int i = 0; i < (int)eCharacterStatType.count; i++)
        {
            m_stats[i].m_originalCost = 10f;
            m_stats[i].m_cost = m_stats[i].m_originalCost;
            m_stats[i].m_costIncreaseRate = 1.8f;
            m_stats[i].m_postAddedValue = 0f;

            m_stats[i].m_name = GetStatName(i);
        }

        SetStatPostAddedValue(eCharacterStatType.dexterity, 0f);
        m_stats[(int)eCharacterStatType.dexterity].m_reductive = true;
        m_stats[(int)eCharacterStatType.dexterity].m_orginalReducedValue = 2;

        SetStatPostAddedValue(eCharacterStatType.constitution, 100f);
        SetStatPostAddedValue(eCharacterStatType.strength, 50f);
    }

    public void AttemptToIncreaseStat(eCharacterStatType a_index)
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
            SetStatPostAddedValue((eCharacterStatType)i, 0f);
        }
    }
}
