using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public TextMeshProUGUI m_fpsText;

    static public float m_shadowDistance = 0.1f;

    BattleUIHandler m_uiHandlerRef;
    public GameHandler m_gameHandlerRef;
    public GameObject m_gameHandlerTemplate;
    public GameObject m_playerTemplate;

    //Enemy Templates
    public GameObject m_enemyTemplate;
    public GameObject m_idleEnemyTemplate;
    public GameObject m_healerEnemyTemplate;
    public GameObject m_dasherEnemyTemplate;
    public GameObject m_strikerEnemyTemplate;
    public GameObject m_dodgerEnemyTemplate;

    public GameObject m_gameViewRef;
    public GameObject m_escapeZoneRef;
    public GameObject m_extraTurnButtonLockImageRef;
    public GameObject m_wallTriangleRef;
    public GameObject m_gravityWellRef;
    public Material m_whiteFlashMaterialRef;
    float m_wallXOffset = 2.11f;
    float m_wallYSpace = 5f;

    public AbilityButton[] m_abilityButtons;

    public Text m_enemyCountText;
    public Text m_levelDifficultyText;
    public Text m_currentDifficultyText;
    public Text m_deltaDifficultyText;

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
    public GameObject m_enemySpawnPointContainerRef;
    List<GameObject> m_enemySpawnPointsRefs;
    internal int m_enemiesToSpawn = 12;
    float m_enemySpawnGap = 0.4f;
    Vector3 m_coreEnemySpawnLocation;
    int m_maxEnemyDifficulty = 0;

    bool m_startingSequence = true;

    //Active Abilities
    public EquipmentAbility[] m_activeAbilities;

    //Prepared Abilities
    //int m_extraTurnsRemaining = 1;
    //bool m_usingExtraTurn;

    internal struct EnvironmentalEffects
    {
        internal bool wallTrianglesEnabled;
        internal bool gravityWellsEnabled;
        internal List<GameObject> m_gravityWellList;
    }
    EnvironmentalEffects m_environmentalEffects;

    //Pre Game
    bool m_runningPregame = true;

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

    public void SetScore(float a_value) { m_score = Mathf.Round(a_value*100f)/100f; }

    public void ChangeScore(float a_change) { SetScore(m_score + a_change); }
    public void PickUpEquipment(Equipment a_equipment) {m_equipmentCollected.Add(a_equipment);}
    public void ChangeXp(float a_change) { m_xpEarned += a_change; }
    public void ChangeInvaderStrength(int a_change) { m_invaderStrengthChange += a_change; }

    public void RefreshAbilityButtons()
    {
        for (int i = 0; i < m_abilityButtons.Length; i++)
        {
            m_abilityButtons[i].SetAbilityRef(m_activeAbilities[i]);
            m_abilityButtons[i].Refresh();
        }
    }

    public void ActivateAbility(int a_id)
    {
        for (int i = 0; i < m_activeAbilities.Length; i++)
        {
            if (m_activeAbilities[i] != null)
            {
                if(i == a_id)
                {
                    EquipmentAbility abil = m_activeAbilities[a_id];
                    if (abil.m_reactive)
                    {
                        switch (abil.m_abilityType)
                        {
                            case EquipmentAbility.eAbilityType.ExtraTurn:
                                abil.m_activated = !abil.m_activated;
                                //TODO: Enable outline around button or something, showing ability is prepared
                                break;
                            case EquipmentAbility.eAbilityType.Projectile:
                                abil.m_activated = !abil.m_activated;
                                //TODO: Enable outline around button or something, showing ability is prepared
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
                            //case ActiveAbility.eAbilityType.Projectile:
                            //    m_player.ShootProjectile(abil);
                            //    break;
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

        //ActiveAbility equipment = m_gameHandlerRef.m_playerStatHandler.m_equippedEquipment[a_id].m_activeAbilityType;
        
        RefreshAbilityButtons();
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
                    m_turnFreezeTimer = m_turnFreezeTimerMax;
                    m_turnFreezingTimer = m_turnFreezingTimerMax / 2f;
                    m_activeAbilities[i].m_activated = false;
                    RefreshAbilityButtons();
                    break;
                }
            }
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

    void SetupEnvironmentalEffects()
    {
        m_environmentalEffects = new EnvironmentalEffects();
        m_environmentalEffects.wallTrianglesEnabled = UnityEngine.Random.Range(0f,1f) <= 0.3f;
        m_environmentalEffects.gravityWellsEnabled =  UnityEngine.Random.Range(0f,1f) <= 0.3f;

        if (m_environmentalEffects.wallTrianglesEnabled)
        {
            int triangleCount = UnityEngine.Random.Range(1,9)*2;
            for (int i = 0; i < triangleCount; i++)
            {
                Vector3 pos = new Vector3();
                pos.x = m_wallXOffset * (i%2 == 0 ? -1f : 1f);
                float yGap = m_wallYSpace / (float)((triangleCount / 2)+1);
                pos.y = (m_wallYSpace/2f) - yGap * ((i/2)+1);
                pos.z = m_gameViewRef.transform.position.z;
                float rotation = 90f * (i % 2 == 0 ? -1f : 1f);
                Instantiate<GameObject>(m_wallTriangleRef, pos, Quaternion.Euler(0f,0f,rotation), m_gameViewRef.transform);
            }
            //m_wallTriangles.SetActive(m_environmentalEffects.wallTriangles);
        }

        if (m_environmentalEffects.gravityWellsEnabled)
        {
            m_environmentalEffects.m_gravityWellList = new List<GameObject>();

            int gravityWellCount = VLib.vRandom(1,4);
            for (int i = 0; i < gravityWellCount; i++)
            {
                bool spawnFound = false;
                Vector3 pos = new Vector3();
                int spawnAttempts = 0;
                while (!spawnFound && spawnAttempts < 20)
                {
                    spawnAttempts++;
                    spawnFound = true;
                    pos.x = VLib.vRandom(-m_wallXOffset, m_wallXOffset);
                    pos.y = VLib.vRandom(-m_wallYSpace / 2f, m_wallYSpace / 2f);
                    for (int j = 0; j < m_environmentalEffects.m_gravityWellList.Count; j++)
                    {
                        Vector3 deltaVec = m_environmentalEffects.m_gravityWellList[j].transform.localPosition - pos;
                        float deltaMag = deltaVec.magnitude;
                        if (deltaMag < 0.88f)
                        {
                            spawnFound = false;
                        }
                        print(deltaMag);
                    }
                    if (spawnFound)
                    {
                        //m_environmentalEffects.m_gravityWellList
                        GameObject gravityWell = Instantiate<GameObject>(m_gravityWellRef, new Vector3(), new Quaternion(), m_gameViewRef.transform);
                        gravityWell.transform.localPosition = pos;
                        m_environmentalEffects.m_gravityWellList.Add(gravityWell);
                        break;
                    }
                }
            }
        }
        
    }

    void Awake()
    {
        m_uiHandlerRef = GetComponent<BattleUIHandler>();
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_turnFreezeTimerMax = m_turnInterval;
        m_turnFreezeTimer = m_turnFreezeTimerMax;
        m_coreEnemySpawnLocation = new Vector3(0f, -1.6f, 0f);
        m_enemySpawnPointsRefs = new List<GameObject>();
        m_equipmentCollected = new List<Equipment>();
        SetupEnvironmentalEffects();
    }

    public void Start()
    {
        SetupDebug();
        if (m_gameHandlerRef.m_currentGameMode != GameHandler.eGameMode.TurnLimit)
        {
            m_turnsRemaining = 0;
        }

        m_healthBarRef.Init(m_gameHandlerRef.m_xCellTeam.m_playerXCell.m_statHandler.m_stats[(int)eCharacterStatIndices.constitution].m_finalValue, m_gameHandlerRef.m_xCellTeam.m_playerXCell.m_statHandler.m_stats[(int)eCharacterStatIndices.constitution].m_finalValue);

        //Reset Trackers
        m_gameHandlerRef.m_teamLevelAtStartOfBattle = m_gameHandlerRef.m_xCellTeam.m_statHandler.m_RPGLevel.m_level;
        m_gameHandlerRef.m_xpEarnedLastGame = 0;

        InitialiseAbilities();

        SpawnPlayer();
        foreach (SpriteRenderer spriteRendererRef in m_enemySpawnPointContainerRef.GetComponentsInChildren<SpriteRenderer>())
        {
            m_enemySpawnPointsRefs.Add(spriteRendererRef.gameObject);
            spriteRendererRef.enabled = false;
        }
        SpawnEnemies();
        m_levelDifficultyText.text = "Level Difficulty: " + m_gameHandlerRef.m_battleDifficulty;
    }

    public void InitialiseUpgrades()
    {
        //Shield
        if (m_player.m_shield.enabled)
        {
            m_shieldBarRef.Init(m_player.m_shield.capacity);
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
        for (int i = 0; i < m_gameHandlerRef.m_xCellTeam.m_playerXCell.m_equippedEquipment.Length; i++)
        {
            m_activeAbilities[i] = m_gameHandlerRef.m_xCellTeam.m_playerXCell.m_equippedEquipment[i] != null ? new EquipmentAbility(m_gameHandlerRef.m_xCellTeam.m_playerXCell.m_equippedEquipment[i].m_activeAbility) : null;
        }

        RefreshAbilityButtons();
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

        /////// OLD Spawning system
        /*
        //Go through and upgrade the enemies from top down, upgrading the first enemy as much as possible then moving to the second, and so forth
        for (int i = 0; i < spawnLocationsTypes.Length; i++)
        {
            //Redundant upgrade check that shouldn't be hit, but for future prrofing in case we ever spawn the top tier enemy straight away
            if (spawnLocationsTypes[i] + 1 >= GameHandler.m_enemyTypeTraits.Length)
            {
                continue;
            }

            //Find the upgrade cost of upgrading this enemy to the next enemy
            int currentLevelDifficulty = spawnLocationsTypes[i] <0 ? 0 : GameHandler.m_enemyTypeTraits[spawnLocationsTypes[i]].difficulty;
            int nextLevelDifficulty = GameHandler.m_enemyTypeTraits[spawnLocationsTypes[i] + 1].difficulty;
            int upgradeCost = nextLevelDifficulty - currentLevelDifficulty;

            //Check whether we can upgrade the enemy with the current budget
            bool upgradeAvailable = difficultyBudget >= upgradeCost;

            //Try to recursively upgrade the enemy until 
            while (upgradeAvailable)
            {
                spawnLocationsTypes[i]++;
                difficultyBudget -= upgradeCost;

                //If the coming upgrade is higher than available enemy types, break
                if (spawnLocationsTypes[i] + 1 >= GameHandler.m_enemyTypeTraits.Length)
                {
                    upgradeAvailable = false;
                    break;
                }

                //Check to see if the next upgrade is viable via being affordable and not over the set m_maxEnemyDifficulty
                upgradeCost = GameHandler.m_enemyTypeTraits[spawnLocationsTypes[i] + 1].difficulty - GameHandler.m_enemyTypeTraits[spawnLocationsTypes[i]].difficulty;
                bool enoughDifficulty = difficultyBudget >= upgradeCost;
                bool notOverDifficultyCap = GameHandler.m_enemyTypeTraits[spawnLocationsTypes[i] + 1].difficulty <= m_maxEnemyDifficulty;
                upgradeAvailable = (enoughDifficulty && notOverDifficultyCap);
            }
        }
        */

        //Spawn the enemies in by iterating through spawnLocationsTypes and spawning the corresponding enemy at the corresponding location
        for (int i = 0; i < processedSpawnSpots.Count; i++)
        {
            if (processedSpawnSpots[i] < 0)
            {
                continue;
            }
            Enemy.eEnemyType enemyType = (Enemy.eEnemyType)(processedSpawnSpots[i]);

            Debug.Log(enemyType);

            Vector3 spawnLocation = m_enemySpawnPointsRefs[i].transform.position;
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
        //if (m_endGameType == eEndGameType.win)
        //{
        //    m_score += m_gameHandlerRef.m_battleDifficulty;
        //}
        m_gameHandlerRef.m_dnaEarnedLastGame = m_score;
        m_gameHandlerRef.m_equipmentCollectedLastGame = m_equipmentCollected.Count;
        m_gameHandlerRef.m_invaderStrengthChangeLastGame = m_invaderStrengthChange;
        for (int i = 0; i < m_equipmentCollected.Count; i++)
        {
            m_gameHandlerRef.PickUpEquipment(m_equipmentCollected[i]);
        }
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
                m_equipmentCollected.Clear();
            }
        }
    }

    void UpdateGameEnding()
    {
        m_gameEndTimer += Time.deltaTime;
        m_fadeToBlackRef.color = new Color(0f, 0f, 0f, Mathf.Pow(m_gameEndTimer / m_maxGameEndTimer,3f));
        if (m_gameEndTimer >= m_maxGameEndTimer)
        {
            FinishGame();
        }
    }

    void Update()
    {
        m_enemyCountText.text = "Enemy Count: " + m_enemyCount;
        int fps = (int)(1f / Time.unscaledDeltaTime);
        m_fpsText.text = fps + " fps";
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
