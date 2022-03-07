using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesScreenHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public Text m_cashCounterTextRef;

    public Sprite[] m_upgradeSprites;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_cashCounterTextRef.text = "" + m_gameHandlerRef.m_cash;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
