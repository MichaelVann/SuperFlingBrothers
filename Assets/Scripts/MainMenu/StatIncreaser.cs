using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatIncreaser : MonoBehaviour
{
    public eCharacterStatType m_statIndex;
    GameHandler m_gameHandlerRef;
    public Text m_effectDescriptionTextRef;
    public Text m_effectSuffixTextRef;
    public TextMeshProUGUI m_effectIntervalText;
    public Counter m_statCounterRef;
    public Button m_increaseButtonRef;

    float m_statValue;


    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
    }

    public void AttemptIncrease()
    {
        m_gameHandlerRef.m_xCellSquad.AttemptToIncreaseStat(m_statIndex);
    }

    // Update is called once per frame
    void Update()
    {
        m_increaseButtonRef.interactable = m_gameHandlerRef.m_xCellSquad.m_statHandler.m_RPGLevel.m_allocationPoints > 0 ? true : false;

        CharacterStat m_referencedStat = m_gameHandlerRef.m_xCellSquad.m_statHandler.m_stats[(int)m_statIndex];

        m_statCounterRef.m_text.text = "" + m_referencedStat.m_value;

        string sign = m_statIndex == eCharacterStatType.dexterity ? "-" : "+";

        m_effectDescriptionTextRef.text = sign + m_referencedStat.m_attributeEffectiveValue;

        m_effectIntervalText.text = sign + m_referencedStat.m_scale + " " + CharacterStatHandler.GetStatName(m_referencedStat.m_type,true) + "/LVL";

        string suffixString = "";
        switch (m_statIndex)
        {
            case eCharacterStatType.strength:
                suffixString = CharacterStatHandler.GetStatName(m_statIndex);
                break;
            case eCharacterStatType.dexterity:
                suffixString = "s " + CharacterStatHandler.GetStatName(m_statIndex);
                break;
            case eCharacterStatType.constitution:
                suffixString = suffixString = CharacterStatHandler.GetStatName(m_statIndex);
                break;
            case eCharacterStatType.protection:
                suffixString = suffixString = CharacterStatHandler.GetStatName(m_statIndex);
                break;
            case eCharacterStatType.count:
                break;
            default:
                break;
        }

        m_effectSuffixTextRef.text = suffixString;

    }


}
