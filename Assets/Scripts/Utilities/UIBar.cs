using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour
{
    //GameHandler m_gameHandlerRef;

    float m_barLength = 650f;
    float m_barHeight = 25f;

    public RectTransform m_barMaskTransform;
    public Text m_barValueText;
    public Text m_labelText;
    public GameObject m_fillingBar;

    float m_value = 0f;
    float m_maxValue = 0f;

    // Start is called before the first frame update
    void Start()
    {
        //m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_barLength = m_fillingBar.GetComponent<RectTransform>().rect.width;
        m_barHeight = m_fillingBar.GetComponent<RectTransform>().rect.height;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(float a_minValue, float a_maxValue)
    {
        m_value = a_minValue;
        m_maxValue = a_maxValue;
        UpdateBarSprite();
    }

    public void SetBarValue(float a_value)
    {
        m_value = a_value;
        UpdateBarSprite();
    }

    public void UpdateBarSprite()
    {
        m_barMaskTransform.sizeDelta = new Vector2(m_barLength * m_value / m_maxValue, m_barHeight);
        m_barValueText.text = "" + m_value.ToString("f2") + " / " + m_maxValue.ToString("f2");
    }

    public void SetLabeltext(string a_string)
    {
        m_labelText.text = a_string;
    }
}
