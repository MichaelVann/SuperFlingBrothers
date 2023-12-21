using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PreBattleBottomPanelHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public GameObject m_readoutRef;
    public GameObject m_passPanelRef;
    [SerializeField] TextMeshProUGUI m_townsRemainingValueTextRef;
    [SerializeField] TextMeshProUGUI m_townsRemainingMaxTextRef;
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
            int friendlyTownCount = m_gameHandlerRef.m_humanBody.GetFriendlyTownCount();
            int totalTownCount = m_gameHandlerRef.m_humanBody.GetTownCount();
            m_townsRemainingValueTextRef.text = "" + friendlyTownCount;
            m_townsRemainingValueTextRef.color = VLib.RatioToColorRGB((float)(friendlyTownCount) / (float)(totalTownCount));
            m_townsRemainingMaxTextRef.text = "/" + totalTownCount;
        }
    }

    public void PassButtonPressed()
    {
        FindObjectOfType<GameHandler>().PassBattle();
        m_gameHandlerRef.TransitionScene(GameHandler.eScene.preBattle);

    }
}
