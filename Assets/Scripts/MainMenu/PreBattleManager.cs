using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreBattleManager : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public Text m_gameModeTextRef;

    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        UpdateGameModeText();
    }

    void UpdateGameModeText()
    {
        m_gameModeTextRef.text = m_gameHandlerRef.m_currentGameMode.ToString();
    }

    public void ChangeGameModeSelection(int a_change)
    {
        m_gameHandlerRef.ChangeGameMode(a_change);
        UpdateGameModeText();
    }

    public void Play()
    {
        SceneManager.LoadScene("Main Scene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
