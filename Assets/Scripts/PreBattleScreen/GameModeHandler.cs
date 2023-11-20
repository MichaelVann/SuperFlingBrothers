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
        FindObjectOfType<GameHandler>().ChangeScene(GameHandler.eScene.battle);
    }

    public void Back()
    {
        FindObjectOfType<GameHandler>().ChangeScene(GameHandler.eScene.mainMenu);
    }
}
