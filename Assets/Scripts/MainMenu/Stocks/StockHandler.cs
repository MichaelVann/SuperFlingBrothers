using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockHandler
{
    GameHandler m_gameHandlerRef;

    internal List<Stock> m_stockList;
    vTimer m_stockUpdateTimer;
    public delegate void StocksUpdatedPtr();
    public StocksUpdatedPtr m_StocksUpdatedPtr;

    // Start is called before the first frame update
    public StockHandler(GameHandler a_gameHandlerRef)
    {
        m_gameHandlerRef = a_gameHandlerRef;
        SetupStocks();
    }


    private void SetupStocks()
    {
        m_stockList = new List<Stock>();
        m_stockList.Add(new Stock(VLib.GenerateName()));
        m_stockList.Add(new Stock(VLib.GenerateName()));
        m_stockUpdateTimer = new vTimer(1f);
    }

    private void UpdateStocks()
    {
        if (m_stockUpdateTimer.Update())
        {
            for (int i = 0; i < m_stockList.Count; i++)
            {
                m_stockList[i].PredictNewValue();
            }
            if (m_StocksUpdatedPtr != null)
            {
                m_StocksUpdatedPtr.Invoke();
            }
        }
    }

    public void AttemptToTradeStock(int a_stockID, int a_amountTraded)
    {
        Stock stockRef = m_stockList[a_stockID];
        bool traded = false;
        if (a_amountTraded > 0)
        {
            if (m_gameHandlerRef.GetCurrentCash() >= stockRef.GetCurrentValue())
            {
                traded = true;
            }
        }
        else
        {
            if (stockRef.GetAmountOwned() >= -a_amountTraded)
            {
                traded = true;
            }
        }
        if (traded)
        {
            stockRef.TradeStock(a_amountTraded);
            m_gameHandlerRef.ChangeCash(-stockRef.GetCurrentValue() * a_amountTraded);
        }

    }

    // Update is called once per frame
    public void Update()
    {
        UpdateStocks();
    }
}
