using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    public enum eGameMode
    {
        TurnLimit,
        Health,
        Pockets,
        Hunger,
        ModeCount
    }

    public eGameMode m_currentGameMode;

    internal StatHandler m_playerStatHandler;
    public int m_cash = 0;

    //Last Game
    public bool m_wonLastGame = false;
    public float m_xpEarnedLastGame = 0f;
    public float m_goldEarnedLastGame = 0f;

    struct SaveData
    {
        public StatHandler statHandler;
    }

    public UpgradeItem m_enemyVectorsUpgrade;
    public UpgradeItem[] m_upgrades;

    public void SetLastGameResult(bool a_value) { m_wonLastGame = a_value; }

    public void SetGameMode(eGameMode a_gameMode) { m_currentGameMode = a_gameMode; }
    public void ChangeCash(int a_score) { m_cash += a_score; }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        //m_playerStatHandler = gameObject.GetComponent<StatHandler>();
        m_playerStatHandler = new StatHandler();
        m_playerStatHandler.Init();
        //m_playerStatHandler.m_stats[(int)eStatIndices.strength].effectiveValue = 1f;

        m_upgrades = new UpgradeItem[1];

        m_enemyVectorsUpgrade = new UpgradeItem();
        m_enemyVectorsUpgrade.SetName("Enemy Vectors");
        m_enemyVectorsUpgrade.SetDescription("Shows the direction of all enemies movement");

        m_enemyVectorsUpgrade.SetCost(50);
        m_upgrades[0] = m_enemyVectorsUpgrade;
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
            m_cash += 1;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            SaveGame();
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            LoadGame();
        }
    }

    public void SaveGame()
    {
        string path = Application.persistentDataPath + "/Data.txt";
        File.WriteAllText(path, JsonUtility.ToJson(m_playerStatHandler));
    }
    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/Data.txt";
        string loadedString = File.ReadAllText(path);
        m_playerStatHandler = JsonUtility.FromJson<StatHandler>(loadedString);
    }

}
