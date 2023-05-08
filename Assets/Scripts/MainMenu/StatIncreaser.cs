using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatIncreaser : MonoBehaviour
{
    public eStatIndices m_statIndex;
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
        m_gameHandlerRef.m_playerStatHandler.AttemptToIncreaseStat(m_statIndex);
    }

    // Update is called once per frame
    void Update()
    {
        m_increaseButtonRef.interactable = m_gameHandlerRef.m_playerStatHandler.m_allocationPoints > 0 ? true : false;

        Stat m_referencedStat = m_gameHandlerRef.m_playerStatHandler.m_stats[(int)m_statIndex];

        m_statCounterRef.m_text.text = "" + m_referencedStat.value;

        m_effectDescriptionTextRef.text = "+" + m_referencedStat.effectiveValue;

        string suffixString = "";
        switch (m_statIndex)
        {
            case eStatIndices.strength:
                suffixString = "Damage";
                break;
            case eStatIndices.dexterity:
                suffixString = "μNm of Fling Strength";
                break;
            case eStatIndices.constitution:
                suffixString = "Health";
                break;
            case eStatIndices.count:
                break;
            default:
                break;
        }

        m_effectSuffixTextRef.text = suffixString;

        m_totalTextRef.text = "Total: " + m_referencedStat.finalValue;
    }


}
