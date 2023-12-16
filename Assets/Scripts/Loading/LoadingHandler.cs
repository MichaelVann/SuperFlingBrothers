using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingHandler : MonoBehaviour
{
    void Start()
    {
        FindObjectOfType<GameHandler>().TransitionScene(GameHandler.eScene.mainMenu);
    }

    void Update()
    {
        
    }
}
