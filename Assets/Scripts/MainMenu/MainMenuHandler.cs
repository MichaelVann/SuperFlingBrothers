using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public GameObject m_titleMenuRef;
    public GameObject m_gameSelectionMenuRef;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
    }

    public void Play()
    {
        SceneManager.LoadScene("Pre Battle");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
