using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public enum eEndGameType
{
    retreat,
    win,
    lose
}

public class BattleManager : MonoBehaviour
{
    internal BattleUIHandler m_uiHandlerRef;
    public GameHandler m_gameHandlerRef;
    public GameObject m_gameHandlerTemplate;
    public GameObject m_playerTemplate;
    [SerializeField] internal Canvas m_canvasRef;

    //Debug
    public Text m_debugText;

    private BattleNode m_battleNodeRef;

    static public float m_shadowDistance = 0.1f;

    //Enemy Templates
    public GameObject m_enemyTemplate;
    public GameObject m_idleEnemyTemplate;
    public GameObject m_healerEnemyTemplate;
    public GameObject m_dasherEnemyTemplate;
    public GameObject m_strikerEnemyTemplate;
    public GameObject m_dodgerEnemyTemplate;

    public GameObject m_gameViewRef;
    public GameObject m_retreatZoneRef;
    public GameObject m_extraTurnButtonLockImageRef;
    public GameObject m_wallTriangleRef;
    public GameObject m_gravityWellRef;
    public Material m_whiteFlashMaterialRef;

    //Game Space
    public GameObject m_gameSpaceMarker;
    internal Vector2 m_gameSpace;

    List<GameObject> m_gravityWellList;

    public Text m_enemyCountText;
    public Text m_levelDifficultyText;
    public Text m_currentDifficultyText;
    public Text m_deltaDifficultyText;

    public Image m_fadeToBlackRef;

    public UIBar m_shieldBarRef;
    public UIBar m_healthBarRef;

    public SpriteRenderer[] m_wallSpriteRenderers;

    Player m_player;
    internal List<Enemy> m_enemies;
    internal int[] m_enemyTypeCounts;

    //Player Spawn Intro
    bool m_introActive = true;
    public GameObject m_playerSpawnPoint;
    public GameObject m_playerSlidePoint;
    float m_playerSlideLerp = 0f;
    float m_playerSlideTime = 1f;

    //Hit Slowdown
    internal bool m_hitSlowDownActive = false;
    float m_hitTimeSlowdownRate = 0.03f;
    float m_enemyHitTimer;
    float m_enemyHitTimerMax = 0.23f;

    //Turn Freezing
    internal bool m_timeFrozen = false;
    float m_turnFreezeTimer;
    float m_turnFreezeTimerMax = 2f;
    bool m_turnFreezing = false;
    float m_turnFreezingTimer = 0f;
    const float m_turnFreezingTimerMax = 0.2f;
    const float m_slowableTime = 0.8f;

    internal float m_coinValue = GameHandler.BATTLE_CoinValue;

    float m_healthbarMainPos = 0f;

    //Enemy Spawning
    public GameObject m_enemySpawnPointContainerRef;
    List<GameObject> m_enemySpawnPointsRefs;
    internal int m_enemiesToSpawn = 12;
    int m_maxEnemyDifficulty = 0;

    //Coin Chests
    [SerializeField] GameObject m_coinChestPrefab;
    const int m_coinChestCoinsToSpawn = 8;
    [SerializeField] List<GameObject> m_coinChestSpawnPositions;
    const int m_maxCoinsPerCoinChest = 8;

    //Active Abilities
    public EquipmentAbility[] m_activeAbilities;

    internal float m_upperLowerFlingPositionBounds;

    //End Game
    public bool m_endingGame = false;
    public float m_gameEndSlowdownFactor = 0.25f;
    public float m_gameEndTimer = 0f;
    public const float m_maxGameEndTimer = 0.55f;
    public eEndGameType m_endGameType;

    public float m_score = 0f;
    List<Equipment> m_equipmentCollected;
    public float m_xpEarned = 0f;

    public int m_enemyCount = 0;

    public float m_pocketDamage = 2f;

    //Post game
    bool m_gameFinished = false;
    int m_invaderStrengthChange = 0;

    //Audio section
    public AudioHandler m_musicPlayerRef;

    internal delegate void onEnemyCountChangeDelegate();
    internal onEnemyCountChangeDelegate m_enemyCountChangeDelegate;

    public float GetMaxGameEndTimer()
    {
        return m_maxGameEndTimer;
    }

