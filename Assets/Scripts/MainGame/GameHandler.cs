using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    //UIHandler m_uiHandlerRef;

    public float m_score = 0f;


    public enum eGameMode
    {
        TurnLimit,
        Health,
        Pockets,
        Hunger,
        ModeCount
    }
    public eGameMode m_currentGameMode;

    public StatHandler m_playerStatHandler;

    public void ChangeScore(float a_change) { m_score += a_change; }
    public void SetGameMode(eGameMode a_gameMode) { m_currentGameMode = a_gameMode; }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        m_playerStatHandler = gameObject.GetComponent<StatHandler>();
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
    }
}
