﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreBattleManager : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public GameObject m_bodyPartSelectionRef;
    public MapHandler m_mapHandlerRef;
    public GameObject m_gameModeSelectionRef;
    public Text m_battleDifficultyText;

    [SerializeField]
    private Canvas m_menuCanvasRef;

    public GameObject m_confirmationBoxPrefab;

    public enum eSubScreen
    {
        choosePart,
        zoomedOnPart,
        chooseGameMode
    } eSubScreen m_currentSubScreen;

    private void Awake()
    {
        m_currentSubScreen = eSubScreen.choosePart;
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_gameHandlerRef.m_humanBody.m_availableBattles = 0;
        if (m_gameHandlerRef.m_humanBody.m_lost)
        {
            ConfirmationBox lostConfirmationBox = Instantiate(m_confirmationBoxPrefab,m_menuCanvasRef.transform).GetComponent<ConfirmationBox>();
            lostConfirmationBox.SetToAcknowledgeOnlyMode();
            lostConfirmationBox.SetMessageText(m_gameHandlerRef.m_humanBody.GetHumansName() + " has succumbed to the infection.");
            lostConfirmationBox.m_confirmationResponseDelegate = new ConfirmationBox.ConfirmationResponseDelegate(LoseGame);
        }
    }

    void Start()
    {


    }

    void LoseGame()
    {
        Application.Quit();
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
        m_battleDifficultyText.text = "ERROR";// "" + m_bodyPartSelectionRef.GetComponent<BodyPartSelectionHandler>().m_selectedBattleNode.m_difficulty;
    }

    public void MoveToSubScreen(eSubScreen a_subScreen)
    {
        m_gameModeSelectionRef.SetActive(true);
        m_bodyPartSelectionRef.SetActive(false);
    }

    public void Play()
    {
        //m_gameHandlerRef.SetBodyPartSelectedForBattle(m_bodyPartSelectionRef.GetComponent<BodyPartSelectionHandler>().m_selectedBodyPart);
        m_gameHandlerRef.SetBattleDifficulty(m_mapHandlerRef.m_selectedBattleNode.m_difficulty);
        int maxEnemyDifficulty = HumanBody.m_maxEnemyDifficulty;
        m_gameHandlerRef.SetMaxEnemyDifficulty(maxEnemyDifficulty);
        m_gameHandlerRef.ChangeScene(GameHandler.eScene.battle);
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
