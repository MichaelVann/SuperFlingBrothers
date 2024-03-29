﻿using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameHandler;
using static UnityEngine.UI.CanvasScaler;

[Serializable]

public class GameHandler : MonoBehaviour
{
    internal static GameHandler m_staticAutoRef;

    static internal int MAIN_VERSION_NUMBER;
    static internal int SUB_VERSION_NUMBER;


    // -- BALANCE VARIABLES --

    static internal float GAME_enemyXPRewardScale = 3f;
    static internal float BATTLE_CoinValue = 1f;
    static internal float BATTLE_ShadowAngle = 135f;
    static internal float BATTLE_FlingStrength = 320f;
    static internal float BATTLE_SkillXPScale = 2f;
    static internal float BATTLE_EnemyEquipmentDropChanceScale = 0.5f;
    internal const int    BATTLE_EnemySpawnPointCount = 19;
    internal const int BATTLE_NucleusTicks = 13;

    static internal float PRE_BATTLE_WarfrontChange = -0.17f; //Default is -0.17f
    //Damageables
    static internal float DAMAGEABLE_DragCoefficient = 1.25f;
    static internal float DAMAGEABLE_HitVelocityMultiplier = 1.6f;
    static internal float DAMAGEABLE_ArmourAbsorption = 0.6f;
    static internal float DAMAGEABLE_DefaultMass = 1f;
    static internal float DAMAGEABLE_BumpFlingStrengthMult = 0.25f;
    static internal float DAMAGEABLE_PocketFlingStrength = 100f;
    static internal float DAMAGEABLE_DamagePerSpeedDivider = 8f;

    [SerializeField] internal AudioHandler m_audioHandlerRef;

    //Player

    // -- END OF BALANCE --

    //Scene
    public enum eScene
    {
        mainMenu,
        preBattle,
        battle,
        postBattle
    } eScene m_currentScene;
    eScene m_queuedScene;
    [SerializeField] Image m_sceneFadeImageRef;
    [SerializeField] internal Canvas m_sceneFadeCanvasRef;
    vTimer m_sceneFadeTimer;
    bool m_sceneFadingOut;
    const float m_sceneFadeDuration = 0.35f;

    // Dialogue
    internal TutorialManager m_tutorialManager;

    internal const string m_speakerCharacterName = "Micky";// Sergeant Geras";
    [SerializeField] GameObject m_dialogBoxPrefab;
    [SerializeField] Canvas m_dialogCanvasRef;
    [SerializeField] GameObject m_tutorialTaskPrefab;
    [SerializeField] Canvas m_tutorialTaskCanvasRef;

    //GAME
    internal float m_cash;
    
    //Player
    internal XCellSquad m_xCellSquad;
    bool m_playerWasKilledLastBattle = false;
    internal bool m_squadRenameNotificationPending = false;

    //Body
    public HumanBody m_humanBody;

    //Current Battle
    internal BattleNode m_attemptedBattleNode;
    internal int m_battleDifficulty = 12;
    internal int m_maxEnemyDifficulty = 20; //Set from humanbody

    //Last Game
    internal struct LastGameStats
    {
        public BattleNode m_lastAttemptedBattleNode;
        internal eEndGameType m_lastGameResult;
        internal float m_xpEarnedLastGame;
        internal int m_invaderStrengthChangeLastGame;
        internal int m_teamLevelAtStartOfBattle;
        internal float m_dnaEarnedLastGame;
        internal int m_equipmentCollectedLastGame;
        internal int m_lastXpBonus;
        internal int m_lastDnaBonus;
        internal bool m_frontLineResultsPending;
        internal float m_lastFrontLinePlayerEffect;
        internal float m_lastFrontLineEnemyEffect;
        internal float m_lastFrontLineChange;
    }
    internal LastGameStats m_lastGameStats;

    //Store
    internal UpgradeTree m_upgradeTree;
    internal float m_junkEquipmentLevelScale;
    internal float m_junkEquipmentCostScale;

    //Equipment
    const int m_startingEquipment = 1;
    public List<Equipment> m_equipmentInventory;

