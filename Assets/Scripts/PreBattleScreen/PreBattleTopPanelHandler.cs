using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PreBattleTopPanelHandler : MonoBehaviour
{
    public TextMeshProUGUI m_personsNameText;
    public TextMeshProUGUI m_battleCountText;
    // Start is called before the first frame update
    void Start()
    {
        GameHandler gameHandler = FindObjectOfType<GameHandler>();
        m_personsNameText.text = gameHandler.m_humanBody.GetHumansName();
        m_battleCountText.text = "Battle Count: " + gameHandler.m_humanBody.m_battlesCompleted;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
