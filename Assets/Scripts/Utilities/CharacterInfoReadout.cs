using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoReadout : MonoBehaviour
{
    public StatReadout[] m_statReadouts;
    public TextMeshProUGUI m_characterIterationText;
    GameHandler m_gameHandlerRef;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        InitialiseStatReadouts();
    }

    void InitialiseStatReadouts()
    {
        for (int i = 0; i < m_statReadouts.Length; i++)
        {
            m_statReadouts[i].AssignStat(m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_statHandler.m_stats[i]);
            m_statReadouts[i].m_titleText.color = CharacterStatHandler.GetStatColor(i);
        }
    }

    public void Refresh()
    {
        for (int i = 0; i < m_statReadouts.Length; i++)
        {
            m_statReadouts[i].Refresh();
        }

        //for (int i = 0; i < m_statTexts.Length && i < (int)eCharacterStatIndices.count; i++)
        //{
        //    CharacterStat stat = m_gameHandlerRef.m_xCellTeam.m_playerXCell.m_statHandler.m_stats[i];
        //    m_statTexts[i].text = "" + VLib.TruncateFloatsDecimalPlaces(stat.m_totalValue, 2);
        //    m_statTexts[i].text += "(+" + stat.m_equipmentAddedValue + ") (+" + stat.m_value + ") (" + stat.m_parentAddedValue + ")";
        //    m_statTexts[i].color = Color.white;// CharacterStatHandler.GetStatColor((eCharacterStatIndices)i);
        //}

        m_characterIterationText.text = "#" + m_gameHandlerRef.m_xCellSquad.m_playerCellIteration;
    }

    // Update is called once per frame
    void Update()
    {
        Refresh();
    }
}