    internal StockHandler m_stockHandler;

    //SaveDataUtility m_saveDataUtility;
    const bool m_autoLoadDataOnLaunch = true;
    internal const bool m_autoSaving = true;

    //Highscores
    internal struct Highscore
    {
        internal Highscore(string a_name, int a_score)
        {
            name = a_name;
            score = a_score;
        }
        internal string name;
        internal int score;
        internal const int maxScores = 8;
    }
    internal List<Highscore> m_highscoreList;

    [SerializeField] GameObject m_explosionTestPrefab;

    [SerializeField] Image m_scanLinesRef;
    internal struct GameOptions
    {
        internal int scanLineSetting;
    }
    internal GameOptions m_gameOptions;


    internal float GetBattleDifficultyBonus()
    {
        float bonusMult = Mathf.Pow(1.022f, m_battleDifficulty);
        bonusMult = VLib.TruncateFloatsDecimalPlaces(bonusMult, 2);
        return bonusMult;
    }

    internal int GetGeneratedEquipmentLevel(bool a_junk) { return (int)(m_xCellSquad.m_statHandler.m_RPGLevel.m_level * (a_junk ? m_junkEquipmentLevelScale : 1f)); }

    internal int GetJunkEquipmentCost() { return  Equipment.GetNominalValue(Equipment.eRarityTier.Magic); }

    internal bool CanAffordJunkEquipment() { return m_cash >= GetJunkEquipmentCost(); }

    internal void AttemptToRespec()
    {
        if (m_xCellSquad.m_statHandler.m_reSpecCost <= m_cash)
        {
            m_cash -= m_xCellSquad.m_statHandler.m_reSpecCost;
            m_xCellSquad.m_statHandler.ReSpec();
        }
    }

    public void SetLastGameResult(eEndGameType a_value) { m_lastGameStats.m_lastGameResult = a_value; }

    public void ChangeCash(float a_change) { m_cash += a_change; AutoSaveCheck(); }

    public void SetSelectedBattle(BattleNode a_battleNode) { m_attemptedBattleNode = a_battleNode; }

    public float GetCurrentCash() { return m_cash; }

    public void SetBattleDifficulty(int a_difficulty) { m_battleDifficulty = a_difficulty; }
    public void SetMaxEnemyDifficulty(int a_difficulty) { m_maxEnemyDifficulty = a_difficulty; }

    internal StockHandler GetStockHandlerRef() { return m_stockHandler; }

    void Awake()
    {
        m_staticAutoRef = this;
        int[] versionNumbers = VLib.GetApplicationVersionNumbers();
        MAIN_VERSION_NUMBER = versionNumbers[0];
        SUB_VERSION_NUMBER = versionNumbers[1];
        DontDestroyOnLoad(gameObject);
        InitGameOptions();
        //m_saveDataUtility = new SaveDataUtility(this);
        ResetGame();
        if (m_autoLoadDataOnLaunch)
        {
            LoadGame();
        }
        if (Application.isMobilePlatform)
        {
            Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
        }
    }

    void InitGameOptions()
    {
        m_gameOptions = new GameOptions();
        m_gameOptions.scanLineSetting = 2;
    }

    internal void SetScanLines(int a_setting)
    {
        m_gameOptions.scanLineSetting = a_setting;
        RefreshScanLines();
    }
    
    void RefreshScanLines()
    {
        switch (m_gameOptions.scanLineSetting)
        {
            case 1:
                m_scanLinesRef.pixelsPerUnitMultiplier = 1f;
                break;
            case 2:
                m_scanLinesRef.pixelsPerUnitMultiplier = 0.5f;
                break;
            default:
                break;
        }
        m_scanLinesRef.gameObject.SetActive(m_gameOptions.scanLineSetting != 0);
    }

    void InitialiseTutorials()
    {
        if (m_tutorialManager != null)
        {
            m_tutorialManager.CleanUp();
        }
        m_tutorialManager = new TutorialManager(this, m_dialogBoxPrefab, m_tutorialTaskPrefab, m_dialogCanvasRef.gameObject, m_tutorialTaskCanvasRef.gameObject);
    }
     