    public void SetScore(float a_value) { m_score = Mathf.Round(a_value*100f)/100f; }

    public void ChangeScore(float a_change) { SetScore(m_score + a_change); }
    public void PickUpEquipment(Equipment a_equipment) {m_equipmentCollected.Add(a_equipment);}
    public void ChangeXp(float a_change) { m_xpEarned += a_change; }
    public void ChangeInvaderStrength(int a_change) { m_invaderStrengthChange += a_change; }

    void Awake()
    {
        m_uiHandlerRef = GetComponent<BattleUIHandler>();
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_battleNodeRef = m_gameHandlerRef.m_attemptedBattleNode;
        if (m_battleNodeRef == null)
        {
            m_battleNodeRef = new BattleNode(2, 4, null);
        }
        m_turnFreezeTimerMax = m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_statHandler.m_stats[(int)eCharacterStatType.dexterity].m_finalValue;
        //m_debugText.text = "" + m_turnFreezeTimerMax;
        m_turnFreezeTimer = 0f;
        m_enemySpawnPointsRefs = new List<GameObject>();
        m_equipmentCollected = new List<Equipment>();
        FindGameSpace();

        SetupEnvironmentalEffects();
        m_enemies = new List<Enemy>();
        m_enemyTypeCounts = new int[(int) Enemy.eEnemyType.Count];
    }

