using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatReadout : MonoBehaviour
{
    public TextMeshProUGUI m_totalStatText;
    public TextMeshProUGUI m_equipmentStatText;
    public TextMeshProUGUI m_skillStatText;
    public TextMeshProUGUI m_teamStatText;

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
        m_totalStatText.text =     "" + m_characterStat.m_totalValue;
        m_equipmentStatText.text = "" + m_characterStat.m_equipmentAddedValue;
        m_skillStatText.text =     "" + m_characterStat.m_value;
        m_teamStatText.text =      "" + m_characterStat.m_parentAddedValue;
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
