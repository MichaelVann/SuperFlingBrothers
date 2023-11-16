﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    Equipment m_equipmentRef;
    Button m_buttonRef;
    public Text m_abilityNameText;
    public TextMeshProUGUI m_affixText;

    public TextMeshProUGUI m_healthText;
    public TextMeshProUGUI m_maxHealthText;

    public void SetEquipmentRef(Equipment a_equipment) { m_equipmentRef = a_equipment; }

    bool m_disabled = false;

    // Start is called before the first frame update
    void Awake()
    {
        //m_buttonRef = null;
        m_buttonRef = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Disable()
    {
        m_buttonRef.interactable = false;
        m_affixText.text = "DESTROYED";
        m_disabled = true;
    }

    public void Refresh()
    { 
        if (m_equipmentRef != null)
        {
            if (m_equipmentRef.IsBroken())
            {
                m_buttonRef.interactable = false;
                m_affixText.text = "DESTROYED";
                m_disabled = true;
                m_equipmentRef.m_activeAbility.m_activated = false;
            }
            else if (m_equipmentRef.m_activeAbility != null)
            {
                EquipmentAbility ability = m_equipmentRef.m_activeAbility;
                bool interactable = ability.m_ammo > 0;
                m_buttonRef.interactable = interactable;
                if (!ability.m_passive)
                {
                    Color buttonColor = Color.grey;
                    if (m_buttonRef.interactable)
                    {
                        buttonColor = ability.m_activated ? Color.blue : Color.grey;
                    }
                    GetComponent<Image>().color = buttonColor;
                }
                m_abilityNameText.text = ability.GetName() + (ability.m_passive ? "" : "(" + ability.m_ammo + ")");
                m_affixText.text = ability.GetAffixNames();
            }

            //Health Text
            m_healthText.text = m_equipmentRef.m_health.ToString("f0");
            m_healthText.color = VLib.PercentageToColor(m_equipmentRef.m_health / m_equipmentRef.m_maxHealth);
            m_maxHealthText.text = m_equipmentRef.m_maxHealth.ToString("f0");

        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}