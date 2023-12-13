using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RisingFadingText : MonoBehaviour
{
    Image m_imageRef;
    float m_lifeTimer = 0f;
    float m_lifeTimerMax = 0.8f;
    float m_risingSpeed = 0.25f;
    float m_horizontalSpeed = 0.2f;
    const float m_fadeDelay = 0.2f;
    bool m_gravityAffected = true;

    Color m_originalColor;
    Vector3 m_originalPosition;
    float m_originalScale = 1f;

    public void SetGravityAffected(bool a_value) { m_gravityAffected = a_value; }
    public void SetHorizontalSpeed(float a_value) { m_horizontalSpeed = a_value; }
    public void SetLifeTimer(float a_value) { m_lifeTimer = a_value; }
    public void SetLifeTimerMax(float a_value) { m_lifeTimerMax = a_value; }

    public void SetImageEnabled(bool a_value) { m_imageRef.enabled = a_value; }
    public void SetOriginalScale(float a_value) { m_originalScale = a_value; }
    public void SetOriginalPosition(Vector3 a_value) { m_originalPosition = a_value; }

    void Awake()
    {
        m_originalColor = GetComponent<Text>().color;
        m_originalPosition = transform.position;
        m_horizontalSpeed = Random.Range(-0.3f, 0.3f);
        m_risingSpeed = Random.Range(0.25f, 0.3f);
        m_imageRef = GetComponentInChildren<Image>();
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
        m_lifeTimer += Time.unscaledDeltaTime;
        if (m_lifeTimer >= m_lifeTimerMax)
        {
            Destroy(m_imageRef.gameObject);
            Destroy(this);
        }

        float completionPerc = m_lifeTimer / m_lifeTimerMax;

        float verticalOffset = m_gravityAffected ? Mathf.Sin(completionPerc * Mathf.PI) * m_risingSpeed : completionPerc * m_risingSpeed / 2f;

        float scale = Mathf.Pow(Mathf.Clamp(Mathf.Sin(Mathf.Pow(completionPerc,0.5f) * Mathf.PI),0f,1f),0.2f);
        scale *= m_originalScale;
        transform.position = m_originalPosition + new Vector3(m_horizontalSpeed * completionPerc, verticalOffset);
        transform.localScale = new Vector3(scale, scale, 1f);
        if (m_lifeTimer >= m_fadeDelay)
        {
            float colorScale = 1f - Mathf.Pow((m_lifeTimer - m_fadeDelay) / (m_lifeTimerMax - m_fadeDelay), 2f);
            float exponentialColorScale = Mathf.Pow(colorScale, 3f);
            GetComponent<Text>().color = new Color(m_originalColor.r * exponentialColorScale, m_originalColor.g * exponentialColorScale, m_originalColor.b * exponentialColorScale, m_originalColor.a * colorScale);
        }
    }

    void Update()
    {
        LifeTimerUpdate();
    }
}
