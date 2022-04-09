using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum eEndGameType
{
    escape,
    win,
    lose
}

public class BattleManager : MonoBehaviour
{
    public GameHandler m_gameHandlerRef;
    public GameObject m_gameHandlerTemplate;
    public GameObject m_enemyTemplate;
    public GameObject m_gameViewRef;
    UIHandler m_uiHandlerRef;
    public GameObject m_escapeZoneRef;

    public Text m_enemyCountText;

    public Image m_fadeToBlackRef;

    public UIBar m_shieldBarRef;
    public UIBar m_healthBarRef;

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
    public const float m_maxGameEndTimer = 0.55f;
    public eEndGameType m_endGameType;

    public float m_score = 0f;
    public float m_xpEarned = 0f;

    public int m_enemyCount = 0;

    public float m_pocketDamage = 2f;

    //Post game
    float m_scoreGained = 0f;
    float m_bonusTimeScoreGained = 0f;

    internal float m_coinValue = 1f;

    float m_healthbarMainPos = 0f;

    internal int m_enemiesToSpawn = 3;
    float m_enemySpawnGap = 0.4f;
    Vector3 m_coreEnemySpawnLocation;

    bool m_startingSequence = true;

    public float GetMaxGameEndTimer()
    {
        return m_maxGameEndTimer;
    }

    public void SetFrozen(bool a_frozen)
    {
        m_frozen = a_frozen;
        if (!m_endingGame)
        {
            SetTimeScale(a_frozen ? 0.0f : 1.0f);
        }
    }

    public void SetScore(float a_value) { m_score = a_value; }

    public void ChangeScore(float a_change) { m_score += a_change; }
    public void ChangeXp(float a_change) { m_xpEarned += a_change; }

    public void CalculateFinishedGame()
    {
        m_gameHandlerRef.CalculateFinishedGame();
    }

    public void SetTimeScale(float a_scale)
    {
        Time.timeScale = a_scale;
        Debug.Log(a_scale);
    }

    void Awake()
    {
        m_uiHandlerRef = GetComponent<UIHandler>();
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_freezeTimerMax = m_turnInterval;
        m_freezeTimer = m_freezeTimerMax;
        m_coreEnemySpawnLocation = new Vector3(0f, -1.6f, 0f);
        
    }

    public void Start()
    {
        if (m_gameHandlerRef.m_currentGameMode != GameHandler.eGameMode.TurnLimit)
        {
            m_turnsRemaining = 0;
        }

        m_healthBarRef.Init(m_gameHandlerRef.m_playerStatHandler.m_stats[(int)eStatIndices.constitution].finalValue, m_gameHandlerRef.m_playerStatHandler.m_stats[(int)eStatIndices.constitution].finalValue);

        if (m_gameHandlerRef.m_shieldUpgrade.m_owned)
        {
            m_shieldBarRef.Init(m_gameHandlerRef.m_playerShield.capacity);
        }
        else
        {
            m_healthBarRef.gameObject.transform.localPosition = new Vector3(0f, m_healthbarMainPos, 0f);
            m_shieldBarRef.gameObject.SetActive(false);
        }

        SpawnEnemies();
    }

    public void SpawnEnemy(Vector3 a_spawnLocation)
    {
        GameObject enemyObj = Instantiate<GameObject>(m_enemyTemplate, a_spawnLocation, Quaternion.identity, m_gameViewRef.transform);
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        enemy.SetUpType(Enemy.eEnemyType.Strikers);
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i < m_enemiesToSpawn; i++)
        {
            float x = 0f;
            float y = 0f;
            switch (i)
            {
                case 0:
                case 1:
                    x = 0f;
                    break;
                case 2:
                case 4:
                case 6:
                    x = -m_enemySpawnGap;
                    break;
                case 3:
                case 5:
                case 7:
                    x = m_enemySpawnGap;
                    break;
                default:
                    break;
            }

            switch (i)
            {
                case 0:
                    y = m_enemySpawnGap/2f;
                    break;
                case 1:
                    y = -m_enemySpawnGap/2f;
                    break;
                case 2:
                case 3:
                    y = 0f;
                    break;
                case 4:
                case 7:
                    y = m_enemySpawnGap;
                    break;
                case 5:
                case 6:
                    y = -m_enemySpawnGap;
                    break;
                default:
                    break;
            }
            Vector3 spawnLocation = new Vector3(x, y, 0f) + m_coreEnemySpawnLocation;
            SpawnEnemy(spawnLocation);
            ChangeEnemyCount(1);
        }
    }

    public void ChangeEnemyCount(int a_change)
    {
        m_enemyCount += a_change;
        if (m_enemyCount <= 0)
        {
            StartEndingGame(eEndGameType.win);
        }
    }

    public void UpdateTurns()
    {
        m_turnsTimer += Time.deltaTime;
        if (m_turnsTimer >= m_turnInterval)
        {
            m_turnsTimer -= m_turnInterval;
            if (m_gameHandlerRef.m_currentGameMode == GameHandler.eGameMode.TurnLimit)
            {
                m_turnsRemaining--;
                if (m_turnsRemaining <= 0)
                {
                    StartEndingGame(eEndGameType.lose);
                }
            }
            else
            {
                m_turnsRemaining++;
            }
        }
    }

    internal void Escape()
    {
        StartEndingGame(eEndGameType.escape);
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

                SetTimeScale(1f - m_slowableTime * m_freezingTimer / m_freezingTimerMax);
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
        m_gameHandlerRef.m_goldEarnedLastGame = m_score;
        m_gameHandlerRef.m_xpEarnedLastGame = m_score;
        //Go to post game screen
        SetTimeScale(1f);
        FindObjectOfType<GameHandler>().ChangeScene(GameHandler.eScene.postBattle);
    }

    public void StartEndingGame(eEndGameType a_type)
    {
        if (!m_endingGame)
        {
            m_endingGame = true;
            m_gameHandlerRef.SetLastGameResult(a_type);
            SetTimeScale(m_gameEndSlowdownFactor);
            m_uiHandlerRef.StartEnding(a_type);
            m_endGameType = a_type;
            if (m_endGameType == eEndGameType.lose)
            {
                SetScore(0f);
            }
        }
    }

    void UpdateGameEnding()
    {
        m_gameEndTimer += Time.deltaTime;
        m_fadeToBlackRef.color = new Color(0f, 0f, 0f, m_gameEndTimer / m_maxGameEndTimer);
        if (m_gameEndTimer >= m_maxGameEndTimer)
        {
            FinishGame();
        }
    }

    void Update()
    {
        m_enemyCountText.text = "Enemy Count: " + m_enemyCount;
        if (!m_endingGame)
        {
            UpdateTurns();
            switch (m_gameHandlerRef.m_currentGameMode)
            {
                case GameHandler.eGameMode.TurnLimit:
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
                StartEndingGame(eEndGameType.win);
            }
            UpdateFreezeTimer();
        }
        else
        {
            UpdateGameEnding();
        }
    }
}
