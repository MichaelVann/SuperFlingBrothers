using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public int m_turnsRemaining = 10;
    public float m_turnInterval = 2f;
    float m_turnsTimer = 0f;

    public float m_score = 0f;

    void Awake()
    {
        
    }

    public void ChangeScore(float a_change) { m_score += a_change; }

    public void UpdateTurns()
    {
        m_turnsTimer += Time.deltaTime;
        if (m_turnsTimer >= m_turnInterval)
        {
            m_turnsTimer -= m_turnInterval;
            m_turnsRemaining--;

            if (m_turnsRemaining <= 0)
            {
                FinishGame(true);
            }
        }
    }

    void FinishGame(bool a_won)
    {
        Time.timeScale = 0;
        //Show title screen
    }

    void Update()
    {
        UpdateTurns();
    }
}
