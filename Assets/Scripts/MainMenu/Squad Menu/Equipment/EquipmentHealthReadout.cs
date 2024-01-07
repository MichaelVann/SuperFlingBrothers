using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EquipmentHealthReadout : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_healthTextRef;
    [SerializeField] TextMeshProUGUI m_damagePercentTextRef;
    [SerializeField] TextMeshProUGUI m_damagedTextRef;
    internal int m_decimalPlaces = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void SetEquipmentRef(Equipment a_equipment)
    {
        if (a_equipment != null) 
        {
            SetHealth(a_equipment.m_health, a_equipment.m_maxHealth);
        }
        else
        {
            SetHealth(0f, 0f);
        }
        SetDamagePercentText(a_equipment);
    }

    void SetDamagePercentText(Equipment a_equipment)
    {
        bool showingDamagedText = a_equipment != null;
        if (a_equipment != null)
        {
            float ratio = a_equipment.GetMaxHealthRatioToOriginal();
            showingDamagedText = ratio < 1f;
            if (showingDamagedText)
            {
                m_damagePercentTextRef.color = VLib.RatioToColorRGB(ratio);
                float percent = 1f - ratio;
                percent *= 100f;
                percent = VLib.RoundToDecimalPlaces(percent, 1);
                m_damagePercentTextRef.text = percent.ToString() + "%";
            }
        }
        m_damagePercentTextRef.gameObject.SetActive(showingDamagedText);
        m_damagedTextRef.gameObject.SetActive(showingDamagedText);
    }

    void SetHealth(float a_health, float a_maxHealth)
    {
        m_healthTextRef.text = VLib.RoundToDecimalPlaces(a_health, m_decimalPlaces) + "/" + VLib.RoundToDecimalPlaces(a_maxHealth, m_decimalPlaces);
        if (a_maxHealth == 0)
        {
            a_maxHealth = 100f;
        }
        m_healthTextRef.color = VLib.RatioToColorRGB(a_health / a_maxHealth);
    }
}
