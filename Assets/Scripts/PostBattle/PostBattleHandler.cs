﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PostBattleHandler : MonoBehaviour
{
    public GameHandler m_gameHandlerRef;
    public Text m_resultTextRef;
    public Text m_goldCollectedTextRef;
    public Text m_levelsGainedTextRef;
    public Text m_xpGainedTextRef;
    public RollingText m_totalGoldTextRef;

    public GameObject m_winBonusRef;
    public Text m_XPBonusText;
    public Text m_goldBonusText;
    public Text m_equipmentCollectedText;

    [SerializeField] CharacterSkillsDigest m_characterSkillsDigestRef;
    [SerializeField] Button m_continueButtonRef;

    eEndGameType m_winResult = eEndGameType.lose;

    public void ContinuePressed()
    {
        FindObjectOfType<GameHandler>().TransitionScene(GameHandler.eScene.mainMenu);
    }

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        
        m_winResult = m_gameHandlerRef.m_lastGameResult;
        m_winBonusRef.SetActive(m_winResult == eEndGameType.win);
        m_gameHandlerRef.m_frontLineResultsPending = true;
        switch (m_winResult)
        {
            case eEndGameType.retreat:
                m_resultTextRef.text ="Retreat";
                break;
            case eEndGameType.win:
                m_resultTextRef.text ="Victory";
                //m_gameHandlerRef.m_lastXpBonus = m_gameHandlerRef.m_lastDnaBonus = Mathf.RoundToInt(Mathf.Pow(m_gameHandlerRef.m_battleDifficulty, 1.1f));
                float bonusMult = m_gameHandlerRef.GetBattleDifficultyBonus();
                m_gameHandlerRef.m_lastXpBonus = (int)(m_gameHandlerRef.m_xpEarnedLastGame * bonusMult);
                //m_gameHandlerRef.m_lastDnaBonus = (int)(m_gameHandlerRef.m_dnaEarnedLastGame);
                //m_gameHandlerRef.m_dnaEarnedLastGame += m_gameHandlerRef.m_lastDnaBonus;
                //m_goldBonusText.text = "" + m_gameHandlerRef.m_lastDnaBonus + "(x" + (1 + bonusMult) + ")";
                m_gameHandlerRef.m_xpEarnedLastGame += m_gameHandlerRef.m_lastXpBonus;
                m_gameHandlerRef.m_xCellSquad.m_statHandler.m_RPGLevel.ChangeXP(m_gameHandlerRef.m_lastXpBonus);

                m_XPBonusText.text = "" + m_gameHandlerRef.m_lastXpBonus + "(x" + (1 + bonusMult) + ")";

                break;
            case eEndGameType.lose:
                m_resultTextRef.text = "Defeat";
                break;
            default:
                break;
        }

        m_goldCollectedTextRef.text = "" + (float)m_gameHandlerRef.m_dnaEarnedLastGame;
        //m_goldCollectedTextRef.SetDesiredValue(0);
        m_totalGoldTextRef.SetCurrentValue(m_gameHandlerRef.GetCurrentCash());
        m_gameHandlerRef.ChangeCash(m_gameHandlerRef.m_dnaEarnedLastGame);
        m_totalGoldTextRef.SetDesiredValue(m_gameHandlerRef.GetCurrentCash());
        m_xpGainedTextRef.text = "" + m_gameHandlerRef.m_xpEarnedLastGame;
        m_levelsGainedTextRef.text = "" + (m_gameHandlerRef.m_xCellSquad.m_statHandler.m_RPGLevel.m_level - m_gameHandlerRef.m_teamLevelAtStartOfBattle);
        m_equipmentCollectedText.text = "" + m_gameHandlerRef.m_equipmentCollectedLastGame;
        m_gameHandlerRef.UnEquipDestroyedEquipment();
        m_gameHandlerRef.m_audioHandlerRef.PlayMenuMusic();
    }

    // Update is called once per frame
    void Update()
    {
        m_continueButtonRef.interactable = m_characterSkillsDigestRef.SkillBarsFinishedAnimating();
    }
}
