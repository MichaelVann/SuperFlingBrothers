using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StockOverview : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    StockScreenHandler m_stockScreenHandler;
    public GameObject m_backgroundRef;
    public GameObject m_selectButtonRef;
    Stock m_stockRef;
    public int m_stockId;

    public Text m_nameTextRef;
    public Text m_costTextRef;
    public Text m_amountOwnedText;
    public Image m_imageRef;

    // Start is called before the first frame update
    void Awake()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
    }

    public void Init(int a_stockID, StockScreenHandler a_stockScreenHandler)
    {
        m_stockId = a_stockID;
        m_stockRef = m_gameHandlerRef.GetStockHandlerRef().m_stockList[m_stockId];
        m_stockScreenHandler = a_stockScreenHandler;

        m_nameTextRef.text = m_stockRef.GetName();
        Refresh();
    }

    void Refresh()
    {
        m_costTextRef.text = "" + VLib.TruncateFloatsDecimalPlaces(m_stockRef.GetCurrentValue(), 2);
        m_amountOwnedText.text = "" + m_stockRef.GetAmountOwned();
    }

    public void SelectButtonPressed()
    {
        m_stockScreenHandler.Select(m_stockId);
    }

    public void SetSelected(bool a_value)
    {
        if (a_value)
        {
            m_backgroundRef.GetComponent<Outline>().effectColor = Color.green;
            m_selectButtonRef.GetComponent<Image>().color = Color.green;
            m_selectButtonRef.GetComponentInChildren<Text>().text = "Selected";
        }
        else
        {
            m_backgroundRef.GetComponent<Outline>().effectColor = Color.black;
            m_selectButtonRef.GetComponent<Image>().color = Color.white;
            m_selectButtonRef.GetComponentInChildren<Text>().text = "Select";
        }
    }


    // Update is called once per frame
    void Update()
    {
        Refresh();
    }
}
