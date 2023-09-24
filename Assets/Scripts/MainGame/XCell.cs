using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XCell
{
    internal string m_name;
    internal CharacterStatHandler m_statHandler;
    [SerializeReference]
    public Equipment[] m_equippedEquipment;
    const float m_equipmentStatEffectMult = 0.1f;


    public XCell()
    {
        m_name = VLib.GenerateRandomizedName(3,3);
        m_statHandler = new CharacterStatHandler();
        m_statHandler.Init();
        Init();
    }

    void Init()
    {
        m_equippedEquipment = new Equipment[4];
    }

    public void EquipEquipment(Equipment a_equipment, int a_slotId)
    {
        bool unequiping = false;
        bool equiping = false;
        if (a_equipment.m_equipped)
        {
            if (m_equippedEquipment[a_slotId] == a_equipment)
            {
                unequiping = true;
            }
            else
            {
                if (m_equippedEquipment[a_slotId] != null)
                {
                    m_equippedEquipment[a_slotId].m_equipped = false;
                    m_equippedEquipment[a_slotId].m_equippedSlotId = -1;
                    m_equippedEquipment[a_slotId] = null;
                }
                equiping = true;
                for (int i = 0; i < m_equippedEquipment.Length; i++)
                {
                    if (m_equippedEquipment[i] == a_equipment)
                    {
                        m_equippedEquipment[i] = null;
                    }
                }
            }

        }
        else
        {
            equiping = true;
            if (m_equippedEquipment[a_slotId] != null)
            {
                m_equippedEquipment[a_slotId].m_equipped = false;
            }
        }

        if (equiping)
        {
            a_equipment.m_equipped = true;
            a_equipment.m_equippedSlotId = a_slotId;
            m_equippedEquipment[a_slotId] = a_equipment;
        }
        else if (unequiping)
        {
            a_equipment.m_equipped = false;
            a_equipment.m_equippedSlotId = -1;
            m_equippedEquipment[a_slotId] = null;
        }
        UpdateStats();
    }


    public void UpdateStats()
    {
        for (int i = 0; i < m_statHandler.m_stats.Length; i++)
        {
            m_statHandler.m_stats[i].equipmentAddedValue = 0f;
        }

        for (int i = 0; i < m_equippedEquipment.Length; i++)
        {
            if (m_equippedEquipment[i] == null)
            {
                continue;
            }
            for (int j = 0; j < m_equippedEquipment[i].m_stats.Count; j++)
            {
                m_statHandler.m_stats[(int)m_equippedEquipment[i].m_stats[j].statType].equipmentAddedValue += m_equippedEquipment[i].m_stats[j].value * m_equipmentStatEffectMult;
            }
        }

        for (int i = 0; i < m_statHandler.m_stats.Length; i++)
        {
            m_statHandler.UpdateStat(i);
        }
    }

}
