using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    UIHandler m_uiHandlerRef;

    public int m_turnsRemaining;
    public float m_turnInterval;
    float m_turnsTimer = 0f;

    public bool m_endingGame = false;
    public float m_gameEndTimer = 0f;
    public const float m_maxGameEndTimer = 3f;
    public bool m_victory;

    public float m_score = 0f;

    public int m_enemyCount = 0;

    public enum eGameMode
    {
        TurnLimit,
        Health,
        Pockets
    }
    public eGameMode m_currentGameMode;

    public float GetMaxGameEndTimer()
    {
        return m_maxGameEndTimer;
    }

    void Awake()
    {
        m_uiHandlerRef = GetComponent<UIHandler>();
    }

    public void ChangeScore(float a_change) { m_score += a_change; }

    public void ChangeEnemyCount(int a_change)
    {
        m_enemyCount += a_change;
        if (m_enemyCount <= 0)
        {
            StartEndingGame(true);
        }
    }

    public void UpdateTurns()
    {
        m_turnsTimer += Time.deltaTime;
        if (m_turnsTimer >= m_turnInterval)
        {
            m_turnsTimer -= m_turnInterval;
            m_turnsRemaining--;

            if (m_turnsRemaining <= 0)
            {
                StartEndingGame(false);
            }
        }
    }

    void FinishGame()
    {
        //Go to post game screen
        SceneManager.LoadScene("Main Menu");
    }

    public void StartEndingGame(bool a_won)
    {
        m_endingGame = true;
        //Time.timeScale = 0;
        m_uiHandlerRef.StartEnding(a_won);

    }

    void UpdateGameEnding()
    {
        m_gameEndTimer += Time.deltaTime;
        if (m_gameEndTimer >= m_maxGameEndTimer)
        {
            FinishGame();
        }
    }

    void Update()
    {
        if (!m_endingGame)
        {
            switch (m_currentGameMode)
            {
                case eGameMode.TurnLimit:
                    UpdateTurns();
                    break;
                case eGameMode.Health:
                    break;
                case eGameMode.Pockets:
                    break;
                default:
                    break;
            }

            if (m_enemyCount <= 0)
            {
                StartEndingGame(true);
            }
        }
        else
        {
            UpdateGameEnding();
        }

    }
}
