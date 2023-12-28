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
    public Rarity m_rarity;
    [SerializeField]
    public string m_name;
    [SerializeField]
    public int m_goldValue = 0;
    const float m_scrapValueRatio = 0.1f;
    const float m_repairEfficiency = 0.6f;

    const float m_valueScale = 0.1f;

    [SerializeField]
    public float m_health;
    [SerializeField]
    public float m_maxHealth = 60f;

    internal bool m_newToPlayer = true;


    [SerializeReference]
    public EquipmentAbility m_activeAbility;

    internal int GetSellValue() {return (int)(m_goldValue * m_scrapValueRatio + m_goldValue * (1f- m_scrapValueRatio) * m_health/m_maxHealth);}

    static int GetAllocatablePoints(eRarityTier a_tier)
    {
        int points = 10;
        points = (int)(points * Mathf.Pow(1.25f, (float)a_tier));
        return(points);
    }

    internal static int GetNominalValue(eRarityTier a_tier)
    {
        int value = GetAllocatablePoints(a_tier);
        return value;
    }

    internal int GetNominalValue()
    {
        return GetNominalValue(m_rarity.tier);
    }

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

    //For some weird reason, having this as a no argument default constructor causes it to be called on any reference to it, as if it were a struct, and then unity complains about using Random.Range() at serialisation
    public Equipment(int a_test)
    {
        m_health = m_maxHealth;
        m_rarity = new Rarity();
        Roll(a_test);
    }

    public void Copy(Equipment a_equipment)
    {

    }

    internal void ResetAbilitysParent()
    {
        if (m_activeAbility != null)
        {
            m_activeAbility.m_parentEquipment = this;
        }
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
        if (m_health < m_maxHealth)
        {
            float damagedAmount = m_maxHealth - m_health;
            float repairedHealth = damagedAmount * m_repairEfficiency;
            m_maxHealth = repairedHealth + m_health;
            m_health = m_maxHealth;
        }
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

        if (m_rarity.tier >= eRarityTier.Unique)
        {
            m_name = VLib.GenerateRandomizedName(4, 8);
        }
        m_rarity.name = m_rarity.tier.ToString();
    }

    private void Roll(int a_test)
    {
        m_name = "";

        //Repetitively attempt to uptier the rarity
        while (UnityEngine.Random.Range(0f,1f) <= 0.25f && m_rarity.tier < eRarityTier.Count-1)
        {
            m_rarity.tier++;
        }
        UpdateRarityTier();

        m_activeAbility = new EquipmentAbility(this);

        m_goldValue = GetNominalValue();
    }

}