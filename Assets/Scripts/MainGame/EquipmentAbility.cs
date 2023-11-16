﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class EquipmentAbility
{
    internal Equipment m_parentEquipment;
    //public bool m_enabled = false;
    internal int m_cooldown;
    internal int m_ammo;
    internal int m_maxAmmo;
    internal bool m_passive = true;
    internal bool m_activated = false;
    internal int m_level;

    internal struct Capacitor
    {
        internal float rechargeDelay;
        internal float rechargeDelayTimer;
        internal float rechargeRate;
        internal float value;
        internal float capacity;
    }
    internal Capacitor m_capacitor;

    public enum eAbilityType
    {
        Armour,
        ExtraTurn,
        Bullet,
        Shield,
        Snare,
        Count
    }

    public enum eAffix
    {
        //-Armour-
        Iron,
        //-Projectile-
        bouncePowerup,
        Count
    }

    static string[] m_affixDescriptions = {
        "Iron: 50% stronger armour.",
        "Bounce Power Up: Everytime this projectile bounces it doubles in power."
     };



    [SerializeReference]

    internal List<eAffix> m_affixes;

    public eAbilityType m_abilityType;

    public string GetName() { return VLib.GetEnumName(m_abilityType); }

    public bool HasAffix(eAffix a_affix)
    {
        bool retVal = false;
        for (int i = 0; i < m_affixes.Count; i++)
        {
            if (m_affixes[i] == a_affix)
            {
                retVal = true;
            }
        }
        return retVal;
    }

    void ApplyAffixes()
    {
        if (HasAffix(eAffix.Iron))
        {
            m_parentEquipment.m_maxHealth *= 1.5f;
            m_parentEquipment.m_health = m_parentEquipment.m_maxHealth;
        }
    }

    private void RollAffixes(Equipment.eRarityTier a_rarity)
    {
        List<eAffix> potentialAffixes = new List<eAffix>();

        switch (m_abilityType)
        {
            case eAbilityType.Armour:
                potentialAffixes.Add(eAffix.Iron);
                break;
            case eAbilityType.ExtraTurn:
                break;
            case eAbilityType.Bullet:
                potentialAffixes.Add(eAffix.bouncePowerup);
                break;
            case eAbilityType.Count:
                break;
            default:
                break;
        }

        int affixsNeeded = (int)a_rarity;

        while (affixsNeeded > 0 && potentialAffixes.Count > 0)
        {
            affixsNeeded--;

            int affixIndex = VLib.vRandom(0, potentialAffixes.Count - 1);

            m_affixes.Add(potentialAffixes[affixIndex]);
            potentialAffixes.RemoveAt(affixIndex);
        }
        ApplyAffixes();
    }

    private void RollCapacitor()
    {
        m_capacitor = new Capacitor();

        float[] rolls = VLib.vRandomSpread(3, 0.3f);

        m_capacitor.rechargeDelay = 3f / rolls[0];
        m_capacitor.capacity = 5f * rolls[1];
        m_capacitor.rechargeRate = 1.6f * rolls[2];


        m_capacitor.value = m_capacitor.capacity;
    }

    private void RollNewAbility(Equipment.eRarityTier a_rarity)
    {
        m_affixes = new List<eAffix>();
        m_abilityType = (eAbilityType)VLib.vRandom(0, (int)(eAbilityType.Count-1));

        switch (m_abilityType)
        {
            case eAbilityType.Armour:
                m_passive = true;
                m_parentEquipment.m_maxHealth *= 1.5f;
                m_parentEquipment.m_health = m_parentEquipment.m_maxHealth;
                break;
            case eAbilityType.ExtraTurn:
                m_passive = false;
                m_cooldown = 1;
                m_maxAmmo = 1;
                m_level = 1;
                break;

            case eAbilityType.Bullet:
                m_passive = false;
                m_cooldown = 1;
                m_maxAmmo = 5;
                m_level = 1;
                break;

            case eAbilityType.Shield:
                m_passive = true;
                m_activated = false;
                RollCapacitor();
                m_level = 1;
                break;

            case eAbilityType.Snare:
                m_passive = false;
                m_activated = false;
                m_level = 1;
                m_maxAmmo = 5;
                break;

            case eAbilityType.Count:
                break;
            default:
                break;
        }
        m_ammo = m_maxAmmo;
        if (a_rarity > Equipment.eRarityTier.Normal)
        {
            RollAffixes(a_rarity);
        }
    }

    public string GetAffixNames()
    {
        string description = "";
        for (int i = 0; i < m_affixes.Count; i++)
        {
            string name = "";
            if(i > 0)
            {
                name = " ";
            }
            name += VLib.GetEnumName(m_affixes[i]);
            name = name.FirstLetterToUpperCaseOrConvertNullToEmptyString();
            description += name;
        }
        return description;
    }

    public string GetAbilityDescription()
    {
        string description = "";
        switch (m_abilityType)
        {
            case eAbilityType.ExtraTurn:
                description = "Extra Turn: Gives the user an extra turn on collision with the enemy.";
                //description += "\n\nTest";
                //description += "\n\nTest";
                //description += "\n\nTest";

                break;
            case eAbilityType.Bullet:
                description = "Bullet: Shoots out a damaging projectile in the direction of movement.";
                break;
            case eAbilityType.Shield:
                description = "Shield: Protects the user from a limited amount of damage, and recharges overtime.";
                break;

            case eAbilityType.Snare:
                description = "Snare: Snares an enemy in place, stunning them.";
                break;
            case eAbilityType.Count:
                break;
            default:
                break;
        }

        for (int i = 0; i < m_affixes.Count; i++)
        {
            description += "\n" + m_affixDescriptions[(int)m_affixes[i]];
        }

        return description;
    }

    public EquipmentAbility()
    {
        //RollNewAbility(Equipment.eRarityTier.Normal);
    }

    public EquipmentAbility(EquipmentAbility a_ability)
    {
        CopyAbility(a_ability);
    }

    public EquipmentAbility(Equipment a_parentEquipment)
    {
        m_parentEquipment = a_parentEquipment;
        RollNewAbility(a_parentEquipment.m_rarity.tier);
    }

    public void CopyAbility(EquipmentAbility a_ability)
    {
        m_abilityType = a_ability.m_abilityType;
        m_cooldown = a_ability.m_cooldown;
        m_ammo = a_ability.m_ammo;
        m_passive = a_ability.m_passive;
        m_activated = a_ability.m_activated;
        m_level = a_ability.m_level;
        m_affixes = a_ability.m_affixes;
    }

    internal void PrepareForBattle()
    {
        if (m_maxAmmo != 0)
        {
            m_ammo = m_maxAmmo;
        }
    }
}
