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
        ModeCount
    }
    public eGameMode m_currentGameMode;

    public void ChangeScore(float a_change) { m_score += a_change; }
    public void SetGameMode(eGameMode a_gameMode) { m_currentGameMode = a_gameMode; }

    void Awake()
    {
        DontDestroyOnLoad(this);
    }


    int SafeMod(int a_value, int a_mod)
    {
        return (a_value % a_mod + a_mod) % a_mod;
    }

    public void ChangeGameMode(int a_change)
    {
        m_currentGameMode = (eGameMode)SafeMod((int)(m_currentGameMode + a_change),(int)(eGameMode.ModeCount));
    }


    void Update()
    {
      
    }
}
