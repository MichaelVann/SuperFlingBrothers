using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Equipment
{
    public enum eRarityTier
    {
        Normal,
        Uncommon,
        Magic,
        Exquisite,
        Rare,
        Unique,
        Legendary,
        Count
    }

    public struct RarityTier
    {
        public eRarityTier level;
        public Color color;
        public string name;
    }
    public bool m_equipped = false;
    public int m_equippedSlotId = -1;
    //public int m_equippedSlot = -1;
    public int m_level = 0;
    //public int m_rarityTier = 0;
    public RarityTier m_rarityTier;
    public string m_name;

    public struct EquipmentStat
    {
        public eCharacterStatIndices statType;
        public float value;
    }

    public List<EquipmentStat> m_stats;

    public void SetEquipStatus(bool a_equipped, int a_equippedSlot)
    {
        m_equipped = a_equipped;
        m_equippedSlotId = a_equippedSlot;
    }

    public void SetRarityTier(eRarityTier a_rarityTier)
    {
        m_rarityTier.level = a_rarityTier;
        UpdateRarityTier();
    }

    public void UpdateRarityTier()
    {
        switch (m_rarityTier.level)
        {
            case eRarityTier.Normal:
                m_rarityTier.color = Color.white;
                break;
            case eRarityTier.Uncommon:
                m_rarityTier.color = Color.green;
                break;
            case eRarityTier.Magic:
                m_rarityTier.color = Color.blue;
                break;
            case eRarityTier.Exquisite:
                m_rarityTier.color = Color.magenta;
                break;
            case eRarityTier.Rare:
                m_rarityTier.color = Color.yellow;
                break;
            case eRarityTier.Unique:
                m_rarityTier.color = new Color(1, 94f / 256f, 5f / 256f);//Orange;
                break;
            case eRarityTier.Legendary:
                m_rarityTier.color = Color.red;
                break;
            default:
                break;
        }
        m_rarityTier.name = m_rarityTier.level.ToString();
    }

    private void Roll(int a_level)
    {
        m_name = VLib.GenerateRandomizedName(4,8);
        int m_level = a_level;
        while (UnityEngine.Random.Range(0f,1f) <= 0.25f && m_rarityTier.level < eRarityTier.Count-1)
        {
            m_rarityTier.level++;
        }
        UpdateRarityTier();

        int points = 10 + (int)((float)(a_level)/10f);
        points = (int)(points*Mathf.Pow(1.25f, (float)m_rarityTier.level));
        int statsChosenCount = 2;
        for (int i = 0; i < (int)eCharacterStatIndices.count; i++)
        {
            EquipmentStat newStat = new EquipmentStat();
            newStat.statType = (eCharacterStatIndices)i;
            m_stats.Add(newStat);
        }

        while (m_stats.Count > statsChosenCount)
        {
            m_stats.RemoveAt(UnityEngine.Random.Range(0, m_stats.Count));
        }

        while (points > 0)
        {
            int index = UnityEngine.Random.Range(0, m_stats.Count);
            EquipmentStat statToEdit = m_stats[index];
            statToEdit.value += 1;
            m_stats[index] = statToEdit;
            points--;
        }
    }

    public Equipment()
    {
        m_stats = new List<EquipmentStat>();
        m_rarityTier = new RarityTier();
        Roll(1);
    }
}