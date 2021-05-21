using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatIncreaser : MonoBehaviour
{
    public eStatIndices m_statIndex;
    GameHandler m_gameHandlerRef;
    public Counter m_statCounter;
    float m_statValue;
    public Counter m_costCounter;
    float m_costValue;

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
        if (m_gameHandlerRef.m_playerStatHandler.m_XP >= m_costValue)
        {
            m_costCounter.m_text.color = Color.green;
        }
        else
        {
            m_costCounter.m_text.color = Color.red;
        }

        m_costCounter.m_text.text = "" + m_gameHandlerRef.m_playerStatHandler.m_stats[(int)m_statIndex].cost;

        m_statCounter.m_text.text = "" + m_gameHandlerRef.m_playerStatHandler.m_stats[(int)m_statIndex].value;
    }


}
