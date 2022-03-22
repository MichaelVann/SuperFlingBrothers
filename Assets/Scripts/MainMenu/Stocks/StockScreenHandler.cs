using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StockScreenHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    StockHandler m_stockHandlerRef;
    public GameObject m_stockOverviewTemplate;
    public GameObject m_contentRef;

    public Sprite[] m_stockSprites;
    List<StockOverview> m_stockOverviewList;

    //Graph
    Stock m_selectedStock;
    public vGraph m_graphRef;
    int m_selectedStockID = 0;
    int m_tradeAmount = 1;

    public Button m_buyButton;
    public Button m_sellButton;
    public Counter m_cashCounter;

    private void AlignSelectedStockRefToID() { m_selectedStock = m_stockHandlerRef.m_stockList[m_selectedStockID]; }

    // Start is called before the first frame update
    void Start()
    {
        m_stockOverviewList = new List<StockOverview>();
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_stockHandlerRef = m_gameHandlerRef.GetStockHandlerRef();
        for (int i = 0; i < m_stockHandlerRef.m_stockList.Count; i++)
        {
            StockOverview stockOverview = Instantiate<GameObject>(m_stockOverviewTemplate, m_contentRef.transform).GetComponent<StockOverview>();
            stockOverview.Init(i, this);
            m_stockOverviewList.Add(stockOverview);
        }

        AlignSelectedStockRefToID();
        m_stockHandlerRef.m_StocksUpdatedPtr = new StockHandler.StocksUpdatedPtr(UpdateGraph);
        m_graphRef.Init(m_selectedStock.GetTrackedValues(), m_selectedStock.GetName());
    }

    internal void Select(int a_graphDisplayedStockID)
    {
        m_selectedStockID = a_graphDisplayedStockID;
        AlignSelectedStockRefToID();
        m_graphRef.Init(m_selectedStock.GetTrackedValues(), m_selectedStock.GetName());
    }

    private void UpdateGraph()
    {
        m_graphRef.Refresh();
    }

    public void AttemptToBuyStock()
    {
        m_stockHandlerRef.AttemptToTradeStock(m_selectedStockID,m_tradeAmount);
    }

    public void AttemptToSellStock()
    {
        m_stockHandlerRef.AttemptToTradeStock(m_selectedStockID, -m_tradeAmount);
    }

    private void UpdateBuySellButtonsStatus()
    {
        m_buyButton.interactable = m_selectedStock.GetCurrentValue() * (float)m_tradeAmount <= m_gameHandlerRef.GetCurrentCash();
        m_sellButton.interactable = m_selectedStock.GetAmountOwned() >= (float)m_tradeAmount;
    }


    // Update is called once per frame
    void Update()
    {
        UpdateBuySellButtonsStatus();
        m_cashCounter.SetString("" + VLib.TruncateFloatsDecimalPlaces(m_gameHandlerRef.GetCurrentCash(),2));
    }
}
