﻿using System;
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
    //Debug
    public Text m_debugText;

    BattleUIHandler m_uiHandlerRef;
    public GameHandler m_gameHandlerRef;
    public GameObject m_gameHandlerTemplate;
    public GameObject m_playerTemplate;
    public GameObject m_enemyTemplate;
    public GameObject m_gameViewRef;
    public GameObject m_escapeZoneRef;
    public Button m_extraTurnButtonRef;
    public GameObject m_extraTurnButtonLockImageRef;

    public Text m_enemyCountText;
    public Text m_levelDifficultyText;

    public Image m_fadeToBlackRef;

    public UIBar m_shieldBarRef;
    public UIBar m_healthBarRef;

    public SpriteRenderer[] m_wallSpriteRenderers;

    Player m_player;
    List<Enemy> m_enemies;

    //Player Spawn Intro
    bool m_introActive = true;
    public GameObject m_playerSpawnPoint;
    public GameObject m_playerSlidePoint;
    float m_playerSlideLerp = 0f;
    float m_playerSlideTime = 1f;

    //Turns
    public int m_turnsRemaining;
    public float m_turnInterval;
    float m_turnsTimer = 0f;

    //Turn Freezing
    public bool m_timeFrozen = false;
    float m_turnFreezeTimer;
    float m_turnFreezeTimerMax;
    bool m_turnFreezing = false;
    float m_turnFreezingTimer = 0f;
    const float m_turnFreezingTimerMax = 0.2f;
    const float m_slowableTime = 0.8f;

    internal float m_coinValue = GameHandler.BATTLE_CoinValue;

    float m_healthbarMainPos = 0f;

    //Enemy Spawning
    internal int m_enemiesToSpawn = 8;
    float m_enemySpawnGap = 0.4f;
    Vector3 m_coreEnemySpawnLocation;
    int m_maxEnemyDifficulty = 0;

    bool m_startingSequence = true;

    //Extra Turn Upgrade
    int m_extraTurnsRemaining = 1;
    bool m_usingExtraTurn;

    //Pre Game
    bool m_runningPregame = true;


    //End Game
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
    int m_invaderStrengthChange = 0;

    public float GetMaxGameEndTimer()
    {
        return m_maxGameEndTimer;
    }

    public void SetFrozen(bool a_frozen)
    {
        m_turnFreezingTimer = 0f;
        m_turnFreezing = false;
        m_timeFrozen = a_frozen;
        for (int i = 0; i < m_wallSpriteRenderers.Length; i++)
        {
            float colourScale = 0.12f;
            m_wallSpriteRenderers[i].color = m_timeFrozen ? Color.yellow : new Color(colourScale, colourScale, colourScale);
        }
        if (!m_endingGame)
        {
            SetTimeScale(a_frozen ? 0.0f : 1.0f);
        }
    }

    public void SetScore(float a_value) { m_score = a_value; }

    public void ChangeScore(float a_change) { m_score += a_change; }
    public void ChangeXp(float a_change) { m_xpEarned += a_change; }
    public void ChangeInvaderStrength(int a_change) { m_invaderStrengthChange += a_change; }

    public void PrepareExtraTurn()
    {
        if (m_extraTurnsRemaining >= 1)
        {
            m_usingExtraTurn = !m_usingExtraTurn;
            UpdateExtraTurnUIState();
        }
    }

    public void UseExtraTurn()
    {
        if (m_usingExtraTurn)
        {
            m_extraTurnsRemaining--;
            m_turnFreezeTimer = m_turnFreezeTimerMax;
            m_turnFreezingTimer = m_turnFreezingTimerMax/2f;
            m_usingExtraTurn = false;
            UpdateExtraTurnUIState();
        }
    }

    public void CalculateFinishedGame()
    {
        m_gameHandlerRef.CalculateFinishedGame();
    }

    public void SetTimeScale(float a_scale)
    {
        Time.timeScale = a_scale;
        //Debug.Log(a_scale);
    }

    void SetupDebug()
    {
        m_debugText.gameObject.SetActive(GameHandler.DEBUG_MODE);
        if (GameHandler.DEBUG_MODE)
        {
        }
    }

    void Awake()
    {
        m_uiHandlerRef = GetComponent<BattleUIHandler>();
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_turnFreezeTimerMax = m_turnInterval;
        m_turnFreezeTimer = m_turnFreezeTimerMax;
        m_coreEnemySpawnLocation = new Vector3(0f, -1.6f, 0f);
    }

    public void Start()
    {
        SetupDebug();
        if (m_gameHandlerRef.m_currentGameMode != GameHandler.eGameMode.TurnLimit)
        {
            m_turnsRemaining = 0;
        }

        m_healthBarRef.Init(m_gameHandlerRef.m_playerStatHandler.m_stats[(int)eStatIndices.constitution].finalValue, m_gameHandlerRef.m_playerStatHandler.m_stats[(int)eStatIndices.constitution].finalValue);

        //Reset Trackers
        m_gameHandlerRef.m_playerLevelAtStartOfBattle = m_gameHandlerRef.m_playerStatHandler.m_level;
        m_gameHandlerRef.m_xpEarnedLastGame = 0;

        InitialiseUpgrades();

        SpawnPlayer();
        SpawnEnemies();
        m_levelDifficultyText.text = "Level Difficulty: " + m_gameHandlerRef.m_battleDifficulty;
    }

    void UpdateExtraTurnUIState()
    {
        bool enabled = m_gameHandlerRef.m_upgrades[(int)GameHandler.UpgradeId.extraTurn].m_owned && m_extraTurnsRemaining > 0;

        m_extraTurnButtonRef.interactable = enabled;
        m_extraTurnButtonLockImageRef.SetActive(!enabled);
        m_extraTurnButtonRef.GetComponentInChildren<Text>().text = "Extra Turn: " + m_extraTurnsRemaining;

        if (enabled)
        {
            m_extraTurnButtonRef.gameObject.GetComponent<Image>().color = m_usingExtraTurn ? Color.green : Color.red;
        }
        else
        {
            m_extraTurnButtonRef.gameObject.GetComponent<Image>().color = Color.grey;
        }
    }

    void InitialiseUpgrades()
    {
        //Shield
        if (m_gameHandlerRef.m_upgrades[(int)GameHandler.UpgradeId.shield].m_owned)
        {
            m_shieldBarRef.Init(m_gameHandlerRef.m_playerShield.capacity);
        }
        else
        {
            m_healthBarRef.gameObject.transform.localPosition = new Vector3(0f, m_healthbarMainPos, 0f);
            m_shieldBarRef.gameObject.SetActive(false);
        }

        //Extra turn
        if (m_gameHandlerRef.m_upgrades[(int)GameHandler.UpgradeId.extraTurn].m_owned)
        {
            m_extraTurnsRemaining = m_gameHandlerRef.m_upgrades[(int)GameHandler.UpgradeId.extraTurn].m_level;
        }
        UpdateExtraTurnUIState();
    }

    public void SpawnPlayer()
    {
        GameObject playerObj = Instantiate<GameObject>(m_playerTemplate, m_playerSpawnPoint.transform.position, Quaternion.identity, m_gameViewRef.transform);
        m_player = playerObj.GetComponent<Player>();
    }

    public void SpawnEnemy(Vector3 a_spawnLocation, Enemy.eEnemyType a_type)
    {
        GameObject enemyObj = Instantiate<GameObject>(m_enemyTemplate, a_spawnLocation, Quaternion.identity, m_gameViewRef.transform);
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        enemy.SetUpType(a_type);
    }

    public void SpawnEnemies()
    {
        m_maxEnemyDifficulty = m_gameHandlerRef.m_maxEnemyDifficulty;
        int remainingDifficulty = m_gameHandlerRef.m_battleDifficulty;
        for (int i = 0; i < m_enemiesToSpawn && remainingDifficulty > 0; i++)
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

            Enemy.eEnemyType enemyType = Enemy.eEnemyType.Idler;

            //List<int> enemyTypeLottery;
            //enemyTypeLottery = new List<int>();
            //for (int j = 0; j < GameHandler.m_enemyTypeTraits.Length; j++)
            //{
            //    if (GameHandler.m_enemyTypeTraits[j].difficulty <= m_gameHandlerRef.m_battleDifficulty)
            //    {
            //        for (int k = 0; k < GameHandler.m_enemyTypeTraits[j].difficulty; k++)
            //        {
            //            enemyTypeLottery.Add(j);
            //        }
            //    }
            //}
            //enemyType = (Enemy.eEnemyType)UnityEngine.Random.Range(0, enemyTypeLottery.Count);

            for (int j = GameHandler.m_enemyTypeTraits.Length -1 ; j >= 0; j--)
            {
                if (GameHandler.m_enemyTypeTraits[j].difficulty <= remainingDifficulty && GameHandler.m_enemyTypeTraits[j].difficulty <= m_maxEnemyDifficulty)
                {
                    enemyType = (Enemy.eEnemyType)j;
                    remainingDifficulty -= GameHandler.m_enemyTypeTraits[j].difficulty;
                    break;
                }
            }

            Debug.Log(enemyType);

            Vector3 spawnLocation = new Vector3(x, y, 0f) + m_coreEnemySpawnLocation;
            SpawnEnemy(spawnLocation, enemyType);
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
        if (!m_timeFrozen && !m_endingGame)
        {
            if (!m_turnFreezing)
            {
                m_turnFreezeTimer += Time.deltaTime;
                if (m_turnFreezeTimer >= m_turnFreezeTimerMax)
                {
                    m_turnFreezing = true;
                    m_turnFreezeTimer = 0f;
                }
            }
            else
            {
                m_turnFreezingTimer += Time.deltaTime;

                SetTimeScale(1f - m_slowableTime * m_turnFreezingTimer / m_turnFreezingTimerMax);
                if (m_turnFreezingTimer >= m_turnFreezingTimerMax)
                {
                    SetFrozen(true);
                }
            }
        }
    }

    void FinishGame()
    {
        m_gameHandlerRef.m_dnaEarnedLastGame = m_score;
        m_gameHandlerRef.m_invaderStrengthChangeLastGame = m_invaderStrengthChange;
        //Go to post game screen
        SetTimeScale(1f);
        CalculateFinishedGame();
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

        //Intro
        if (m_introActive)
        {
            m_playerSlideLerp += Time.deltaTime;
            if (m_playerSlideLerp >= m_playerSlideTime)
            {
                m_playerSlideLerp = m_playerSlideTime;
                m_introActive = false;
                SetFrozen(true);
            }
            //m_player.gameObject.transform.position = Vector3.Lerp(m_playerSpawnPoint.transform.position, m_playerSlidePoint.transform.position, m_playerSlideLerp/m_playerSlideTime);
            m_player.gameObject.transform.position = VLib.SigmoidLerp(m_playerSpawnPoint.transform.position, m_playerSlidePoint.transform.position, m_playerSlideLerp);
        }
        else if (!m_endingGame)
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
        else if(m_endingGame)
        {
            UpdateGameEnding();
        }
    }
}
