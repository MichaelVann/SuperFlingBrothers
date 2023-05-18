using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Equipable
{
    public bool m_equipped = false;
    //public int m_equippedSlot = -1;
    public int m_level = 0;
    public int m_rarityTier = 0;
    public string m_name;


    public enum eEquipableStatIndex
    {
        Damage,
        AttackSpeed,
        Shield,
        Health,
        count
    }

    public struct EquipableStat
    {
        public eEquipableStatIndex statType;
        public float value;
    }

    public List<EquipableStat> m_stats;

    private void Roll(int a_level)
    {
        int m_level = a_level;
        while (UnityEngine.Random.Range(0f,1f) <= 0.25f)
        {
            m_rarityTier++;
        }

        int points = 10 + (int)((float)(a_level)/10f);
        points = (int)(points*Mathf.Pow(1.15f, m_rarityTier));
        int statsChosenCount = 2;
        for (int i = 0; i < (int)eEquipableStatIndex.count; i++)
        {
            EquipableStat newStat = new EquipableStat();
            newStat.statType = (eEquipableStatIndex)i;
            m_stats.Add(newStat);
        }

        while (m_stats.Count > statsChosenCount)
        {
            m_stats.RemoveAt(UnityEngine.Random.Range(0, m_stats.Count));
        }

        while (points > 0)
        {
            int index = UnityEngine.Random.Range(0, m_stats.Count);
            EquipableStat statToEdit = m_stats[index];
            statToEdit.value += 1;
            m_stats[index] = statToEdit;
            points--;
        }
    }

    public Equipable()
    {
        m_stats = new List<EquipableStat>();
        Roll(1);
    }
}