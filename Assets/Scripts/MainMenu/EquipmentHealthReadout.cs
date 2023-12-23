using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EquipmentHealthReadout : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_textRef;
    internal int m_decimalPlaces = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void SetHealth(float a_health, float a_maxHealth)
    {
        m_textRef.text = VLib.RoundToDecimalPlaces(a_health, m_decimalPlaces) + "/" + VLib.RoundToDecimalPlaces(a_maxHealth, m_decimalPlaces);
        if (a_maxHealth == 0)
        {
            a_maxHealth = 100f;
        }
        m_textRef.color = VLib.RatioToColorRGB(a_health / a_maxHealth);
    }
}
