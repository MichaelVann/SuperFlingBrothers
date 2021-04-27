using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    GameHandler m_gamehandlerRef;

    public Text m_turnsText;
    public Text m_scoreText;

    void Awake()
    {
        m_gamehandlerRef = GetComponent<GameHandler>();
    }

    void Update()
    {
        m_turnsText.text = "" + m_gamehandlerRef.m_turnsRemaining;
        m_scoreText.text = "" + m_gamehandlerRef.m_score;
    }
}
