using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public GameObject m_titleMenuRef;
    public GameObject m_characterMenuRef;
    public GameObject m_upgradeMenuRef;

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

    public void MainMenuPressed()
    {
        m_titleMenuRef.SetActive(true);
        m_characterMenuRef.SetActive(false);
        m_upgradeMenuRef.SetActive(false);
    }

    public void CharacterPressed()
    {
        m_titleMenuRef.SetActive(false);
        m_characterMenuRef.SetActive(true);

    }
    public void UpgradePressed()
    {
        m_titleMenuRef.SetActive(false);
        m_characterMenuRef.SetActive(false);
        m_upgradeMenuRef.SetActive(true);

    }
}