    internal void ResetRoguelike()
    {
        m_cash = 0f;
        m_squadRenameNotificationPending = false;
        m_xCellSquad = new XCellSquad();
        m_xCellSquad.Init();
        m_stockHandler = new StockHandler(this);
        Enemy.SetUpEnemyTypes();
        SetupHumanBody();
        m_upgradeTree = new UpgradeTree();
        SetupEquipment();
        SetupLastGameStats();

        m_junkEquipmentLevelScale = 0.5f;
        m_junkEquipmentCostScale = 10f;
    }

    internal void LoseRougelike()
    {
        m_highscoreList.Insert(0,new Highscore(m_xCellSquad.m_name, m_humanBody.m_battlesCompleted));
        m_highscoreList.Sort(HighscoreComparison);
        
        if (m_highscoreList.Count > Highscore.maxScores)
        {
            m_highscoreList.RemoveAt(Highscore.maxScores);
        }
        ResetRoguelike();
        if (m_autoSaving)
        {
            SaveGame();
        }
        TransitionScene(eScene.mainMenu);
    }

    internal void ResetGame()
    {
        ResetRoguelike();
        m_highscoreList = new List<Highscore>();
        InitialiseTutorials();
    }

    internal void HardResetGame()
    {
        File.Delete(Application.persistentDataPath + SaveDataUtility.m_saveFileName);
        ResetGame();
    }

    void SetupLastGameStats()
    {
        m_lastGameStats = new LastGameStats();
        m_lastGameStats.m_lastGameResult = eEndGameType.lose;
        m_lastGameStats.m_xpEarnedLastGame = 0f;
        m_lastGameStats.m_invaderStrengthChangeLastGame = 0;
        m_lastGameStats.m_teamLevelAtStartOfBattle = 0;
        m_lastGameStats.m_dnaEarnedLastGame = 0f;
        m_lastGameStats.m_equipmentCollectedLastGame = 0;
        m_lastGameStats.m_lastXpBonus = 0;
        m_lastGameStats.m_lastDnaBonus = 0;
        m_lastGameStats.m_frontLineResultsPending = false;
        m_lastGameStats.m_lastFrontLinePlayerEffect = 0f;
        m_lastGameStats.m_lastFrontLineChange = 0f;
        m_lastGameStats.m_lastFrontLineEnemyEffect = 0f;
    }

    private void SetupHumanBody()
    {
        m_humanBody = new HumanBody(this);
    }

    void SetupEquipment()
    {
        m_equipmentInventory = new List<Equipment>();
        for (int i = 0; i < m_startingEquipment; i++)
        {
            m_equipmentInventory.Add(new Equipment(-1));
        }
    }

    internal Equipment AttemptToBuyJunkEquipment()
    {
        Equipment equipment = null;
        bool successfullyBought = false;
        float cost = GetJunkEquipmentCost();
        if (m_cash >= cost)
        {
            m_cash -= cost;
            successfullyBought = true;
            equipment = GenerateEquipment(true);
            PickUpEquipment(equipment);
        }
        return equipment;
    }

    public int EquipmentComparison(Equipment a_first, Equipment a_second)
    {
        int returnVal = 0;
        int[] vals = new int[2];
        for (int i = 0; i < 2; i++)
        {
            Equipment evaluatedEquipment = i == 0 ? a_first : a_second;
            vals[i] = evaluatedEquipment.GetSellValue();
            //vals[i] += 1000 * (evaluatedEquipment.m_equipped ? 1 : 0);
        }
        returnVal = vals[0] > vals[1] ? 1 : (vals[0] < vals[1] ? -1 : 0);
        return returnVal * -1;
    }

    static internal int HighscoreComparison(Highscore a_first,  Highscore a_second)
    {
        int returnVal = a_second.score - a_first.score;

        return returnVal;
    }

    internal void SortEquipmentInventory()
    {
        m_equipmentInventory.Sort(EquipmentComparison);
    }