    public void Start()
    {
        SetupDebug();

        m_healthBarRef.Init(m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_statHandler.m_stats[(int)eCharacterStatType.constitution].m_finalValue, m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_statHandler.m_stats[(int)eCharacterStatType.constitution].m_finalValue);

        //Reset Trackers
        m_gameHandlerRef.m_teamLevelAtStartOfBattle = m_gameHandlerRef.m_xCellSquad.m_statHandler.m_RPGLevel.m_level;
        m_gameHandlerRef.m_xpEarnedLastGame = 0;

        InitialiseAbilities();

        SpawnPlayer();
        foreach (Transform enemySpawnTransform in m_enemySpawnPointContainerRef.transform)
        {
            m_enemySpawnPointsRefs.Add(enemySpawnTransform.gameObject);
        }
        SpawnEnemies();
        //m_levelDifficultyText.text = "Level Difficulty: " + m_gameHandlerRef.m_battleDifficulty;
        m_upperLowerFlingPositionBounds = m_wallSpriteRenderers[3].gameObject.transform.position.y;
        m_gameHandlerRef.m_audioHandlerRef.PlayBattleMusic(this);
        SpawnCoinChests();
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
            UpdateTimeScale();
        }
    }

    void FindGameSpace()
    {
        m_gameSpace = new Vector2();
        m_gameSpace[0] = m_gameSpaceMarker.transform.position.x;
        m_gameSpace[1] = m_gameSpaceMarker.transform.position.y;
    }

    internal void ActivateAbility(int a_id)
    {
        for (int i = 0; i < m_activeAbilities.Length; i++)
        {
            if (m_activeAbilities[i] != null)
            {
                if(i == a_id)
                {
                    EquipmentAbility abil = m_activeAbilities[a_id];
                    if (!abil.m_passive)
                    {
                        switch (abil.m_abilityType)
                        {
                            case EquipmentAbility.eAbilityType.ExtraTurn:
                                abil.m_activated = !abil.m_activated;
                                //TODO: Enable outline around button or something, showing ability is prepared
                                break;
                            case EquipmentAbility.eAbilityType.Bullet:
                                abil.m_activated = !abil.m_activated;
                                //TODO: Enable outline around button or something, showing ability is prepared
                                break;
                            case EquipmentAbility.eAbilityType.Snare:
                                abil.m_activated = !abil.m_activated;
                                break;
                            case EquipmentAbility.eAbilityType.Count:
                                break;
                            default:
                                break;
                        }
                    }
                    else if (abil.m_ammo >= 1)
                    {
                        switch (abil.m_abilityType)
                        {
                            case EquipmentAbility.eAbilityType.Count:
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    m_activeAbilities[i].m_activated = false;
                }
            }
        }
 
    }

    public void UseExtraTurn()
    {
        for (int i = 0; i < m_activeAbilities.Length; i++)
        {
            if (m_activeAbilities[i] != null)
            {
                if (m_activeAbilities[i].m_abilityType == EquipmentAbility.eAbilityType.ExtraTurn && m_activeAbilities[i].m_activated)
                {
                    m_activeAbilities[i].m_ammo--;
                    //m_turnFreezeTimer = m_turnFreezeTimerMax;
                    //m_turnFreezingTimer = m_turnFreezingTimerMax / 2f;
                    SetFrozen(true);
                    m_activeAbilities[i].m_activated = false;
                    m_uiHandlerRef.RefreshAbilityButtons();
                    break;
                }
            }
        }
    }

    public void CalculateFinishedGame()
    {
        m_gameHandlerRef.CalculateFinishedGame();
    }

    public void UpdateTimeScale()
    {
        float timeScale = 1f;

        if (m_gameFinished)
        {
            timeScale = 1f;
        }
        else if (m_endingGame)
        {
            timeScale = m_gameEndSlowdownFactor;
        }
        else if (m_timeFrozen)
        {
            timeScale = 0f;
        }
        else if (m_turnFreezing)
        {
            timeScale = (1f - m_slowableTime * m_turnFreezingTimer / m_turnFreezingTimerMax);
        }
        else if (m_hitSlowDownActive)
        {
            timeScale = m_hitTimeSlowdownRate;
        }

        Time.timeScale = timeScale;
        //Debug.Log(a_scale);
    }

    internal void EngageHitSlowdown(bool a_value)
    {
        m_hitSlowDownActive = a_value;
        m_enemyHitTimer = 0f;
        UpdateTimeScale();
    }

    void SpawnCoinChests()
    {
        int totalCoinsToSpawn = (int)(m_coinChestCoinsToSpawn * m_gameHandlerRef.GetBattleDifficultyBonus());
        List<Vector3> spawnPositions = new List<Vector3>();
        foreach (GameObject spawnGameObject in m_coinChestSpawnPositions)
        {
            spawnPositions.Add(spawnGameObject.transform.position);
        }

        
        while(totalCoinsToSpawn > 0 &&  m_coinChestSpawnPositions.Count > 0)
        {
            int coinsToSpawn = Mathf.Clamp(totalCoinsToSpawn, 0, m_maxCoinsPerCoinChest);
            int spawnLocationIndex = VLib.vRandom(0, spawnPositions.Count - 1);
            CoinChest coinChest = Instantiate(m_coinChestPrefab, spawnPositions[spawnLocationIndex], Quaternion.identity).GetComponent<CoinChest>();
            spawnPositions.RemoveAt(spawnLocationIndex);
            coinChest.Init(coinsToSpawn);
            totalCoinsToSpawn -= coinsToSpawn;
        }
    }
    
    void SetupDebug()
    {
        m_debugText.gameObject.SetActive(GameHandler.DEBUG_MODE);
        if (GameHandler.DEBUG_MODE)
        {
        }
    }

    void SpawnWallTriangles()
    {
        int triangleCount = VLib.vRandom(1, 4) * 2;
        for (int i = 0; i < triangleCount; i++)
        {
            Vector3 pos = new Vector3();
            pos.x = m_gameSpace.x * (i % 2 == 0 ? -1f : 1f);
            float yGap = m_gameSpace.y * 2f / (float)((triangleCount / 2) + 1);
            pos.y = m_gameSpace.y - yGap * ((i / 2) + 1);
            pos.z = m_gameViewRef.transform.position.z;
            float rotation = 90f * (i % 2 == 0 ? -1f : 1f);
            Instantiate<GameObject>(m_wallTriangleRef, pos, Quaternion.Euler(0f, 0f, rotation), m_gameViewRef.transform);
        }
    }

    void SpawnGravityWells()
    {
        m_gravityWellList = new List<GameObject>();

        int gravityWellCount = VLib.vRandom(1, 4);
        for (int i = 0; i < gravityWellCount; i++)
        {
            bool spawnFound = false;
            Vector3 pos = new Vector3();
            int spawnAttempts = 0;
            while (!spawnFound && spawnAttempts < 20)
            {
                spawnAttempts++;
                spawnFound = true;
                pos.x = VLib.vRandom(-m_gameSpace.x, m_gameSpace.x);
                pos.y = VLib.vRandom(-m_gameSpace.y, m_gameSpace.y);
                for (int j = 0; j < m_gravityWellList.Count; j++)
                {
                    Vector3 deltaVec = m_gravityWellList[j].transform.localPosition - pos;
                    float deltaMag = deltaVec.magnitude;
                    if (deltaMag < 0.88f)
                    {
                        spawnFound = false;
                    }
                }
                if (spawnFound)
                {
                    //m_environmentalEffects.m_gravityWellList
                    GameObject gravityWell = Instantiate<GameObject>(m_gravityWellRef, new Vector3(), new Quaternion(), m_gameViewRef.transform);
                    gravityWell.transform.localPosition = pos;
                    m_gravityWellList.Add(gravityWell);
                    break;
                }
            }
        }
    }

    void SetupEnvironmentalEffects()
    {
        if (m_battleNodeRef.m_environmentalEffects.wallTrianglesEnabled)
        {
            SpawnWallTriangles();
        }

        if (m_battleNodeRef.m_environmentalEffects.gravityWellsEnabled)
        {
            if (m_battleNodeRef.m_environmentalEffects.megaGravityWellEnabled)
            {
                GravityWell gravityWell = Instantiate<GameObject>(m_gravityWellRef, new Vector3(0f,0f,0f), new Quaternion(), m_gameViewRef.transform).GetComponent<GravityWell>();
                gravityWell.Init(GravityWell.eGravityWellType.MegaWhirlpool);
            }
            else
            {
                SpawnGravityWells();
            }
        }
    }

    internal void RefreshUIShieldBar()
    {
        m_shieldBarRef.Init(m_player.m_shield.capacity);

    }

    public void InitialiseUpgrades()
    {
        //Shield
        if (m_player.m_shield.enabled)
        {
            RefreshUIShieldBar();
        }
        else
        {
            m_healthBarRef.gameObject.transform.localPosition = new Vector3(0f, m_healthbarMainPos, 0f);
            m_shieldBarRef.gameObject.SetActive(false);
        }
    }

    void InitialiseAbilities()
    {
        m_activeAbilities = new EquipmentAbility[4];
        for (int i = 0; i < m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_equippedEquipment.Length; i++)
        {
            if (m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_equippedEquipment[i] != null)
            {
                m_activeAbilities[i] = m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_equippedEquipment[i].m_activeAbility;
                m_activeAbilities[i].PrepareForBattle();
            }
            //m_activeAbilities[i] = m_gameHandlerRef.m_xCellTeam.m_playerXCell.m_equippedEquipment[i] != null ? new EquipmentAbility(m_gameHandlerRef.m_xCellTeam.m_playerXCell.m_equippedEquipment[i].m_activeAbility) : null;
        }

        m_uiHandlerRef.RefreshAbilityButtons();
    }

    public void SpawnPlayer()
    {
        GameObject playerObj = Instantiate<GameObject>(m_playerTemplate, m_playerSpawnPoint.transform.position, Quaternion.identity, m_gameViewRef.transform);
        m_player = playerObj.GetComponent<Player>();
    }

    public void SpawnEnemy(Vector3 a_spawnLocation, Enemy.eEnemyType a_type)
    {
        GameObject enemyObj;
        GameObject template;
        switch (a_type)
        {
            case Enemy.eEnemyType.Idler:
                template = m_idleEnemyTemplate; 
                break;
            case Enemy.eEnemyType.Healer:
                template = m_healerEnemyTemplate;
                break;
            case Enemy.eEnemyType.InertiaDasher:
                template = m_dasherEnemyTemplate;
                break;
            case Enemy.eEnemyType.Dodger:
                template = m_dodgerEnemyTemplate;
                break;
            case Enemy.eEnemyType.Striker:
                template = m_strikerEnemyTemplate;
                break;
            case Enemy.eEnemyType.Count:
            default:
                template = m_enemyTemplate;
                break;
        }
        enemyObj = Instantiate<GameObject>(template, a_spawnLocation, Quaternion.identity, m_gameViewRef.transform);
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        enemy.SetUpType(a_type);
        m_enemies.Add(enemy);
        ChangeEnemyCount(1, enemy.m_enemyType);
    }

    public void SpawnEnemies()
    {
        int difficultyBudget = m_gameHandlerRef.m_battleDifficulty;
        m_maxEnemyDifficulty = m_gameHandlerRef.m_maxEnemyDifficulty > m_gameHandlerRef.m_battleDifficulty ? m_gameHandlerRef.m_battleDifficulty : m_gameHandlerRef.m_maxEnemyDifficulty;
        int minimumDifficulty = 0;

        bool highestSpawnableEnemyFound = false;
        int highestSpawnableEnemyIndex = (int)(Enemy.eEnemyType.Count)-1;
        //Find the highest spawnable enemy with the current difficulty available for the battle
        while (!highestSpawnableEnemyFound)
        {
            if (m_maxEnemyDifficulty >= Enemy.m_enemyTypeTraits[highestSpawnableEnemyIndex].difficulty || highestSpawnableEnemyIndex == 0)
            {
                highestSpawnableEnemyFound = true;
                m_maxEnemyDifficulty = Enemy.m_enemyTypeTraits[highestSpawnableEnemyIndex].difficulty;
            }
            else
            {
                highestSpawnableEnemyIndex--;
            }
        }
        //Find minimum spawns needed to equal or exceed the available budget
        int minimumSpawnsNeeded = Mathf.CeilToInt((float)m_gameHandlerRef.m_battleDifficulty / (float)(m_maxEnemyDifficulty));
        int maximumSpawns = 2 * minimumSpawnsNeeded;// Mathf.CeilToInt((float)m_gameHandlerRef.m_battleDifficulty / (float)GameHandler.m_enemyTypeTraits[0].difficulty);

        //Roll an amount of enemies to be spawned between the minimum and maximum needed enemies
        m_enemiesToSpawn = UnityEngine.Random.Range(minimumSpawnsNeeded, maximumSpawns);
        if (m_enemySpawnPointsRefs.Count != GameHandler.BATTLE_EnemySpawnPointCount)
        {
            throw new Exception("Update Enemy spawn count const in GameHandler");
        }
        int[] spawnLocationsTypes = new int[m_enemySpawnPointsRefs.Count];

        //Initialise the spawnLocations to empty
        for (int i = 0; i < spawnLocationsTypes.Length; i++)
        {
            spawnLocationsTypes[i] = -1;
        }

        int enemiesToSpawn = m_enemiesToSpawn;
        //Make sure that we're not trying to spawn more enemies than there are available spawn points
        enemiesToSpawn = enemiesToSpawn < m_enemySpawnPointsRefs.Count ? enemiesToSpawn : m_enemySpawnPointsRefs.Count;

        List<int> spawnSpots = new List<int>();
        List<int> processedSpawnSpots = new List<int>();

        //Do a first pass to add some basic enemies to the level
        for (int i = 0; i < enemiesToSpawn && difficultyBudget > 0; i++)
        {
            spawnLocationsTypes[i] = minimumDifficulty;
            difficultyBudget -= Enemy.m_enemyTypeTraits[minimumDifficulty].difficulty;
        }

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            spawnSpots.Add(spawnLocationsTypes[i]);
        }

        while (spawnSpots.Count > 0)
        {
            bool viable = false;
            int roll = VLib.vRandom(0, spawnSpots.Count - 1);

            if (spawnSpots[roll] + 1 < Enemy.m_enemyTypeTraits.Length)
            {
                //Find the upgrade cost of upgrading this enemy to the next enemy
                int currentLevelDifficulty = spawnSpots[roll] < 0 ? 0 : Enemy.m_enemyTypeTraits[spawnSpots[roll]].difficulty;

                int nextLevelDifficulty = Enemy.m_enemyTypeTraits[spawnSpots[roll] + 1].difficulty;
                int upgradeCost = nextLevelDifficulty - currentLevelDifficulty;

                //Check whether we can upgrade the enemy with the current budget
                bool upgradeAvailable = difficultyBudget >= upgradeCost && nextLevelDifficulty <= m_maxEnemyDifficulty;

                if (upgradeAvailable)
                {
                    spawnSpots[roll]++;
                    difficultyBudget -= upgradeCost;
                    viable = true;
                }
            }

            if (!viable)
            {
                processedSpawnSpots.Add(spawnSpots[roll]);
                spawnSpots.RemoveAt(roll);
            }
        }
 

        //Spawn the enemies in by iterating through spawnLocationsTypes and spawning the corresponding enemy at the corresponding location
        for (int i = 0; i < processedSpawnSpots.Count; i++)
        {
            if (processedSpawnSpots[i] < 0)
            {
                continue;
            }
            Enemy.eEnemyType enemyType = (Enemy.eEnemyType)(processedSpawnSpots[i]);

            Vector3 spawnLocation = m_enemySpawnPointsRefs[i].transform.position;
            SpawnEnemy(spawnLocation, enemyType);
        }
    }

    public void ChangeEnemyCount(int a_change, Enemy.eEnemyType a_type)
    {
        m_enemyCount += a_change;
        m_enemyTypeCounts[(int) a_type] += a_change;
        if (m_enemyCountChangeDelegate != null)
        {
            m_enemyCountChangeDelegate.Invoke();
        }
        if (m_enemyCount <= 0)
        {
            StartEndingGame(eEndGameType.win);
        }
    }

    internal void Retreat()
    {
        StartEndingGame(eEndGameType.retreat);
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
                UpdateTimeScale();
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
        m_gameHandlerRef.m_equipmentCollectedLastGame = m_equipmentCollected.Count;
        m_gameHandlerRef.m_invaderStrengthChangeLastGame = m_invaderStrengthChange;
        for (int i = 0; i < m_equipmentCollected.Count; i++)
        {
            m_gameHandlerRef.PickUpEquipment(m_equipmentCollected[i]);
        }
        m_gameFinished = true;
        UpdateTimeScale();
        CalculateFinishedGame();
        FindObjectOfType<GameHandler>().TransitionScene(GameHandler.eScene.postBattle);
    }

    public void StartEndingGame(eEndGameType a_type)
    {
        if (!m_endingGame)
        {
            m_endingGame = true;
            m_gameHandlerRef.SetLastGameResult(a_type);
            UpdateTimeScale();
            m_uiHandlerRef.StartEnding(a_type);
            m_endGameType = a_type;
            if (m_endGameType == eEndGameType.lose)
            {
                SetScore(0f);
                m_equipmentCollected.Clear();
            }
        }
    }

    void UpdateIntro()
    {
        m_playerSlideLerp += Time.deltaTime;
        if (m_playerSlideLerp >= m_playerSlideTime)
        {
            m_playerSlideLerp = m_playerSlideTime;
            m_introActive = false;
            SetFrozen(true);
        }
        m_player.gameObject.transform.position = VLib.SigmoidLerp(m_playerSpawnPoint.transform.position, m_playerSlidePoint.transform.position, m_playerSlideLerp);
    }

    void UpdateBattle()
    {
        if (m_enemyCount <= 0)
        {
            StartEndingGame(eEndGameType.win);
        }
        UpdateFreezeTimer();

        if (m_hitSlowDownActive)
        {
            if (!m_timeFrozen)
            {
                m_enemyHitTimer += Time.unscaledDeltaTime;
                if (m_enemyHitTimer >= m_enemyHitTimerMax)
                {
                    EngageHitSlowdown(false);
                }
            }
            else
            {
                m_enemyHitTimer = 0f;
                m_hitSlowDownActive = false;
            }
        }
    }

    void UpdateGameEnding()
    {
        m_gameEndTimer += Time.deltaTime;
        m_fadeToBlackRef.color = new Color(0f, 0f, 0f, Mathf.Pow(m_gameEndTimer / m_maxGameEndTimer, 3f));
        if (!m_gameFinished && m_gameEndTimer >= m_maxGameEndTimer)
        {
            FinishGame();
        }
    }

    void Update()
    {
        //m_enemyCountText.text = "Enemy Count: " + m_enemyCount;

        //Intro
        if (m_introActive)
        {
            UpdateIntro();
        }
        else if (!m_endingGame)
        {
            UpdateBattle();
        }
        else if (m_endingGame)
        {
            UpdateGameEnding();
        }
        m_debugText.text = "" + Time.timeScale;

    }
}
