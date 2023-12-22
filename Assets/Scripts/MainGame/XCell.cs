using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class XCell
{
    [SerializeField]
    internal string m_name;
    [SerializeField]
    internal CharacterStatHandler m_statHandler;
    [SerializeReference]
    public Equipment[] m_equippedEquipment;
    [SerializeField]
    const float m_equipmentStatEffectMult = 1f;
    [SerializeField]
    public Color m_colorShade;

    public XCell()
    {
        //Init();
    }

    public void Init()
    {
        m_equippedEquipment = new Equipment[4];
        m_name = VLib.GenerateRandomizedName(3, 3);
        m_name += "-";
        m_name += VLib.vRandom(100, 999);
        m_statHandler = new CharacterStatHandler();
        m_statHandler.Init(true);
        m_colorShade = new Color();
        // m_colorShade = m_colorShade.Randomise();
        m_colorShade = Color.white;
        // m_colorShade.g = 0f;
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


        for (int i = 0; i < m_equippedEquipment.Length; i++)
        {
            if (m_equippedEquipment[i] == null)
            {
                continue;
            }

        }

        for (int i = 0; i < m_statHandler.m_stats.Length; i++)
        {
            m_statHandler.UpdateStat(i);
        }
    }

}
