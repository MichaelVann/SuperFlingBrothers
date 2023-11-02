using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]
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

    [Serializable]
    public struct Rarity
    {
        public eRarityTier tier;
        public Color color;
        public string name;
    }
    [SerializeField]
    public bool m_equipped = false;
    [SerializeField]
    public int m_equippedSlotId = -1;
    [SerializeField]
    public int m_level = 0;
    [SerializeField]
    public Rarity m_rarity;
    [SerializeField]
    public string m_name;
    [SerializeField]
    public int m_goldValue = 0;
    const float m_scrapValueRatio = 0.1f;
    const float m_repairEfficacy = 0.95f;


    [SerializeField]
    public float m_health;
    [SerializeField]
    public float m_maxHealth = 60f;


    internal bool m_newToPlayer = true;

    [Serializable]
    public struct EquipmentStat
    {
        public eCharacterStatIndices statType;
        public float value;
    }

    //[SerializeReference]
    public List<EquipmentStat> m_stats;

    [SerializeReference]
    public EquipmentAbility m_activeAbility;

    internal int GetGoldValue() {return (int)(m_goldValue * m_scrapValueRatio + m_goldValue * (1f- m_scrapValueRatio) * m_health/m_maxHealth);}
    internal int GetRepairCost()
    {
        float repairCost = m_goldValue;
        float healthRatio = m_health / m_maxHealth;
        float markUp = 1.5f;

        repairCost *= 1f - healthRatio;
        repairCost *= markUp;
        return (int)repairCost; 
    }
    internal bool IsBroken() { return m_health <= 0; }

    public Equipment(int a_level)
    {
        m_health = m_maxHealth;
        m_stats = new List<EquipmentStat>();
        m_rarity = new Rarity();
        Roll(a_level);
    }

    public void Copy(Equipment a_equipment)
    {

    }

    internal float Damage(float a_damage)
    {
        float excessDamage = 0f;
        m_health -= a_damage;
        if (m_health <= 0)
        {
            excessDamage = -m_health;
            m_health = 0f;
        }
        return excessDamage;
    }

    internal void Repair()
    {
        m_maxHealth *= m_repairEfficacy;
        m_health = m_maxHealth;
    }

    public void SetEquipStatus(bool a_equipped, int a_equippedSlot)
    {
        m_equipped = a_equipped;
        m_equippedSlotId = a_equippedSlot;
    }

    public void SetRarityTier(eRarityTier a_rarityTier)
    {
        m_rarity.tier = a_rarityTier;
        UpdateRarityTier();
    }

    public void UpdateRarityTier()
    {
        switch (m_rarity.tier)
        {
            case eRarityTier.Normal:
                m_rarity.color = Color.white;
                break;
            case eRarityTier.Uncommon:
                m_rarity.color = Color.green;
                break;
            case eRarityTier.Magic:
                m_rarity.color = Color.blue;
                break;
            case eRarityTier.Exquisite:
                m_rarity.color = Color.magenta;
                break;
            case eRarityTier.Rare:
                m_rarity.color = Color.yellow;
                break;
            case eRarityTier.Unique:
                m_rarity.color = new Color(1, 94f / 256f, 5f / 256f);//Orange;
                break;
            case eRarityTier.Legendary:
                m_rarity.color = Color.red;
                break;
            default:
                break;
        }
        m_rarity.name = m_rarity.tier.ToString();
    }

    private void Roll(int a_level)
    {
        m_name = VLib.GenerateRandomizedName(4,8);
        m_level = a_level;

        //Repetitively attempt to uptier the rarity
        while (UnityEngine.Random.Range(0f,1f) <= 0.25f && m_rarity.tier < eRarityTier.Count-1)
        {
            m_rarity.tier++;
        }
        UpdateRarityTier();

        m_activeAbility = new EquipmentAbility(this);

        int points = 10 + (int)((float)(a_level)*2f);
        points = (int)(points*Mathf.Pow(1.25f, (float)m_rarity.tier));
        m_goldValue = points;
        int statsChosenCount = VLib.vRandom(1,m_stats.Count-1);
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

}