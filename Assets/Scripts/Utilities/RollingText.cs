using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RollingText : MonoBehaviour
{
    Text m_localTextRef;

    int m_desiredValue = 100;
    int m_currentValue = 0;
    float m_currentInterest = 0f;

    float m_rollSpeed = 160f;

    public void SetDesiredValue(int a_value)
    {
        m_desiredValue = a_value;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_localTextRef = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_desiredValue != m_currentValue && m_desiredValue > m_currentValue)
        {
            float increase = Time.deltaTime * (Mathf.Sin((((float)m_currentValue / (float)m_desiredValue) * Mathf.PI/2) + Mathf.PI/2) + 0.01f) * m_rollSpeed;
            m_currentInterest += increase;
            if (m_currentInterest >= 1f)
            {
                m_currentValue += (int)m_currentInterest;
                if (m_currentValue > m_desiredValue)
                {
                    m_currentValue = m_desiredValue;
                }
                m_currentInterest -= 1f;
            }
            m_localTextRef.text = "" + m_currentValue;
        }

        if (Input.GetKey(KeyCode.H))
        {
            m_currentValue = 0;
        }

        if (Input.GetKey(KeyCode.Y))
        {
            m_rollSpeed += Time.deltaTime * 10f;
            Debug.Log(m_rollSpeed);
        }
        if (Input.GetKey(KeyCode.N))
        {
            m_rollSpeed -= Time.deltaTime * 10f;
            Debug.Log(m_rollSpeed);
        }
    }
}
