using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityButton : MonoBehaviour
{
    internal Equipment m_equipmentRef;
    public Image m_panelImageRef;
    Button m_buttonRef;
    public TextMeshProUGUI m_abilityNameText;
    public TextMeshProUGUI m_affixText;

    public TextMeshProUGUI m_healthText;
    public TextMeshProUGUI m_maxHealthText;

    [SerializeField] GameObject m_cooldownStatusRef;
    [SerializeField] GameObject m_cooldownIconRef;
    [SerializeField] GameObject m_cooldownReadyTextRef;
    [SerializeField] TextMeshProUGUI m_cooldownCountTextRef;

    BattleManager m_battleManagerRef;

    public void SetEquipmentRef(Equipment a_equipment) { m_equipmentRef = a_equipment; }

    bool m_disabled = false;

    // Start is called before the first frame update
    void Awake()
    {
        //m_buttonRef = null;
        m_buttonRef = GetComponent<Button>();
        m_battleManagerRef = FindObjectOfType<BattleManager>();
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
            //If the equipment has been destroyed
            if (m_equipmentRef.IsBroken())
            {
                m_buttonRef.interactable = false;
                m_affixText.text = "DESTROYED";
                m_disabled = true;
                m_equipmentRef.m_activeAbility.m_engaged = false;
                m_cooldownCountTextRef.gameObject.SetActive(false);
                m_cooldownIconRef.gameObject.SetActive(false);
                m_cooldownReadyTextRef.SetActive(false);
            }
            //Else if the equipment has an ability
            else if (m_equipmentRef.m_activeAbility != null)
            {
                EquipmentAbility ability = m_equipmentRef.m_activeAbility;
                m_buttonRef.interactable = !ability.m_passive && ability.m_cooldown == 0;
                //Set the Color of the button depending on the type of ability and it's activation
                if (!ability.m_passive)
                {
                    Color buttonColor = Color.grey;
                    m_panelImageRef.color = buttonColor;
                }
                m_abilityNameText.text = ability.GetName();
                m_affixText.text = ability.GetAffixNames();
                m_abilityNameText.color = m_equipmentRef.m_rarity.color;

                m_cooldownStatusRef.SetActive(!ability.m_passive);
                if (!ability.m_passive)
                {
                    if (ability.m_cooldown == 0)
                    {
                        m_cooldownReadyTextRef.SetActive(true);
                        m_cooldownCountTextRef.gameObject.SetActive(false);
                        m_cooldownIconRef.gameObject.SetActive(false);
                    }
                    else 
                    {
                        m_cooldownReadyTextRef.SetActive(false);
                        m_cooldownCountTextRef.gameObject.SetActive(true);
                        m_cooldownIconRef.gameObject.SetActive(true);
                        m_cooldownCountTextRef.text = "" + ability.m_cooldown;
                    }
                }
            }

            //Health Text
            m_healthText.text = m_equipmentRef.m_health.ToString("f0");
            m_healthText.color = VLib.RatioToColorRGB(m_equipmentRef.m_health / m_equipmentRef.m_maxHealth);
            m_maxHealthText.text = m_equipmentRef.m_maxHealth.ToString("f0");

            m_buttonRef.interactable &= m_battleManagerRef.m_timeFrozen;
        }
    }
}
