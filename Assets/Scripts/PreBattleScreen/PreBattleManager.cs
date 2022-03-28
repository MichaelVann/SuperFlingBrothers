using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreBattleManager : MonoBehaviour
{
    GameHandler m_gameHandlerRef;

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



    // Update is called once per frame
    void Update()
    {
        
    }
}
