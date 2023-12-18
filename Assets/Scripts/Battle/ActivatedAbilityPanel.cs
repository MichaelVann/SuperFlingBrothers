using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActivatedAbilityPanel : MonoBehaviour
{
    public TextMeshProUGUI m_titleText;
    public TextMeshProUGUI m_descriptionText;
    public TextMeshProUGUI m_coolDownText;

    EquipmentAbility m_equipmentAbilityRef;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void SetEquipmentAbility(EquipmentAbility a_ability)
    {
        m_equipmentAbilityRef = a_ability;
        m_titleText.text = a_ability.GetName();
        m_descriptionText.text = a_ability.GetAbilityDescription();
        m_coolDownText.text = a_ability.m_cooldown.ToString();
    }

    public void ButtonPressed()
    {

    }
}
