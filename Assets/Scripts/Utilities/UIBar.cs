using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{
    //GameHandler m_gameHandlerRef;

    float m_barLength = 650f;
    float m_barHeight = 25f;

    public RectTransform m_barMaskTransform;
    public TextMeshProUGUI m_barValueText;
    public TextMeshProUGUI m_labelText;
    public GameObject m_fillingBar;

    float m_value = 0f;
    float m_maxValue = 0f;

    public void SetBarValue(float a_value)
    {
        m_value = a_value;
        UpdateBarSprite();
    }

    public void SetBarColor(Color a_color)
    {
        m_fillingBar.GetComponent<Image>().color = a_color;
    }

    public void SetValueTextColor(Color a_color)
    {
        m_barValueText.color = a_color;
    }


    // Start is called before the first frame update
    void Awake()
    {
        //m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_barLength = m_fillingBar.GetComponent<RectTransform>().rect.width;
        m_barHeight = m_fillingBar.GetComponent<RectTransform>().rect.height;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(float a_currentValue, float a_maxValue)
    {
        m_value = a_currentValue;
        m_maxValue = a_maxValue;
        UpdateBarSprite();
    }

    //Sets the current and max of the bar to the value
    public void Init(float a_maxValue)
    {
        Init(a_maxValue, a_maxValue);
    }

    public void UpdateBarSprite()
    {
        m_barMaskTransform.sizeDelta = new Vector2(m_barLength * m_value / m_maxValue, m_barHeight);
        m_barValueText.text = "" + (int)(m_value)+ " / " + m_maxValue.ToString("f0");
    }

    public void SetLabeltext(string a_string)
    {
        m_labelText.text = a_string;
    }
}
