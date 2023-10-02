﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameHandler;
using static UnityEngine.UI.CanvasScaler;

[Serializable]

public class GameHandler : MonoBehaviour
{
    public const float _VERSION_NUMBER = 20.3f;

    static internal bool DEBUG_MODE = true;

    // -- BALANCE VARIABLES --

    static internal float GAME_enemyXPRewardScale = 20f;
    static internal float BATTLE_CoinValue = 1f;
    static internal float BATTLE_ShadowAngle = 135f;
    //Damageables
    static internal float DAMAGEABLE_defaultMass = 1f;
    static internal float DAMAGEABLE_bumpFlingStrengthMult = 0.25f;
    static internal float DAMAGEABLE_pocketFlingStrength = 100f;
    static internal float DAMAGEABLE_damagePerSpeedDivider = 8f;
    // -- END OF BALANCE --

    public enum eScene
    {
        mainMenu,
        preBattle,
        battle,
        postBattle
    } eScene m_currentScene;

    public enum eGameMode
    {
        TurnLimit,
        Health,
        Pockets,
        Hunger,
        ModeCount
    }

    public eGameMode m_currentGameMode;

    private float m_cash = 0;
    
    //Player

    internal XCellTeam m_xCellTeam;
    bool m_playerWasKilledLastBattle = false;

    //Body
    public HumanBody m_humanBody;

    //Current Battle
    public BattleNode m_attemptedBattleNode;
    internal int m_battleDifficulty = 2;
    internal int m_maxEnemyDifficulty; //Set from humanbody

    //Last Game
    public BattleNode m_lastAttemptedBattleNode;
    public eEndGameType m_lastGameResult = eEndGameType.lose;
    public float m_xpEarnedLastGame = 0f;
    public int m_invaderStrengthChangeLastGame = 0;
    public int m_teamLevelAtStartOfBattle = 0;
    public float m_dnaEarnedLastGame = 0f;
    public int m_equipmentCollectedLastGame = 0;
    public int m_lastXpBonus = 0;
    public int m_lastDnaBonus = 0;

    //Upgrades
    public UpgradeItem[] m_upgrades;
    public enum UpgradeId
    {
        enemyVector,
        playerVector,
        Count
    }

    public List<Equipment> m_equipmentInventory;

    StockHandler m_stockHandler;

    [Serializable]
    struct SaveData
    {
        public float cash;
        public XCell xCell;
        public List<Stock> stockList;
        public UpgradeItem[] upgrades;
        public List<Equipment> equipmentList;
        public string bodyFirstName;
        public string bodyLastName;
    }
    SaveData m_saveData;
    bool m_autoLoadDataOnLaunch = false;

    internal void AttemptToRespec()
    {
        if (m_xCellTeam.m_statHandler.m_reSpecCost <= m_cash)
        {
            m_cash -= m_xCellTeam.m_statHandler.m_reSpecCost;
            m_xCellTeam.m_statHandler.ReSpec();
        }
    }

    public void SetLastGameResult(eEndGameType a_value) { m_lastGameResult = a_value; }

    public void SetGameMode(eGameMode a_gameMode) { m_currentGameMode = a_gameMode; }
    public void ChangeCash(float a_change) { m_cash += a_change; }

    public void SetSelectedBattle(BattleNode a_battleNode) { m_attemptedBattleNode = a_battleNode; }

    public float GetCurrentCash() { return m_cash; }

    public void SetBattleDifficulty(int a_difficulty) { m_battleDifficulty = a_difficulty; }
    public void SetMaxEnemyDifficulty(int a_difficulty) { m_maxEnemyDifficulty = a_difficulty; }

    internal StockHandler GetStockHandlerRef() { return m_stockHandler; }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        m_xCellTeam = new XCellTeam();

        //Stocks
        m_stockHandler = new StockHandler(this);

        SetupHumanBody();

