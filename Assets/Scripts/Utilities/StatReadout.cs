using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatReadout : MonoBehaviour
{
    public TextMeshProUGUI m_titleText;
    public TextMeshProUGUI m_totalStatText;
    public TextMeshProUGUI m_teamStatText;
    public TextMeshProUGUI m_skillStatText;

    internal CharacterStat m_characterStat;

    internal void AssignStat(CharacterStat a_characterStat)
    {
        if (a_characterStat != null)
        {
            m_characterStat = a_characterStat;
        }
        Refresh();
    }

    internal void Refresh()
    {
        //m_titleText.color = CharacterStatHandler.GetStatColor(m_characterStat);
        m_titleText.text = m_characterStat.m_name;
        m_totalStatText.text = "" + VLib.RoundToDecimalPlaces(m_characterStat.m_finalValue, CharacterStat.m_statRoundedDecimals);
        m_skillStatText.text = "" + VLib.RoundToDecimalPlaces(CharacterStat.ConvertNominalValueToEffectiveValue(m_characterStat.m_value, m_characterStat.m_type), CharacterStat.m_statRoundedDecimals);
        m_teamStatText.text = "" + VLib.RoundToDecimalPlaces(CharacterStat.ConvertNominalValueToEffectiveValue(m_characterStat.m_parentAddedValue, m_characterStat.m_type), CharacterStat.m_statRoundedDecimals);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
