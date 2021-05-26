using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RisingFadingText : MonoBehaviour
{
    float m_lifeTimer = 0f;
    const float m_lifeTimerMax = 0.75f;
    const float m_risingSpeed = 0.25f;
    const float m_horizontalSpeed = 0.2f;
    const float m_fadeDelay = 0.2f;

    Color m_originalColor;
    Vector3 m_originalPosition;

    void Awake()
    {
        m_originalColor = GetComponent<Text>().color;
        m_originalPosition = transform.position;
    }

    public void SetOriginalColor(Color a_color)
    {
        m_originalColor = a_color;
        GetComponent<Text>().color = a_color;
    }

    public void SetTextContent(string a_string)
    {
        GetComponent<Text>().text = a_string;
    }

    public void SetTextContent(float a_float)
    {
        GetComponent<Text>().text = ("" + VLib.TruncateFloatsDecimalPlaces(a_float, 2));
    }

    void LifeTimerUpdate()
    {
        m_lifeTimer += Time.deltaTime;
        if (m_lifeTimer >= m_lifeTimerMax)
        {
            Destroy(this);
        }

        float completionPerc = m_lifeTimer / m_lifeTimerMax;

        transform.position = m_originalPosition + new Vector3(m_horizontalSpeed * completionPerc, Mathf.Sin(completionPerc * 4f)* m_risingSpeed);
        if (m_lifeTimer >= m_fadeDelay)
        {
            GetComponent<Text>().color = new Color(m_originalColor.r, m_originalColor.g, m_originalColor.b, m_originalColor.a * 1 - ((m_lifeTimer-m_fadeDelay) / (m_lifeTimerMax-m_fadeDelay)));
        }


    }

    void Update()
    {
        LifeTimerUpdate();
    }
}
