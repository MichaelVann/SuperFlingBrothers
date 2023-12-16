using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public Text m_gameModeTextRef;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        FindObjectOfType<GameHandler>().TransitionScene(GameHandler.eScene.battle);
    }

    public void Back()
    {
        FindObjectOfType<GameHandler>().TransitionScene(GameHandler.eScene.mainMenu);
    }
}
