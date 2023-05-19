using System;
using System.Collections;
using System.Collections.Generic;
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

[Serializable]
public struct CharacterStat
{
    public string name;
    public float value;
    public float scale;
    public float postAddedValue;
    public float equipmentAddedValue;
    public float effectiveValue; // = ((stat.value-1f) * stat.scale);
    public float finalValue; // = effectiveValue + stat.postAddedValue + equipmentAddedValue; 
    public float originalCost;
    public float cost;
    public float costIncreaseRate;
}

[Serializable]
public class CharacterStatHandler
{
    public int m_XP = 0;
    public int m_maxXP = 83;
    public int m_level = 1;
    public int m_allocationPoints = 0;

    public int m_DNA = 0;
    public int m_reSpecCost = 100;

    public CharacterStat[] m_stats;

    public Equipable[] m_equippedEquipables;

    const float m_baseHealthScale = 4f;

    public float GetStatFinalValue(int a_index)
    {
        return m_stats[a_index].finalValue;
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

    public static string GetStatName(int a_index)
    {
        return GetStatName((eCharacterStatIndices)a_index);
    }

    public void ChangeScore(int a_change) { m_DNA += a_change; }

    // Start is called before the first frame update
    public void Init()
    {
        m_stats = new CharacterStat[(int)eCharacterStatIndices.count];
        m_equippedEquipables = new Equipable[4];
        SetDefaultStats();
    }

    public void EquipEquipable(Equipable a_equipable, int a_slotId)
    {
        bool unequiping = false;
        bool equiping = false;
        if (a_equipable.m_equipped)
        {
            if (m_equippedEquipables[a_slotId] == a_equipable)
            {
                unequiping = true;
            }
            else
            {
                if (m_equippedEquipables[a_slotId] != null)
                {
                    m_equippedEquipables[a_slotId].m_equipped = false;
                    m_equippedEquipables[a_slotId].m_equippedSlotId = -1;
                    m_equippedEquipables[a_slotId] = null;
                }
                equiping = true;
                for (int i = 0; i < m_equippedEquipables.Length; i++)
                {
                    if (m_equippedEquipables[i] == a_equipable)
                    {
                        m_equippedEquipables[i] = null;
                    }
                }
            }

        }
        else
        {
            equiping = true;
            if (m_equippedEquipables[a_slotId] != null)
            {
                m_equippedEquipables[a_slotId].m_equipped = false;
            }
        }

        if (equiping)
        {
            a_equipable.m_equipped = true;
            a_equipable.m_equippedSlotId = a_slotId;
            m_equippedEquipables[a_slotId] = a_equipable;
        }
        else if (unequiping)
        {
            a_equipable.m_equipped = false;
            a_equipable.m_equippedSlotId = -1;
            m_equippedEquipables[a_slotId] = null;
        }



        //if (m_equippedEquipables[a_slotId] != null)
        //{
        //    m_equippedEquipables[a_slotId].m_equipped = false;

        //    if (m_equippedEquipables[a_slotId] == a_equipable)
        //    {
        //        unequiping = true;
        //        m_equippedEquipables[a_slotId].m_equipped = false;
        //        m_equippedEquipables[a_slotId] = null;
        //    }
        //}
        //if (!unequiping)
        //{
        //    m_equippedEquipables[a_slotId] = a_equipable;
        //    a_equipable.m_equipped = true;
        //    a_equipable.SetEquipStatus(true, a_slotId);
        //}
        UpdateStats();
    }

    public void ReSpec()
    {
        SetDefaultStats();
        m_allocationPoints = m_level - 1;
        m_reSpecCost *= 10;
    }

    public void UpdateStat(eCharacterStatIndices a_index)
    {
        UpdateStat((int)a_index);
    }

    public void UpdateStat(int a_index)
    {
        CharacterStat stat = m_stats[a_index];
        stat.effectiveValue = ((stat.value - 1f) * stat.scale);
        stat.finalValue = stat.effectiveValue + stat.postAddedValue;
        stat.finalValue += stat.equipmentAddedValue;
        m_stats[a_index] = stat;
    }

    public void UpdateStats()
    {
        for (int i = 0; i < m_stats.Length; i++)
        {
            m_stats[i].equipmentAddedValue = 0f;
        }

        for (int i = 0; i < m_equippedEquipables.Length; i++)
        {
            if (m_equippedEquipables[i] == null)
            {
                continue;
            }
            for (int j = 0; j < m_equippedEquipables[i].m_stats.Count; j++)
            {
                m_stats[(int)m_equippedEquipables[i].m_stats[j].statType].equipmentAddedValue += m_equippedEquipables[i].m_stats[j].value;
            }
        }

        for (int i = 0; i < m_stats.Length; i++)
        {
            UpdateStat(i);
        }
    }

    public void SetStatValue(eCharacterStatIndices a_index, float a_value)
    {
        m_stats[(int)a_index].value = a_value;
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
        SetStatValue(a_index, m_stats[(int)a_index].value + a_change);
    }

    public void SetStatScale(eCharacterStatIndices a_index, float a_value)
    {
        m_stats[(int)a_index].scale = a_value;
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

        SetStatPostAddedValue(eCharacterStatIndices.dexterity, 259f);
        SetStatScale(eCharacterStatIndices.dexterity, 4f);

        m_stats[(int)eCharacterStatIndices.constitution].scale = 10f;
        SetStatPostAddedValue(eCharacterStatIndices.constitution, 100f);
        SetStatScale(eCharacterStatIndices.strength, 5f);
        SetStatPostAddedValue(eCharacterStatIndices.strength, 50f);
    }

    public void AttemptToIncreaseStat(eCharacterStatIndices a_index)
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
            //int firstPass = (int)(levelPlusOne + 300 * Mathf.Pow(2f, (float)(levelPlusOne) / 7f));
            //m_maxXP += (int)((firstPass) / 12f);
            float additionalXPNeeded = ((float)(levelPlusOne) + 300f * Mathf.Pow(2f, (float)(levelPlusOne) / 7f))/4f;
            m_maxXP += (int)(additionalXPNeeded);

            m_level++;
            m_allocationPoints++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
