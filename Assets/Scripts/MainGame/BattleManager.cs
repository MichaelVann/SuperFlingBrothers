using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public GameHandler m_gameHandlerRef;
    public GameObject m_gameHandlerTemplate;
    UIHandler m_uiHandlerRef;

    public int m_turnsRemaining;
    public float m_turnInterval;
    float m_turnsTimer = 0f;

    public bool m_frozen = false;
    float m_freezeTimer;
    float m_freezeTimerMax;
    bool m_freezing = false;
    float m_freezingTimer = 0f;
    const float m_freezingTimerMax = 0.2f;
    const float m_slowableTime = 0.8f;

    public bool m_endingGame = false;
    public float m_gameEndSlowdownFactor = 0.25f;
    public float m_gameEndTimer = 0f;
    public const float m_maxGameEndTimer = 1f;
    public bool m_victory;

    public float m_score = 0f;

    public int m_enemyCount = 0;

    public float m_pocketDamage = 2f;

    //Post game
    float m_scoreGained = 0f;
    float m_bonusTimeScoreGained = 0f;

    public float GetMaxGameEndTimer()
    {
        return m_maxGameEndTimer;
    }

    public void SetFrozen(bool a_frozen)
    {
        m_frozen = a_frozen;
        if (!m_endingGame)
        {
            Time.timeScale = a_frozen ? 0.0f : 1.0f;
        }
    }

    public void ChangeScore(float a_change) { m_score += a_change; }

    public void CalculateFinishedGame()
    {
        m_gameHandlerRef.CalculateFinishedGame();
    }

    void Awake()
    {
        m_uiHandlerRef = GetComponent<UIHandler>();
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_freezeTimerMax = m_turnInterval;
        m_freezeTimer = m_freezeTimerMax;
    }

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

    void UpdateFreezeTimer()
    {
        if (!m_frozen && !m_endingGame)
        {
            m_freezeTimer += Time.deltaTime;
            if (m_freezeTimer >= m_freezeTimerMax)
            {
                m_freezing = true;
                m_freezeTimer = 0f;
            }

            if (m_freezing)
            {
                m_freezingTimer += Time.deltaTime;
                Time.timeScale = 1f - m_slowableTime * m_freezingTimer / m_freezingTimerMax;
                if (m_freezingTimer >= m_freezingTimerMax)
                {
                    m_freezingTimer = 0f;
                    m_freezing = false;
                    SetFrozen(true);
                    
                }
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
        if (!m_endingGame)
        {
            m_endingGame = true;
            Time.timeScale = m_gameEndSlowdownFactor;
            m_uiHandlerRef.StartEnding(a_won);
            m_victory = a_won;
        }
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
            switch (m_gameHandlerRef.m_currentGameMode)
            {
                case GameHandler.eGameMode.TurnLimit:
                    UpdateTurns();
                    break;
                case GameHandler.eGameMode.Health:
                    break;
                case GameHandler.eGameMode.Pockets:
                    break;
                default:
                    break;
            }

            if (m_enemyCount <= 0)
            {
                StartEndingGame(true);
            }
            UpdateFreezeTimer();
        }
        else
        {
            UpdateGameEnding();
        }

    }
}
