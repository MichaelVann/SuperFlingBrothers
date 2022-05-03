using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollingText : MonoBehaviour
{
    Text m_localTextRef;

    float m_desiredValue = 100;
    float m_currentValue = 0;

    float m_rollTime= 1.5f;
    float m_elapsedTime = 0f;


    public void SetDesiredValue(float a_value)
    {
        m_desiredValue = a_value;
    }

    public void SetCurrentValue(float a_value)
    {
        m_currentValue = a_value;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_localTextRef = GetComponent<Text>();
        m_localTextRef.text = "" + m_currentValue;
    }

    // Update is called once per frame
    void Update()
    {
        //If roll is not completed
        if (m_desiredValue != m_currentValue && m_desiredValue > m_currentValue)
        {
            m_elapsedTime += Time.deltaTime;
            float value = m_desiredValue * Mathf.Pow(m_elapsedTime / m_rollTime, 3f);

            m_currentValue = value;
            m_currentValue = Mathf.Clamp(m_currentValue, 0f, m_desiredValue);
            m_localTextRef.text = "" + VLib.TruncateFloatsDecimalPlaces(m_currentValue, 2);
        }
    }
}
