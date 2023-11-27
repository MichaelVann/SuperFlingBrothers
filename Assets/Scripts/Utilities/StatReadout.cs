using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatReadout : MonoBehaviour
{
    public TextMeshProUGUI m_titleText;
    public TextMeshProUGUI m_totalStatText;
    public TextMeshProUGUI m_equipmentStatText;
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
        m_totalStatText.text =     m_characterStat.m_totalValue.ToString("f1");
        m_equipmentStatText.text = m_characterStat.m_equipmentAddedValue.ToString("f1");
        m_skillStatText.text =     m_characterStat.m_value.ToString("f1");
        m_teamStatText.text =      m_characterStat.m_parentAddedValue.ToString("f1");
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
