﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreBattleManager : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public GameObject m_bodyPartSelectionRef;
    public GameObject m_gameModeSelectionRef;
    public Text m_battleDifficultyText;

    public enum eSubScreen
    {
        choosePart,
        zoomedOnPart,
        chooseGameMode
    } eSubScreen m_currentSubScreen;

    private void Awake()
    {
        m_currentSubScreen = eSubScreen.choosePart;
    }

    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
    }

    public void MoveToBodySelection()
    {
        m_gameModeSelectionRef.SetActive(false);
        m_bodyPartSelectionRef.SetActive(true);
    }
    public void MoveToGameSelection()
    {
        m_gameModeSelectionRef.SetActive(true);
        m_bodyPartSelectionRef.SetActive(false);
        m_battleDifficultyText.text = "" + m_bodyPartSelectionRef.GetComponent<BodyPartSelectionHandler>().m_selectedBattleNode.m_difficulty;

    }

    public void MoveToSubScreen(eSubScreen a_subScreen)
    {
        m_gameModeSelectionRef.SetActive(true);
        m_bodyPartSelectionRef.SetActive(false);
    }

    public void Play()
    {
        m_gameHandlerRef.SetBodyPartSelectedForBattle(m_bodyPartSelectionRef.GetComponent<BodyPartSelectionHandler>().m_selectedBodyPart);
        m_gameHandlerRef.SetBattleDifficulty(m_bodyPartSelectionRef.GetComponent<BodyPartSelectionHandler>().m_selectedBattleNode.m_difficulty);
        int maxEnemyDifficulty = m_bodyPartSelectionRef.GetComponent<BodyPartSelectionHandler>().m_selectedBodyPart.m_maxEnemyDifficulty;
        m_gameHandlerRef.SetMaxEnemyDifficulty(maxEnemyDifficulty);
        m_gameHandlerRef.ChangeScene(GameHandler.eScene.battle);
        //m_gameHandlerRef.m_battleAllowedEnemyTypes[(int)Enemy.eEnemyType.Idler] = true;
        //m_gameHandlerRef.m_battleAllowedEnemyTypes[(int)Enemy.eEnemyType.Striker] = true;
        //m_gameHandlerRef.m_battleAllowedEnemyTypes[(int)Enemy.eEnemyType.Dodger] = true;
    }

    public void ReturnToMainMenu()
    {
        m_gameHandlerRef.ChangeScene(GameHandler.eScene.mainMenu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
