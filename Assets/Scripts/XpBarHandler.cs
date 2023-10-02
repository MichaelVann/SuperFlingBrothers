using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XpBarHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;

    float m_xpBarLength = 650f;
    float m_xpBarHeight = 25f;

    public RectTransform m_XPBarMaskTransform;
    public Text m_XPBarValueText;
    public Text m_levelText;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        CharacterStatHandler statHandler = m_gameHandlerRef.m_xCellTeam.m_statHandler;
        m_XPBarMaskTransform.sizeDelta = new Vector2(m_xpBarLength * ((float)statHandler.m_RPGLevel.m_XP / (float)statHandler.m_RPGLevel.m_maxXP), m_xpBarHeight);

        m_XPBarValueText.text = "" + statHandler.m_RPGLevel.m_XP + " / " + statHandler.m_RPGLevel.m_maxXP;
        m_levelText.text = "Level " + statHandler.m_RPGLevel.m_level;
    }
}
