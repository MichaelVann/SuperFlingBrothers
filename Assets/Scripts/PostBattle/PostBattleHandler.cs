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
    public RollingText m_totalGoldTextRef;

    bool m_winResult = false;

    public void ContinuePressed()
    {
        SceneManager.LoadScene("Main Menu");
    }

    private void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        
        m_winResult = m_gameHandlerRef.m_wonLastGame;
        m_resultTextRef.text = m_winResult ? "Victory!" : "Defeat!";
        m_goldCollectedTextRef.text = "" + (int)m_gameHandlerRef.m_goldEarnedLastGame;
        //m_goldCollectedTextRef.SetDesiredValue(0);
        m_totalGoldTextRef.SetCurrentValue((int)m_gameHandlerRef.m_cash);
        m_gameHandlerRef.ChangeCash((int)m_gameHandlerRef.m_goldEarnedLastGame);
        m_totalGoldTextRef.SetDesiredValue((int)m_gameHandlerRef.m_cash);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
