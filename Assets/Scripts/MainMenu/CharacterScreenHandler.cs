using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterScreenHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public Counter m_strengthCounterRef;
    public Text m_availablePointsValueTextRef;
    public Button m_reSpecButtonRef;
    public Text m_reSpecButtonTextRef;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        m_availablePointsValueTextRef.text = "" + m_gameHandlerRef.m_playerStatHandler.m_allocationPoints;
        m_availablePointsValueTextRef.color = m_gameHandlerRef.m_playerStatHandler.m_allocationPoints > 0 ? Color.yellow : Color.red;
        m_reSpecButtonRef.interactable = m_gameHandlerRef.m_playerStatHandler.m_reSpecCost <= m_gameHandlerRef.GetCurrentCash();
    }

    public void Respec()
    {
        m_gameHandlerRef.AttemptToRespec();
        m_reSpecButtonTextRef.text = "Respec \n" + m_gameHandlerRef.m_playerStatHandler.m_reSpecCost + " DNA";
        m_reSpecButtonRef.interactable = m_gameHandlerRef.m_playerStatHandler.m_reSpecCost <= m_gameHandlerRef.GetCurrentCash();
    }
}
