using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScreenHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public Counter m_strengthCounterRef;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        m_strengthCounterRef.SetString("" + m_gameHandlerRef.m_playerStatHandler.m_stats[(int)eStatIndices.strength].value);
    }
}
