using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoReadout : MonoBehaviour
{
    public Text[] m_statNameTexts;
    public Text[] m_statTexts;
    public TextMeshProUGUI m_characterIterationText;
    GameHandler m_gameHandlerRef;
    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        for (int i = 0; i < m_statNameTexts.Length; i++)
        {
            m_statNameTexts[i].color = CharacterStatHandler.GetStatColor(i);
        }
    }

    public void Refresh()
    {
        for (int i = 0; i < m_statTexts.Length; i++)
        {
            m_statTexts[i].text = "";
        }

        for (int i = 0; i < m_statTexts.Length && i < (int)eCharacterStatIndices.count; i++)
        {
            CharacterStat stat = m_gameHandlerRef.m_xCellTeam.m_playerXCell.m_statHandler.m_stats[i];
            m_statTexts[i].text = "" + stat.m_finalValue;
            m_statTexts[i].text += "(+" + stat.m_equipmentAddedValue + ") (+" + stat.m_effectiveValue + ") (" + stat.m_parentAddedValue + ")";
            m_statTexts[i].color = Color.white;// CharacterStatHandler.GetStatColor((eCharacterStatIndices)i);
        }

        m_characterIterationText.text = "#" + m_gameHandlerRef.m_xCellTeam.m_playerCellIteration;
    }

    // Update is called once per frame
    void Update()
    {
        Refresh();
    }
}
