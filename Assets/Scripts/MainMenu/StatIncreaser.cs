using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatIncreaser : MonoBehaviour
{
    public eStatIndices m_statIndex;
    GameHandler m_gameHandlerRef;
    public Counter m_statCounter;
    public Button m_increaseButton;
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
        m_increaseButton.interactable = m_gameHandlerRef.m_playerStatHandler.m_allocationPoints > 0 ? true : false;

        m_statCounter.m_text.text = "" + m_gameHandlerRef.m_playerStatHandler.m_stats[(int)m_statIndex].value;
    }


}
