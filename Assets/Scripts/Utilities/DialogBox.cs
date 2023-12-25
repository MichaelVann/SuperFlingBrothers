using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour
{
    [SerializeField] GameObject m_parentObject;
    [SerializeField] TextMeshProUGUI m_speakerNameTextRef;
    [SerializeField] TextMeshProUGUI m_descriptionTextRef;
    [SerializeField] Image m_speakerImageRef;

    string m_descriptionString;
    int m_charactersShown = 0;
    vTimer m_printTimer;
    bool m_printing = false;

    bool m_writingText = false;

    float m_printScreen = 1f;
    const float m_characterPrintTime = 0.03f;

    List<string> m_dialogList;

    float m_flashStrength = 0f;
    const float m_baseSpeakerColorScale = 0.5f;
    const float m_flashDecayMultiplier = 0.99f;

    internal void AddDialog(string a_string) { m_dialogList.Add(a_string); m_descriptionString = m_dialogList[0]; } 

    internal void SetPrintSpeed(float a_printSpeed) { m_printScreen = a_printSpeed; RefreshPrintTimer(); }

    // Start is called before the first frame update
    void Start()
    {
        ZoomExpandComponent closingZoom = gameObject.AddComponent<ZoomExpandComponent>();
        closingZoom.SetUp(0f, 1f, 0.3f, 2f, Open);
    }

    internal void Init(string a_speakerName)
    {
        m_speakerNameTextRef.text = a_speakerName;
        m_dialogList = new List<string>();
        m_descriptionString = "";
        RefreshPrintTimer();
    }

    internal void AddDialogs(List<string> a_dialogs)
    {
        for (int i = 0; i < a_dialogs.Count; i++)
        {
            AddDialog(a_dialogs[i]);
        }
    }

    void RefreshPrintTimer()
    {
        m_printTimer = new vTimer(m_characterPrintTime / m_printScreen, false, true, true, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_writingText && m_charactersShown < m_descriptionString.Length)
        {
            if (m_printTimer.Update())
            {
                m_charactersShown++;
                m_descriptionTextRef.text = m_descriptionString.Substring(0, m_charactersShown);

                if (m_descriptionTextRef.text[m_descriptionTextRef.text.Length - 1] != ' ')
                {
                    m_flashStrength = 1f;
                }
            }
            m_printing = true;
        }
        else
        {
            m_printing = false;
        }
        m_flashStrength *= m_flashDecayMultiplier;
        float colorScale = m_baseSpeakerColorScale + (1f- m_baseSpeakerColorScale) * m_flashStrength;
        m_speakerImageRef.color = new Color(colorScale, colorScale, colorScale,1f);
    }

    void ProgressDialog()
    {
        if (m_dialogList.Count > 1)
        {
            m_dialogList.RemoveAt(0);
            m_descriptionString = m_dialogList[0];
            m_charactersShown = 0;
            RefreshPrintTimer();
        }
        else
        {
            ZoomExpandComponent closingZoom = gameObject.AddComponent<ZoomExpandComponent>();
            closingZoom.SetUp(1f,0f,0.3f,2f, Close);
        }
    }

    public void DialogPressed()
    {
        if (m_printing)
        {
            m_charactersShown = m_descriptionString.Length-1;
        }
        else
        {
            ProgressDialog();
        }
    }

    void Open()
    {
        m_writingText = true;
    }

    void Close()
    {
        Destroy(m_parentObject);
    }
}