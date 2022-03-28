﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
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
    } public eGameMode m_currentGameMode;

    private float m_cash = 0;
    internal StatHandler m_playerStatHandler;
    HumanBody m_humanBody;

    //Last Game
    public eEndGameType m_lastGameResult = eEndGameType.lose;
    public float m_xpEarnedLastGame = 0f;
    public float m_goldEarnedLastGame = 0f;

    //Upgrades
    public UpgradeItem[] m_upgrades;
    public UpgradeItem m_enemyVectorsUpgrade;
    public UpgradeItem m_shieldUpgrade;


    public struct Shield
    {
        public float delay;
        public float rechargeRate;
        public float capacity;

        public float value;
        public float delayTimer;
    } public Shield m_playerShield;

    StockHandler m_stockHandler;

    [Serializable]
    struct SaveData
    {
        public StatHandler statHandler;
        public List<Stock> stockList;
    }
    SaveData m_saveData;
    bool m_autoLoadDataOnLaunch = false;

    public void SetLastGameResult(eEndGameType a_value) { m_lastGameResult = a_value; }

    public void SetGameMode(eGameMode a_gameMode) { m_currentGameMode = a_gameMode; }
    public void ChangeCash(float a_change) { m_cash += a_change; }

    public float GetCurrentCash() { return m_cash; }

    internal StockHandler GetStockHandlerRef() { return m_stockHandler; }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        //Stats
        m_playerStatHandler = new StatHandler();
        m_playerStatHandler.Init();
        //m_playerStatHandler.m_stats[(int)eStatIndices.strength].effectiveValue = 1f;

        //Stocks
        m_stockHandler = new StockHandler(this);

        SetupHumanBody();
        SetupUpgrades();
        SetupShield();
        if (m_autoLoadDataOnLaunch)
        {
            LoadGame();
        }
    }

    private void SetupHumanBody()
    {
        m_humanBody = new HumanBody();
    }

    private void SetupShield()
    {
        m_playerShield.capacity = 5f;
        m_playerShield.delay = 3f;
        m_playerShield.rechargeRate = 1.6f;
    }

    void SetupUpgrades()
    {
        m_upgrades = new UpgradeItem[2];

        m_enemyVectorsUpgrade = new UpgradeItem();
        m_enemyVectorsUpgrade.SetName("Enemy Vectors");
        m_enemyVectorsUpgrade.SetDescription("Shows the direction of all enemies movement");
        m_enemyVectorsUpgrade.SetCost(50);
        m_upgrades[0] = m_enemyVectorsUpgrade;

        m_shieldUpgrade = new UpgradeItem();
        m_shieldUpgrade.SetName("Shield");
        m_shieldUpgrade.SetDescription("Enables a shield that protects the user from a limited amount of damage");
        m_shieldUpgrade.SetCost(100);
        m_upgrades[1] = m_shieldUpgrade;
    }

    internal bool AttemptToBuyUpgrade(int m_upgradeID)
    {
        UpgradeItem upgrade = m_upgrades[m_upgradeID];
        if (upgrade.m_cost <= m_cash)
        {
            m_cash -= upgrade.m_cost;
            upgrade.SetOwned(true);
            return true;
        }
        return false;
    }

    public void ChangeGameMode(int a_change)
    {
        m_currentGameMode = (eGameMode)VLib.SafeMod((int)(m_currentGameMode + a_change),(int)(eGameMode.ModeCount));
    }
  
    public void CalculateFinishedGame()
    {

    }

    void Update()
    {
        if (Input.GetKey(KeyCode.K))
        {
            m_playerStatHandler.ChangeXP(1);
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
            m_playerStatHandler = new StatHandler();
            m_playerStatHandler.Init();
        }
        m_stockHandler.Update();
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
        m_saveData.statHandler = m_playerStatHandler;
        m_saveData.stockList = m_stockHandler.m_stockList;

        string path = Application.persistentDataPath + "/Data.txt";
        string json = JsonUtility.ToJson(m_saveData);
        File.WriteAllText(path, json);
    }
    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/Data.txt";
        string loadedString = File.ReadAllText(path);
        m_saveData = JsonUtility.FromJson<SaveData>(loadedString);
        m_playerStatHandler = m_saveData.statHandler;
        m_stockHandler.m_stockList = m_saveData.stockList;
    }

}
