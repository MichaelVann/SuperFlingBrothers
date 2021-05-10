using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    //UIHandler m_uiHandlerRef;

    public float m_score = 0f;
    public int m_XP = 0;
    public int m_maxXP = 100;

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

    public void ChangeGameMode(int a_change)
    {
        m_currentGameMode = (eGameMode)VLib.SafeMod((int)(m_currentGameMode + a_change),(int)(eGameMode.ModeCount));
    }
    public void ChangeXP(int a_xpReward)
    {
        m_XP += a_xpReward;
    }

    void Update()
    {
      
    }
}
