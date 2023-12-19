using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PreBattleTopPanelHandler : MonoBehaviour
{
    public TextMeshProUGUI m_personsNameText;
    public TextMeshProUGUI m_battlesEnteredText;
    public TextMeshProUGUI m_battlesAvailableText;
    // Start is called before the first frame update
    void Start()
    {
        GameHandler gameHandler = FindObjectOfType<GameHandler>();
        m_personsNameText.text = gameHandler.m_humanBody.GetHumansName();
        m_battlesEnteredText.text = "Day: " + gameHandler.m_humanBody.m_battlesCompleted;
        m_battlesAvailableText.text = "Available Battles: " + gameHandler.m_humanBody.m_availableBattles;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
