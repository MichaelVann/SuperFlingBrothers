using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterScreenHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public Counter m_strengthCounterRef;
    public Counter m_cashCounterRef;
    public Text m_availablePointsValueTextRef;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        m_cashCounterRef.SetString("" + m_gameHandlerRef.m_cash);
        m_availablePointsValueTextRef.text = "" + m_gameHandlerRef.m_playerStatHandler.m_allocationPoints;
        m_availablePointsValueTextRef.color = m_gameHandlerRef.m_playerStatHandler.m_allocationPoints > 0 ? Color.yellow : Color.red;
    }
}
