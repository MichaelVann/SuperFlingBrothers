using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RisingFadingText : MonoBehaviour
{
    float m_lifeTimer = 0f;
    float m_lifeTimerMax = 0.8f;
    float m_risingSpeed = 0.25f;
    float m_horizontalSpeed = 0.2f;
    const float m_fadeDelay = 0.2f;
    bool m_gravityAffected = true;

    Color m_originalColor;
    Vector3 m_originalPosition;

    public void SetGravityAffected(bool a_value) { m_gravityAffected = a_value; }
    public void SetHorizontalSpeed(float a_value) { m_horizontalSpeed = a_value; }
    public void SetLifeTimerMax(float a_value) { m_lifeTimerMax = a_value; }

    void Awake()
    {
        m_originalColor = GetComponent<Text>().color;
        m_originalPosition = transform.position;
        m_horizontalSpeed = Random.Range(-0.3f, 0.3f);
        m_risingSpeed = Random.Range(0.25f, 0.3f);
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

        float verticalOffset = m_gravityAffected ? Mathf.Sin(completionPerc * 4f) * m_risingSpeed : completionPerc * m_risingSpeed / 2f;

        transform.position = m_originalPosition + new Vector3(m_horizontalSpeed * completionPerc, verticalOffset);
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