    internal Equipment GenerateEquipment(bool a_junk = false)
    {
        float equipmentScale = a_junk ? 0.5f : 1.0f;
        float equipmentLevel = equipmentScale * m_xCellSquad.m_statHandler.m_RPGLevel.m_level;
        Equipment generatedEquipment = new Equipment(-1);
        return generatedEquipment;
    }

    internal void PickUpEquipment(Equipment a_equipment)
    {
        m_equipmentInventory.Add(a_equipment);
        SortEquipmentInventory();
    }

    internal void SellEquipment(Equipment a_equipment)
    {
        for (int i = 0; i < m_equipmentInventory.Count; i++)
        {
            if (m_equipmentInventory[i] == a_equipment)
            {
                ChangeCash(m_equipmentInventory[i].GetSellValue());
                m_equipmentInventory.RemoveAt(i);
                break;
            }
        }
    }

    internal void AttemptToRepairEquipment(Equipment a_equipmentRef)
    {
        float repairCost = a_equipmentRef.GetRepairCost();
        if (GetCurrentCash() >= repairCost)
        {
            a_equipmentRef.Repair();
            ChangeCash(-repairCost);
        }
    }

    internal void UnEquipDestroyedEquipment()
    {
        for (int i = 0; i < m_xCellSquad.m_playerXCell.m_equippedEquipment.Length; i++)
        {
            if (m_xCellSquad.m_playerXCell.m_equippedEquipment[i] != null && m_xCellSquad.m_playerXCell.m_equippedEquipment[i].IsBroken())
            {
                Equipment equipment = m_xCellSquad.m_playerXCell.m_equippedEquipment[i];
                m_xCellSquad.m_playerXCell.EquipEquipment(m_xCellSquad.m_playerXCell.m_equippedEquipment[i], i);
                m_equipmentInventory.Remove(equipment);
            }
        }
    }
  
    void KillPlayer()
    {
        for (int i = 0; i < m_xCellSquad.m_playerXCell.m_equippedEquipment.Length; i++)
        {
            for (int j = 0; j < m_equipmentInventory.Count; j++)
            {
                if (m_xCellSquad.m_playerXCell.m_equippedEquipment[i] == m_equipmentInventory[j])
                {
                    m_equipmentInventory.RemoveAt((int)j);
                    j--;
                }
            }
        }
        m_xCellSquad.KillPlayer();

    }

    public void CalculateFinishedGame()
    {
        if (m_attemptedBattleNode != null)
        {
            m_humanBody.ProgressEnemyFronts();
            m_lastGameStats.m_lastAttemptedBattleNode = m_attemptedBattleNode;

            //if (m_lastGameResult == eEndGameType.lose || m_lastGameResult == eEndGameType.escape)
            //{
            //    warfrontBalanceChange -= warfrontBalanceChangeAmount;
            //}
            float warfrontBalanceChange = PRE_BATTLE_WarfrontChange;
            m_playerWasKilledLastBattle = false;

            if (m_lastGameStats.m_lastGameResult == eEndGameType.win)
            {
                float warfrontBalanceChangeAmount = (float)m_lastGameStats.m_lastAttemptedBattleNode.m_difficulty / (float)HumanBody.m_battleMaxDifficulty;
                warfrontBalanceChangeAmount *= -warfrontBalanceChange;//Turn to percentage
                warfrontBalanceChangeAmount = Mathf.Clamp(warfrontBalanceChangeAmount, 0f, -warfrontBalanceChange);
                m_lastGameStats.m_lastAttemptedBattleNode.m_owningConnection.ChangeWarfrontBalance(warfrontBalanceChangeAmount);
                m_lastGameStats.m_lastFrontLinePlayerEffect = warfrontBalanceChangeAmount;
                //warfrontBalanceChange += warfrontBalanceChangeAmount;
            }
            else if (m_lastGameStats.m_lastGameResult == eEndGameType.lose)
            {
                m_playerWasKilledLastBattle = true;
                KillPlayer();
            }
            m_lastGameStats.m_lastFrontLineChange = m_lastGameStats.m_lastFrontLineEnemyEffect + m_lastGameStats.m_lastFrontLinePlayerEffect;

            m_humanBody.m_battlesCompleted++;
            m_humanBody.Refresh();

            for (int i = 0; i < m_xCellSquad.m_playerXCell.m_equippedEquipment.Length; i++)
            {
                Equipment equipment = m_xCellSquad.m_playerXCell.m_equippedEquipment[i];
                if (equipment != null)
                {
                    if (!equipment.IsBroken())
                    {
                        equipment.Repair();
                    }
                }
            }
            UnEquipDestroyedEquipment();
            m_xCellSquad.Refresh();
        }
    }

