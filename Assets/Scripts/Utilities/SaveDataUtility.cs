using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;

public class SaveDataUtility
{
    const string m_saveFileName = "Data2.txt";

    [Serializable]
    internal struct BattleNodeData
    {
        public int difficulty;
        public int difficultyBoostTier;
        public int minDifficulty;
        public int maxDifficulty;
        public Vector3 position;
        public bool positionCalculated;
        public bool available;

        public BattleNode.EnvironmentalEffects environmentalEffects;
    }

    [Serializable]
    struct TownConnectionData
    {
        public float warfrontBalance;
        public List<BattleNodeData> nodes;
    }

    [Serializable]
    struct TownData
    {
        public bool overrun;
        public float health;
    }

    [Serializable]
    struct HumanBodyData
    {
        public string firstName;
        public string lastName;
        public string residingTownName;
        public int battlesCompleted;
        public bool lost;
        public List<TownData> towns;
        public List<TownConnectionData> connections;
    }

    [Serializable]
    struct AudioData
    {
        public bool muted;
        public bool musicEnabled;
        public bool soundEffectsEnabled;
    }

    [Serializable]
    struct SaveData
    {
        public HumanBodyData humanBodyData;
        public AudioData audioData;
    }
    [SerializeField]
    SaveData m_saveData;

    GameHandler m_gameHandlerRef;

    string GetSaveDataPath()
    {
        return Application.persistentDataPath + "/" + m_saveFileName;
    }

    internal SaveDataUtility(GameHandler a_gameHandlerRef)
    {
        m_saveData = new SaveData();
        m_gameHandlerRef = a_gameHandlerRef;
    }

    void SaveHumanBody()
    {
        m_saveData.humanBodyData.firstName = m_gameHandlerRef.m_humanBody.m_firstName;
        m_saveData.humanBodyData.lastName = m_gameHandlerRef.m_humanBody.m_lastName;
        m_saveData.humanBodyData.residingTownName = m_gameHandlerRef.m_humanBody.m_playerResidingTown.m_name;
        m_saveData.humanBodyData.battlesCompleted = m_gameHandlerRef.m_humanBody.m_battlesCompleted;
        m_saveData.humanBodyData.lost = m_gameHandlerRef.m_humanBody.m_lost;

        //Towns
        m_saveData.humanBodyData.towns = new List<TownData>();
        for (int i = 0; i < m_gameHandlerRef.m_humanBody.m_towns.Count; i++)
        {
            TownData townData = new TownData();
            Town town = m_gameHandlerRef.m_humanBody.m_towns[i];
            townData.health = town.m_health;
            townData.overrun = town.m_overrun;
            m_saveData.humanBodyData.towns.Add(townData);
        }

        //Connections
        m_saveData.humanBodyData.connections = new List<TownConnectionData>();
        for (int i = 0; i < m_gameHandlerRef.m_humanBody.m_townConnections.Count; i++)
        {
            TownConnectionData connectionData = new TownConnectionData();
            TownConnection connection= m_gameHandlerRef.m_humanBody.m_townConnections[i];
            connectionData.warfrontBalance = connection.m_warfrontBalance;
            connectionData.nodes = new List<BattleNodeData>();
            //Battles
            for (int j = 0; j < connection.m_battles.Count; j++)
            {
                BattleNodeData battleData = new BattleNodeData();
                BattleNode battleNode = connection.m_battles[j];
                battleData.difficulty = battleNode.m_difficulty;
                battleData.difficultyBoostTier = battleNode.m_difficultyBoostTier;
                battleData.position = battleNode.m_position;
                battleData.positionCalculated = battleNode.m_positionCalculated;
                battleData.available = battleNode.m_available;
                battleData.environmentalEffects = battleNode.m_environmentalEffects;
                connectionData.nodes.Add(battleData);
            }
            m_saveData.humanBodyData.connections.Add(connectionData);
        }
    }

    void LoadHumanBody()
    {
        m_gameHandlerRef.m_humanBody.m_firstName = m_saveData.humanBodyData.firstName;
        m_gameHandlerRef.m_humanBody.m_lastName = m_saveData.humanBodyData.lastName;
        m_gameHandlerRef.m_humanBody.m_playerResidingTown = m_gameHandlerRef.m_humanBody.FindTownByName(m_saveData.humanBodyData.residingTownName);
        m_gameHandlerRef.m_humanBody.m_battlesCompleted = m_saveData.humanBodyData.battlesCompleted;
        m_gameHandlerRef.m_humanBody.m_lost = m_saveData.humanBodyData.lost;

        //Towns
        for (int i = 0; i < m_saveData.humanBodyData.towns.Count; i++)
        {
            Town town = m_gameHandlerRef.m_humanBody.m_towns[i];
            TownData townData = m_saveData.humanBodyData.towns[i];
            town.m_health = townData.health;
            town.m_overrun = townData.overrun;
        }

        //Connections
        for (int i = 0; i < m_saveData.humanBodyData.connections.Count; i++)
        {
            TownConnection connection = m_gameHandlerRef.m_humanBody.m_townConnections[i];
            TownConnectionData connectionData = m_saveData.humanBodyData.connections[i];
            connection.m_battles = new List<BattleNode>();
            for (int j = 0; j < connectionData.nodes.Count; j++)
            {
                BattleNode battleNode = new BattleNode(connectionData.nodes[j], connection);
                connection.m_battles.Add(battleNode);
                connection.m_warfrontBalance = connectionData.warfrontBalance;
            }
        }
        
    }

    void SaveAudioData()
    {
        m_saveData.audioData.muted = m_gameHandlerRef.m_musicPlayerRef.m_musicEnabled;
        m_saveData.audioData.musicEnabled = m_gameHandlerRef.m_musicPlayerRef.m_musicEnabled;
        m_saveData.audioData.soundEffectsEnabled = m_gameHandlerRef.m_musicPlayerRef.m_soundEffectsEnabled;
    }

    void LoadAudioData()
    {
        m_gameHandlerRef.m_musicPlayerRef.m_muted = m_saveData.audioData.muted;
        m_gameHandlerRef.m_musicPlayerRef.m_musicEnabled = m_saveData.audioData.musicEnabled;
        m_gameHandlerRef.m_musicPlayerRef.m_soundEffectsEnabled = m_saveData.audioData.soundEffectsEnabled;
    }

    internal void Save()
    {
        SaveHumanBody();
        SaveAudioData();
        string path = GetSaveDataPath();
        string json = JsonUtility.ToJson(m_saveData);
        File.WriteAllText(path, json);
    }

    internal bool Load()
    {
        bool retVal = false;
        string path = GetSaveDataPath();
        if (File.Exists(path))
        {
            string loadedString = File.ReadAllText(path);
            m_saveData = JsonUtility.FromJson<SaveData>(loadedString);
            LoadHumanBody();
            LoadAudioData();
            retVal = true;
        }
        else
        {
            //Throw up window saying no save found
        }
        return retVal;
    }
}
