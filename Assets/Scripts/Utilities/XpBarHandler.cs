using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XpBarHandler : UIBar
{
    GameHandler m_gameHandlerRef;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        CharacterStatHandler statHandler = m_gameHandlerRef.m_xCellSquad.m_statHandler;
        Init(statHandler.m_RPGLevel.m_XP, statHandler.m_RPGLevel.m_maxXP);
        SetLabeltext("Level " + statHandler.m_RPGLevel.m_level);
    }
}
