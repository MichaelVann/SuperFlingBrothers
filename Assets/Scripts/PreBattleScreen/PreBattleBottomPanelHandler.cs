using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PreBattleBottomPanelHandler : MonoBehaviour
{
    public GameObject m_readoutRef;
    public GameObject m_passPanelRef;
    GameHandler m_gameHandlerRef;
    bool inited = false;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!inited)
        {
            bool needToPass = m_gameHandlerRef.m_humanBody.m_availableBattles <= 0;
            m_readoutRef.gameObject.SetActive(!needToPass);
            m_passPanelRef.gameObject.SetActive(needToPass);
            inited = true;
        }
    }

    public void PassButtonPressed()
    {
        FindObjectOfType<GameHandler>().PassBattle();
        m_gameHandlerRef.ChangeScene(GameHandler.eScene.preBattle);

    }
}
