using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    BattleManager m_battleManagerRef;

    public Text m_turnsText;
    public Text m_scoreText;

    public bool m_gameOver = false;
    public GameObject m_endingText;
    public Text m_endingHighlightText;
    public bool m_playingEnding = false;

    void Awake()
    {
        m_battleManagerRef = GetComponent<BattleManager>();
    }

    public void StartEnding(bool a_won)
    {
        string endTextString = a_won ? "Victory!" : "Defeat!";
        Color m_highlightColor = a_won ? Color.white : Color.red;
        Color m_bgColor = a_won ? Color.green : Color.black;

        m_endingText.SetActive(true);
        m_endingText.GetComponent<Text>().color = m_bgColor;
        m_endingText.GetComponent<Text>().text = endTextString;
        m_endingHighlightText.text = endTextString;
        m_endingHighlightText.color = m_highlightColor;

        m_gameOver = true;
        m_playingEnding = true;
        m_endingText.SetActive(true);
    }

    public void PlayEnding()
    {
        float textScale = Mathf.Pow(m_battleManagerRef.m_gameEndTimer, 1f); //;Mathf.Pow((m_gamehandlerRef.m_gameEndTimer / m_gamehandlerRef.GetMaxGameEndTimer()),2f);
        if (textScale > 1f)
        {
            textScale = 1f;
        }
        m_endingText.transform.localScale = new Vector3(textScale, textScale, 1f);
    }

    void Update()
    {
        m_turnsText.text = "" + m_battleManagerRef.m_turnsRemaining;
        m_scoreText.text = "" + m_battleManagerRef.m_score;

        if (m_gameOver)
        {
            if (m_playingEnding)
            {
                PlayEnding();
            }
        }
    }
}
