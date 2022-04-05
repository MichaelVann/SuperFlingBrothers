using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreBattleManager : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public GameObject m_bodyPartSelectionRef;
    public GameObject m_gameModeSelectionRef;


    enum SubScreen
    {
        choosePart,
        zoomedOnPart,
        chooseGameMode
    } SubScreen m_currentSubScreen;

    private void Awake()
    {
        m_currentSubScreen = SubScreen.choosePart;
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
    }

    public void Play()
    {
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
