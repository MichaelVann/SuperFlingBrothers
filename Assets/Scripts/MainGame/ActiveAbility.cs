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
        Count
    }
    public eAbilityType m_abilityType;

    public string GetName() { return VLib.GetEnumName(m_abilityType); }

    private void RollNewAbility()
    {
        m_abilityType = (eAbilityType)VLib.vRandom(0, (int)(eAbilityType.Count-1));

        switch (m_abilityType)
        {
            case eAbilityType.ExtraTurn:
                m_reactive = true;
                m_cooldown = 1;
                m_ammo = 1;
                m_level = 1;
                break;

            case eAbilityType.Count:
                break;
            default:
                break;
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
            case eAbilityType.Count:
                break;
            default:
                break;
        }
        return description;
    }

    public ActiveAbility()
    {
        RollNewAbility();
    }

    public ActiveAbility(ActiveAbility a_ability)
    {
        CopyAbility(a_ability);
    }

    public void CopyAbility(ActiveAbility a_ability)
    {
        m_cooldown = a_ability.m_cooldown;
        m_ammo = a_ability.m_ammo;
        m_reactive = a_ability.m_reactive;
        m_active = a_ability.m_active;
        m_level = a_ability.m_level;
    }

}
