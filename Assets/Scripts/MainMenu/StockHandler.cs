using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockHandler : MonoBehaviour
{
    List<Stock> m_stockList;
    vTimer m_stockUpdateTimer;
    public vGraph m_graphRef;
    int m_displayedGraphID = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_stockList = new List<Stock>();
        m_stockList.Add(new Stock());
        m_stockUpdateTimer = new vTimer(1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_stockUpdateTimer.Update())
        {
            for (int i = 0; i < m_stockList.Count; i++)
            {
                m_stockList[i].PredictNewValue();
            }
            m_graphRef.AddValue(m_stockList[m_displayedGraphID].GetCurrentValue());
        }
    }
}
