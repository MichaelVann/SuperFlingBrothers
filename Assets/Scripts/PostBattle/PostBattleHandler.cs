using System.Collections;
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

    eEndGameType m_winResult = eEndGameType.lose;

    public void ContinuePressed()
    {
        FindObjectOfType<GameHandler>().ChangeScene(GameHandler.eScene.mainMenu);
    }

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        
        m_winResult = m_gameHandlerRef.m_lastGameResult;
        switch (m_winResult)
        {
            case eEndGameType.escape:
                m_resultTextRef.text ="Escaped!";
                break;
            case eEndGameType.win:
                m_resultTextRef.text ="Victory!";
                break;
            case eEndGameType.lose:
                m_resultTextRef.text = "Defeat!";
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
        m_levelsGainedTextRef.text = "" + (m_gameHandlerRef.m_playerStatHandler.m_level - m_gameHandlerRef.m_playerLevelAtStartOfBattle);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
