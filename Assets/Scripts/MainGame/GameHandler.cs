﻿using System;
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

    //UpgradeItem

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
        //File.Create(path);
        File.WriteAllText(path, JsonUtility.ToJson(m_playerStatHandler));

        //SaveData data = new SaveData();
        //data.statHandler = m_playerStatHandler;

        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Create(Application.persistentDataPath + "/BrainData.dat");
        //bf.Serialize(file, data);
        //file.Close();
    }
    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/Data.txt";
        
        //m_playerStatHandler = JsonUtility.<StatHandler>(File.ReadAllText(path));
        string loadedString = File.ReadAllText(path);
        m_playerStatHandler = JsonUtility.FromJson<StatHandler>(loadedString);
        //m_playerStatHandler = JsonUtility.FromJson<StatHandler>(File.ReadAllText(path));
    }

}
