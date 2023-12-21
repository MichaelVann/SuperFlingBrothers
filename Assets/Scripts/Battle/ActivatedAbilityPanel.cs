using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActivatedAbilityPanel : MonoBehaviour
{
    public TextMeshProUGUI m_titleText;
    public TextMeshProUGUI m_descriptionText;
    public TextMeshProUGUI m_coolDownText;

    [SerializeField] Button m_buttonRef;

    EquipmentAbility m_equipmentAbilityRef;
    BattleManager m_battleManagerRef;

    // Start is called before the first frame update
    void Start()
    {
        m_buttonRef = GetComponent<Button>();
        m_battleManagerRef = FindObjectOfType<BattleManager>();
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

    internal void Refresh()
    {
        if (m_battleManagerRef == null)
        {
            m_battleManagerRef = FindObjectOfType<BattleManager>();
        }
        m_buttonRef.interactable = m_battleManagerRef.m_timeFrozen;
    }

    public void ButtonPressed()
    {

    }
}