        SetupUpgrades();
        SetupEquipment();
        Enemy.SetUpEnemyTypes();
        if (m_autoLoadDataOnLaunch)
        {
            LoadGame();
        }

        //Battle
    }

    private void SetupHumanBody()
    {
        m_humanBody = new HumanBody(this);
    }

    void SetupUpgrades()
    {
        m_upgrades = new UpgradeItem[(int)UpgradeId.Count];

        m_upgrades[(int)UpgradeId.enemyVector] = new UpgradeItem();
        m_upgrades[(int)UpgradeId.enemyVector].SetName("Enemy Vectors");
        m_upgrades[(int)UpgradeId.enemyVector].SetDescription("Shows the direction of all enemies movement.");
        m_upgrades[(int)UpgradeId.enemyVector].SetCost(30);

        m_upgrades[(int)UpgradeId.playerVector] = new UpgradeItem();
        m_upgrades[(int)UpgradeId.playerVector].SetName("Player Vector");
        m_upgrades[(int)UpgradeId.playerVector].SetDescription("Shows the direction of player movement.");
        m_upgrades[(int)UpgradeId.playerVector].SetCost(20);
    }

    void SetupEquipment()
    {
        m_equipmentInventory = new List<Equipment>();
        for (int i = 0; i < 3; i++)
        {
            m_equipmentInventory.Add(new Equipment(0));
        }
        //Equipment test = m_equipmentInventory[0];
        //test.m_level = 5;
    }

    internal bool AttemptToBuyUpgrade(int a_upgradeID)
    {
        bool returnValue = false;
        UpgradeItem upgrade = m_upgrades[a_upgradeID];
        if (upgrade.m_cost <= m_cash)
        {
            m_cash -= upgrade.m_cost;
            upgrade.m_level++;
            upgrade.m_cost *= upgrade.m_costScaling;
            if (!upgrade.m_owned)
            {
                upgrade.SetOwned(true);
            }
            returnValue =  true;
        }
        return returnValue;
    }

    public int EquipmentComparison(Equipment a_first, Equipment a_second)
    {
        int returnVal = 0;
        int[] vals = new int[2];
        for (int i = 0; i < 2; i++)
        {
            Equipment evaluatedEquipment = i == 0 ? a_first : a_second;
            vals[i] = evaluatedEquipment.m_goldValue;
            vals[i] += 1000 * (evaluatedEquipment.m_equipped ? 1 : 0);
        }
        returnVal = vals[0] > vals[1] ? 1 : (vals[0] < vals[1] ? -1 : 0);
        return returnVal * -1;
    }

    internal void SortEquipmentInventory()
    {
        m_equipmentInventory.Sort(EquipmentComparison);
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
                ChangeCash(m_equipmentInventory[i].m_goldValue);
                m_equipmentInventory.RemoveAt(i);
                break;
            }
        }
    }

    public void ChangeGameMode(int a_change)
    {
        m_currentGameMode = (eGameMode)VLib.SafeMod((int)(m_currentGameMode + a_change),(int)(eGameMode.ModeCount));
    }
  
    void KillPlayer()
    {
        for (int i = 0; i < m_xCellTeam.m_playerXCell.m_equippedEquipment.Length; i++)
        {
            for (int j = 0; j < m_equipmentInventory.Count; j++)
            {
                if (m_xCellTeam.m_playerXCell.m_equippedEquipment[i] == m_equipmentInventory[j])
                {
                    m_equipmentInventory.RemoveAt((int)j);
                    j--;
                }
            }
        }
        m_xCellTeam.m_playerXCell = new XCell();
    }

    public void CalculateFinishedGame()
    {
        if (m_attemptedBattleNode != null)
        {
            m_lastAttemptedBattleNode = m_attemptedBattleNode;

            //if (m_lastGameResult == eEndGameType.lose || m_lastGameResult == eEndGameType.escape)
            //{
            //    warfrontBalanceChange -= warfrontBalanceChangeAmount;
            //}
            float warfrontBalanceChange = -0.05f;
            m_playerWasKilledLastBattle = false;

            if (m_lastGameResult == eEndGameType.win)
            {
                float warfrontBalanceChangeAmount = (float)m_lastAttemptedBattleNode.m_difficulty / (float)m_humanBody.m_battleMaxDifficulty;
                warfrontBalanceChangeAmount *= -warfrontBalanceChange;//Turn to percentage
                warfrontBalanceChange += warfrontBalanceChangeAmount;
            }
            else if (m_lastGameResult == eEndGameType.lose)
            {
                m_playerWasKilledLastBattle = true;
                KillPlayer();
            }

            m_lastAttemptedBattleNode.m_owningConnection.ChangeWarfrontBalance(warfrontBalanceChange);
            m_humanBody.m_battlesCompleted++;
            m_humanBody.Refresh();
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.K))
        {
            m_xCellTeam.m_statHandler.m_RPGLevel.ChangeXP(1);
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
            m_xCellTeam.m_playerXCell.m_statHandler = new CharacterStatHandler();
            m_xCellTeam.m_playerXCell.m_statHandler.Init();
        }
        if (Input.GetKeyUp(KeyCode.H))
        {
            PickUpEquipment(new Equipment(m_xCellTeam.m_playerXCell.m_statHandler.m_RPGLevel.m_level));
        }
        m_stockHandler.Update();
        //BATTLE_ShadowAngle = Mathf.Sin(Time.unscaledTime)*360f;
    }

    public void ChangeScene(eScene a_scene)
    {
        m_currentScene = a_scene;
        switch (a_scene)
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
    }

    public void SaveGame()
    {
        //test[0] = new Test();
        //test[0].x = 5;

        m_saveData.cash = m_cash;
        m_saveData.xCell = m_xCellTeam.m_playerXCell;
        //m_saveData.statHandler.Copy(m_playerStatHandler);
        m_saveData.stockList = m_stockHandler.m_stockList;
        m_saveData.upgrades = m_upgrades;
        m_saveData.equipmentList = m_equipmentInventory;
        m_saveData.bodyFirstName = m_humanBody.m_firstName;
        m_saveData.bodyLastName = m_humanBody.m_lastName;

        string path = Application.persistentDataPath + "/Data.txt";
        string json = JsonUtility.ToJson(m_saveData);
        File.WriteAllText(path, json);
    }
    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/Data.txt";
        string loadedString = File.ReadAllText(path);
        m_saveData = JsonUtility.FromJson<SaveData>(loadedString);
        m_cash = m_saveData.cash;
        m_xCellTeam.m_playerXCell = m_saveData.xCell;
        //m_playerStatHandler = m_saveData.statHandler;
        for (int i = 0; i < m_stockHandler.m_stockList.Count; i++)
        {
            m_stockHandler.m_stockList[i].CopyValues(m_saveData.stockList[i]);
        }
        m_humanBody.m_firstName = m_saveData.bodyFirstName;
        m_humanBody.m_lastName = m_saveData.bodyLastName;

        for (int i = 0; i < m_upgrades.Length; i++)
        {
            m_upgrades[i].Copy(m_saveData.upgrades[i]);
        }

        for (int i = 0; i < m_xCellTeam.m_playerXCell.m_equippedEquipment.Length; i++)
        {
            m_xCellTeam.m_playerXCell.m_equippedEquipment[i] = null;
        }

        m_equipmentInventory = new List<Equipment>();

        for (int i = 0; i < m_equipmentInventory.Count; i++)
        {
            m_equipmentInventory.Add(m_saveData.equipmentList[i]);
            if (m_equipmentInventory[i].m_equipped)
            {
                m_xCellTeam.m_playerXCell.m_equippedEquipment[m_equipmentInventory[i].m_equippedSlotId] = m_equipmentInventory[i];
            }
        }
    }
}