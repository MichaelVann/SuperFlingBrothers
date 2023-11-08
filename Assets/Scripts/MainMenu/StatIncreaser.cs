using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatIncreaser : MonoBehaviour
{
    public eCharacterStatIndices m_statIndex;
    GameHandler m_gameHandlerRef;
    public Text m_effectDescriptionTextRef;
    public Text m_effectSuffixTextRef;
    public Text m_totalTextRef;
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
        m_gameHandlerRef.m_xCellTeam.AttemptToIncreaseStat(m_statIndex);
    }

    // Update is called once per frame
    void Update()
    {
        m_increaseButtonRef.interactable = m_gameHandlerRef.m_xCellTeam.m_statHandler.m_RPGLevel.m_allocationPoints > 0 ? true : false;

        CharacterStat m_referencedStat = m_gameHandlerRef.m_xCellTeam.m_statHandler.m_stats[(int)m_statIndex];

        m_statCounterRef.m_text.text = "" + m_referencedStat.m_value;

        m_effectDescriptionTextRef.text = "+" + m_referencedStat.m_attributeEffectiveValue;

        string suffixString = "";
        switch (m_statIndex)
        {
            case eCharacterStatIndices.strength:
                suffixString = "Hit Damage";
                break;
            case eCharacterStatIndices.dexterity:
                suffixString = "s Turn Speed";
                break;
            case eCharacterStatIndices.constitution:
                suffixString = "Health";
                break;
            case eCharacterStatIndices.protection:
                suffixString = "Armour";
                break;
            case eCharacterStatIndices.count:
                break;
            default:
                break;
        }

        m_effectSuffixTextRef.text = suffixString;

        m_totalTextRef.text = "Total: " + m_referencedStat.m_finalValue;
    }


}
