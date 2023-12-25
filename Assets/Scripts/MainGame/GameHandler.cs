using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static GameHandler;
using static UnityEngine.UI.CanvasScaler;

[Serializable]

public class GameHandler : MonoBehaviour
{
    internal static GameHandler m_staticAutoRef;

    public const int MAIN_VERSION_NUMBER = 28;
    public const int SUB_VERSION_NUMBER = 9;


    // -- BALANCE VARIABLES --

    static internal float GAME_enemyXPRewardScale = 3f;
    static internal float BATTLE_CoinValue = 1f;
    static internal float BATTLE_ShadowAngle = 135f;
    static internal float BATTLE_FlingStrength = 320f;
    static internal float BATTLE_SkillXPScale = 2f;
    static internal float BATTLE_EnemyEquipmentDropChanceScale = 0.5f;
    internal const int    BATTLE_EnemySpawnPointCount = 19;

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
    [SerializeField] Canvas m_sceneFadeCanvasRef;
    vTimer m_sceneFadeTimer;
    bool m_sceneFadingOut;
    const float m_sceneFadeDuration = 0.35f;

    // Dialogue
    internal const string m_speakerCharacterName = "Micky";// Sergeant Geras";
    [SerializeField] GameObject m_dialogBoxPrefab;
    internal bool m_firstTimeSquadOverview = true;

    //GAME
    private float m_cash;
    
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

    StockHandler m_stockHandler;

    [Serializable]
    struct SaveData
    {
        public float cash;
        public XCellSquad xCellTeam;
        public XCell xCell;
        //public HumanBody humanBody;
        public List<Stock> stockList;
        public UpgradeItem[] upgrades;
        public List<Equipment> equipmentList;
        public string bodyFirstName;
        public string bodyLastName;
    }
    SaveData m_saveData;
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
    }
    internal List<Highscore> m_highscoreList;

    [SerializeField] GameObject m_explosionTestPrefab;

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
        DontDestroyOnLoad(gameObject);
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

    internal void CreateDialogBox(string a_speakerName, List<string> a_dialogs)
    {
        DialogBox dialogBox = Instantiate(m_dialogBoxPrefab, m_sceneFadeCanvasRef.transform).GetComponentInChildren<DialogBox>();
        dialogBox.Init(a_speakerName);
        dialogBox.AddDialogs(a_dialogs);
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
        
        if (m_highscoreList.Count > 9)
        {
            m_highscoreList.RemoveAt(9);
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
        m_firstTimeSquadOverview = true;
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

    int HighscoreComparison(Highscore a_first,  Highscore a_second)
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
                m_xCellSquad.m_playerXCell.EquipEquipment(m_xCellSquad.m_playerXCell.m_equippedEquipment[i], i);
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
        m_saveData.cash = m_cash;
        m_saveData.xCellTeam = m_xCellSquad;
        //m_saveData.humanBody = m_humanBody;
        //m_saveData.statHandler.Copy(m_playerStatHandler);
        m_saveData.stockList = m_stockHandler.m_stockList;
        //m_saveData.upgrades = m_upgrades;
        m_saveData.equipmentList = m_equipmentInventory;
        m_saveData.bodyFirstName = m_humanBody.m_firstName;
        m_saveData.bodyLastName = m_humanBody.m_lastName;

        string path = Application.persistentDataPath + "/Data.txt";
        string json = JsonUtility.ToJson(m_saveData);
        File.WriteAllText(path, json);
        SaveDataUtility saveDataUtility =  new SaveDataUtility(this);
        saveDataUtility.Save();
    }

    public bool LoadGame()
    {
        bool retVal = true;

        string path = Application.persistentDataPath + "/Data.txt";
        if (File.Exists(path))
        {
            string loadedString = File.ReadAllText(path);
            m_saveData = JsonUtility.FromJson<SaveData>(loadedString);
            m_cash = m_saveData.cash;
            m_xCellSquad = m_saveData.xCellTeam;
            //m_humanBody = m_saveData.humanBody;
            m_humanBody.m_gameHandlerRef = this;
            //m_playerStatHandler = m_saveData.statHandler;
            for (int i = 0; i < m_stockHandler.m_stockList.Count; i++)
            {
                m_stockHandler.m_stockList[i].CopyValues(m_saveData.stockList[i]);
            }
            //m_humanBody.m_firstName = m_saveData.bodyFirstName;
            //m_humanBody.m_lastName = m_saveData.bodyLastName;

            //for (int i = 0; i < m_upgrades.Length; i++)
            //{
            //    m_upgrades[i].Copy(m_saveData.upgrades[i]);
            //}

            for (int i = 0; i < m_xCellSquad.m_playerXCell.m_equippedEquipment.Length; i++)
            {
                m_xCellSquad.m_playerXCell.m_equippedEquipment[i] = null;
            }

            m_equipmentInventory = new List<Equipment>();

            for (int i = 0; i < m_saveData.equipmentList.Count; i++)
            {
                m_equipmentInventory.Add(m_saveData.equipmentList[i]);
                m_saveData.equipmentList[i].ResetAbilitysParent();
                if (m_equipmentInventory[i].m_equipped)
                {
                    m_xCellSquad.m_playerXCell.m_equippedEquipment[m_equipmentInventory[i].m_equippedSlotId] = m_equipmentInventory[i];
                }
            }
        }
        else
        {
            retVal = false;
        }
        
        SaveDataUtility saveDataUtility = new SaveDataUtility(this);
        if (!saveDataUtility.Load())
        {
            retVal = false;
        }
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