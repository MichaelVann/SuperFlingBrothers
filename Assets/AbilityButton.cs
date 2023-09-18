using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    EquipmentAbility m_abilityRef;
    Button m_buttonRef;
    public Text m_abilityNameText;
    public TextMeshProUGUI m_affixText;
    public void SetAbilityRef(EquipmentAbility a_ability) { m_abilityRef = a_ability; } 

    // Start is called before the first frame update
    void Start()
    {
        m_buttonRef = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Refresh()
    {
        if (m_abilityRef != null)
        {
            m_buttonRef.interactable = m_abilityRef.m_ammo > 0;
            if (m_abilityRef.m_reactive)
            {
                Color buttonColor = Color.grey;
                if (m_buttonRef.interactable)
                {
                    buttonColor = m_abilityRef.m_activated ? Color.blue : Color.grey;
                }
                GetComponent<Image>().color = buttonColor;
            }
            m_abilityNameText.text = m_abilityRef.GetName() + $" ({m_abilityRef.m_ammo})";
            m_affixText.text = m_abilityRef.GetAffixNames();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
