using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIHandler : MonoBehaviour
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

    public void StartEnding(eEndGameType a_type)
    {
        string endTextString = "";
        Color m_highlightColor = Color.white;
        Color m_bgColor = Color.white;

        switch (a_type)
        {
            case eEndGameType.retreat:
                endTextString = "Retreat!";
                m_highlightColor = Color.white;
                m_bgColor = Color.green;
                break;
            case eEndGameType.win:
                endTextString ="Victory!";
                m_highlightColor = Color.white;
                m_bgColor = Color.blue;
                break;
            case eEndGameType.lose:
                endTextString = "Defeat!";
                m_highlightColor = Color.red;
                m_bgColor = Color.black;
                break;
            default:
                break;
        }

        m_endingText.SetActive(true);
        m_endingText.GetComponent<Text>().color = m_bgColor;
        m_endingText.GetComponent<Text>().text = endTextString;
        m_endingHighlightText.text = endTextString;
        m_endingHighlightText.color = m_highlightColor;

        m_gameOver = true;
        m_playingEnding = true;
        m_endingText.SetActive(true);

        //m_battleManagerRef.CalculateFinishedGame();
    }

    public void PlayEnding()
    {
        float textScale = (m_battleManagerRef.m_gameEndTimer / m_battleManagerRef.GetMaxGameEndTimer())/ m_battleManagerRef.m_gameEndSlowdownFactor; //Mathf.Pow(m_battleManagerRef.m_gameEndTimer, 1f); 
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
