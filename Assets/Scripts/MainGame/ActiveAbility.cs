using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ActiveAbility
{
    //public bool m_enabled = false;
    public int m_cooldown;
    public int m_ammo;
    public bool m_reactive = false;
    public bool m_active = false;
    int m_level;
    public enum eAbilityType
    {
        ExtraTurn,
        Projectile,
        Count
    }

    public enum eAffix
    {
        bouncePowerup,
        Count
    }

    static string[] m_affixDescriptions = {
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

    private void RollAffixes(Equipment.eRarityTier a_rarity)
    {
        List<eAffix> potentialAffixes = new List<eAffix>();

        switch (m_abilityType)
        {
            case eAbilityType.ExtraTurn:
                break;
            case eAbilityType.Projectile:
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
    }

    private void RollNewAbility(Equipment.eRarityTier a_rarity)
    {
        m_affixes = new List<eAffix>();
        m_abilityType = (eAbilityType)VLib.vRandom(0, (int)(eAbilityType.Count-1));

        switch (m_abilityType)
        {
            case eAbilityType.ExtraTurn:
                m_reactive = true;
                m_cooldown = 1;
                m_ammo = 1;
                m_level = 1;
                break;

            case eAbilityType.Projectile:
                m_reactive = true;
                m_cooldown = 1;
                m_ammo = 5;
                m_level = 1;
                break;

            case eAbilityType.Count:
                break;
            default:
                break;
        }
        if (a_rarity > Equipment.eRarityTier.Normal)
        {
            RollAffixes(a_rarity);
        }
    }

    public string GetAbilityDescription()
    {
        string description = "";
        switch (m_abilityType)
        {
            case eAbilityType.ExtraTurn:
                description = "Extra Turn: Gives the user an extra turn on collision with the enemy, potentially giving a much needed moment of composure.";
                break;
            case eAbilityType.Projectile:
                description = "Projectile: Shoots out a projectile in the direction of movement.";
                break;
            case eAbilityType.Count:
                break;
            default:
                break;
        }
        return description;
    }

    public ActiveAbility()
    {
        RollNewAbility(Equipment.eRarityTier.Normal);
    }

    public ActiveAbility(ActiveAbility a_ability)
    {
        CopyAbility(a_ability);
    }

    public ActiveAbility(Equipment.eRarityTier a_rarity)
    {
        RollNewAbility(a_rarity);
    }

    public void CopyAbility(ActiveAbility a_ability)
    {
        m_abilityType = a_ability.m_abilityType;
        m_cooldown = a_ability.m_cooldown;
        m_ammo = a_ability.m_ammo;
        m_reactive = a_ability.m_reactive;
        m_active = a_ability.m_active;
        m_level = a_ability.m_level;
        m_affixes = a_ability.m_affixes;
    }

}
