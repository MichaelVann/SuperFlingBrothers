using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfoReadout : MonoBehaviour
{
    public Text[] m_statNameTexts;
    public Text[] m_statTexts;
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
            CharacterStat stat = m_gameHandlerRef.m_playerStatHandler.m_stats[i];
            m_statTexts[i].text = "" + stat.finalValue;
            m_statTexts[i].text += "(+" + stat.equipmentAddedValue + ") (+" + stat.effectiveValue + ")";
            m_statTexts[i].color = Color.white;// CharacterStatHandler.GetStatColor((eCharacterStatIndices)i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Refresh();
    }
}
