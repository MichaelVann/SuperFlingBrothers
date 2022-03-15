using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    Stock m_referencedStock;
    public vGraph m_graphRef;
    int m_graphDisplayedStockID = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_referencedStock = m_gameHandlerRef.m_stockList[m_graphDisplayedStockID];
        m_gameHandlerRef.m_StocksUpdatedPtr = new GameHandler.StocksUpdatedPtr(UpdateGraph);
        m_graphRef.Init(m_referencedStock.GetTrackedValues());
    }

    private void UpdateGraph()
    {
        m_graphRef.Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        //if (m_stockUpdateTimer.Update())
        //{
        //    for (int i = 0; i < m_stockList.Count; i++)
        //    {
        //        m_stockList[i].PredictNewValue();
        //    }
        //}
    }
}
