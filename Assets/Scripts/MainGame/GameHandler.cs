using System;
using System.Collections;
using System.Collections.Generic;
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

    public StatHandler m_playerStatHandler;

    public void SetGameMode(eGameMode a_gameMode) { m_currentGameMode = a_gameMode; }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        m_playerStatHandler = gameObject.GetComponent<StatHandler>();
        m_playerStatHandler.Init();
        m_playerStatHandler.SetStatValue(eStatIndices.flingStrength,2f);
        m_playerStatHandler.m_stats[(int)eStatIndices.strength].effectiveValue = 1f;
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
