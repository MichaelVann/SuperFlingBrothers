using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Stock
{
    string m_name;
    float[] m_values;
    float m_currentValue;
    int m_valuesTracked;
    float m_normalValue;
    float m_trendNormalValue;

    float m_deviationMultiplicityRange;
    float m_volatility;

    float m_stability = 2f;

    vTimer m_priceShiftTimer;

    const int m_defaultvaluesTracked = 10;
    const float m_defaultNormalValue = 100f;

    public float GetCurrentValue() { return m_currentValue; }

    private void Init(string a_name, int a_valuesTracked, float a_normalValue, float a_deviationMultiplicityRange, float a_volatility, float a_stability)
    {
        m_name = a_name;
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

    public Stock()
    {
        Init("Stock", m_defaultvaluesTracked, m_defaultNormalValue, 1f, 0.1f, 2f);
    }

    public void PredictNewValueOld()
    {
        if (m_priceShiftTimer.Update())
        {
            m_trendNormalValue = m_normalValue * UnityEngine.Random.Range(1 / m_stability, m_stability);
        }

        float deltaNormal = m_trendNormalValue - m_currentValue;
        float deltaNormalFactor = m_volatility * Mathf.Pow((deltaNormal / m_trendNormalValue * m_deviationMultiplicityRange), 1f);
        float randomAddition = UnityEngine.Random.Range(-m_volatility + deltaNormalFactor, m_volatility + deltaNormalFactor);
        float newValue = m_currentValue * (1f + randomAddition);

        m_currentValue = newValue;
        //Debug.Log("newValue:" + newValue);
        //Debug.Log("deltaNormal:" + deltaNormal);
        //Debug.Log("deltaNormalFactor:" + deltaNormalFactor);
        //Debug.Log("m_internalTrendInertia:" + m_internalTrendInertia);
        //Debug.Log("m_internalTrend:" + m_internalTrend);
        Debug.Log("deltaNormalFactor:" + deltaNormalFactor);
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

        m_currentValue = newValue;
        //Debug.Log("newValue:" + newValue);
        //Debug.Log("deltaNormal:" + deltaNormal);
        //Debug.Log("deltaNormalFactor:" + deltaNormalFactor);
        Debug.Log("m_trendNormalValue:" + m_trendNormalValue);
    }
}
