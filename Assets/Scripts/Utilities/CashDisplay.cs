using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashDisplay : MonoBehaviour
{
    public Counter m_counterRef;
    GameHandler m_gameHandlerRef;
    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        m_counterRef.SetString("" + VLib.TruncateFloatsDecimalPlaces(m_gameHandlerRef.GetCurrentCash(), 2));
    }
}