    internal void PassBattle()
    {
        m_humanBody.ProgressEnemyFronts();
        m_humanBody.Refresh();
    }

    internal void SceneFadeUpdate()
    {
        if (m_sceneFadeTimer != null && !m_sceneFadeTimer.m_finished)
        {
            if (m_sceneFadeTimer.Update())
            {
                if (m_sceneFadingOut)
                {
                    ChangeScene();
                }
            }
            float compPerc = m_sceneFadeTimer.GetCompletionPercentage();
            float fade = m_sceneFadingOut ? compPerc : 1f- compPerc;
            fade = Mathf.Clamp(fade, 0f, 1f);
            m_sceneFadeImageRef.color = new Color(0f, 0f, 0f, fade);
            m_audioHandlerRef.m_sceneFadeAmount = fade;
            m_audioHandlerRef.Refresh();
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.K))
        {
            m_xCellSquad.m_statHandler.m_RPGLevel.ChangeXP(1);
        }
        if (Input.GetKey(KeyCode.J))
        {
            m_cash += 1f;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            SaveGame();
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            LoadGame();
        }
        if (Input.GetKeyUp(KeyCode.O))
        {
            m_xCellSquad.m_playerXCell.m_statHandler = new CharacterStatHandler();
            m_xCellSquad.m_playerXCell.m_statHandler.Init(true);
        }
        if (Input.GetKeyUp(KeyCode.H))
        {
            PickUpEquipment(new Equipment(-1));
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            LoseRougelike();
        }
        m_stockHandler.Update();
        SceneFadeUpdate();
        //BATTLE_ShadowAngle = Mathf.Sin(Time.unscaledTime)*360f;
    }

    public void TransitionScene(eScene a_scene)
    {
        m_queuedScene = a_scene;
        m_sceneFadeTimer = new vTimer(m_sceneFadeDuration, true, true, false, false);
        m_sceneFadingOut = true;
        m_sceneFadeCanvasRef.worldCamera = FindObjectOfType<Camera>();
        m_sceneFadeCanvasRef.sortingLayerName = "UI";
        m_sceneFadeCanvasRef.sortingOrder = 20;
    }

    internal void ChangeScene()
    {
        m_currentScene = m_queuedScene;

        switch (m_queuedScene)
        {
            case eScene.mainMenu:
                SceneManager.LoadScene("Main Menu");
                break;
            case eScene.preBattle:
                SceneManager.LoadScene("Pre Battle");
                m_stockHandler.m_StocksUpdatedPtr = null;
                break;
            case eScene.battle:
                SceneManager.LoadScene("Battle");
                break;
            case eScene.postBattle:
                SceneManager.LoadScene("Post Battle");
                break;
            default:
                break;
        }
        m_sceneFadeTimer = new vTimer(m_sceneFadeDuration, true, true, false, false);
        m_sceneFadingOut = false;
    }

    public void SaveGame()
    {
        SaveDataUtility saveDataUtility =  new SaveDataUtility(this);
        saveDataUtility.Save();
    }

    public bool LoadGame()
    {
        SaveDataUtility saveDataUtility = new SaveDataUtility(this);
        bool retVal = saveDataUtility.Load();

        RefreshScanLines();
        return retVal;
    }

    static internal void AutoSaveCheck()
    {
        if (m_autoSaving)
        {
            m_staticAutoRef.SaveGame();
            //ParticleSystem test = Instantiate(m_staticAutoRef.m_explosionTestPrefab).GetComponent<ParticleSystem>();
        }
    }
}