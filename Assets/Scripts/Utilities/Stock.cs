using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class Stock
{
    [SerializeField]
    string m_name;
    [SerializeField]
    int m_amountOwned;
    [SerializeField]
    float[] m_values;
    [SerializeField]
    float m_currentValue;
    [SerializeField]
    int m_valuesTracked;
    [SerializeField]
    float m_normalValue;
    [SerializeField]
    float m_trendNormalValue;

    [SerializeField]
    float m_deviationMultiplicityRange;
    [SerializeField]
    float m_volatility;

    [SerializeField]
    float m_stability;

    [SerializeField]
    vTimer m_priceShiftTimer;

    //Defaults
    const int m_defaultvaluesTracked = 15;
    const float m_defaultNormalValue = 100f;
    float m_defaultDeviationMultiplicityRange = 1f;
    float m_defaultVolatility = 0.1f;
    float m_defaultStability = 2f;

    public float GetCurrentValue() { return m_currentValue; }
    public float GetAmountOwned() { return m_amountOwned; }
    public float[] GetTrackedValues() { return m_values; }
    public string GetName() { return m_name; }

    private void Init(string a_name, int a_valuesTracked, float a_normalValue, float a_deviationMultiplicityRange, float a_volatility, float a_stability)
    {
        m_name = a_name;
        m_amountOwned = 0;
        m_values = new float[a_valuesTracked];
        m_valuesTracked = a_valuesTracked;
        m_values[0] = m_currentValue = m_normalValue = m_trendNormalValue = a_normalValue;
        m_deviationMultiplicityRange = a_deviationMultiplicityRange;
        m_volatility = a_volatility;
        m_stability = a_stability;
        m_priceShiftTimer = new vTimer(0.06f,true);
    }

    public Stock(string a_name, int a_valuesTracked, float a_normalValue, float a_deviationMultiplicityRange, float a_volatility, float a_stability)
    {
        Init(a_name, a_valuesTracked, a_normalValue, a_deviationMultiplicityRange,  a_volatility, a_stability);
    }

    public Stock(string a_name, float a_normalValue, float a_deviationMultiplicityRange, float a_volatility, float a_stability)
    {
        Init(a_name, m_defaultvaluesTracked, a_normalValue, a_deviationMultiplicityRange, a_volatility, a_stability);
    }

    public Stock(string a_name)
    {
        Init(a_name, m_defaultvaluesTracked, m_defaultNormalValue, m_defaultDeviationMultiplicityRange, m_defaultVolatility, m_defaultStability);
    }

    public Stock()
    {
        Init("Stock", m_defaultvaluesTracked, m_defaultNormalValue, m_defaultDeviationMultiplicityRange, m_defaultVolatility, m_defaultStability);
    }

    private void PushBackValue(float a_value)
    {
        for (int i = m_values.Length - 1; i > 0; i--)
        {
            m_values[i] = m_values[i - 1];
        }
        m_values[0] = a_value;
        m_currentValue = m_values[0];
    }

    public void PredictNewValue()
    {
        if (m_priceShiftTimer.Update())
        {
            m_trendNormalValue = m_normalValue * UnityEngine.Random.Range(1/m_stability, m_stability);
        }

        float deltaNormal = (m_trendNormalValue / m_currentValue);

        if (deltaNormal < 1f)
        {
            deltaNormal = -((m_currentValue / m_trendNormalValue) - 1f);
        }
        else
        {
            deltaNormal -= 1f;
        }

        float deltaNormalFactor = m_volatility * Mathf.Pow((deltaNormal/ m_deviationMultiplicityRange), 1f);
        float randomAddition = UnityEngine.Random.Range(-m_volatility + deltaNormalFactor, m_volatility + deltaNormalFactor);
        float newValue = m_currentValue * (1f + randomAddition);

        PushBackValue(newValue);
        //Debug.Log("newValue:" + newValue);
        //Debug.Log("deltaNormal:" + deltaNormal);
        //Debug.Log("deltaNormalFactor:" + deltaNormalFactor);
        Debug.Log("m_trendNormalValue:" + m_trendNormalValue);
    }

    public void TradeStock(int a_amountTraded)
    {
        m_amountOwned += a_amountTraded;
    }
}
