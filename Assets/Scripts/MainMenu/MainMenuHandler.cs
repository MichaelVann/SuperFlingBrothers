using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    GameObject m_titleMenuRef;
    GameObject m_characterMenuRef;
    GameObject m_upgradeMenuRef;
    GameObject m_stockMenuRef;

    public List<GameObject> m_menuList;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_gameHandlerRef.m_audioHandlerRef.PlayMenuMusic();
    }

    public void Play()
    {
        FindObjectOfType<GameHandler>().TransitionScene(GameHandler.eScene.preBattle);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenMenu(int a_index)
    {
        for (int i = 0; i < m_menuList.Count; i++)
        {
            m_menuList[i].SetActive(a_index == i);
        }
    }
}
