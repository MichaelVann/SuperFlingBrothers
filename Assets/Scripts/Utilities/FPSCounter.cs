using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_textRef;
    [SerializeField] float m_sampleRateInSeconds = 0.3f;
    float m_elapsedTime;
    // Start is called before the first frame update
    void Start()
    {
    }

    void ReadToDisplay()
    {
        float fps = 1f / Time.unscaledDeltaTime;
        if (fps < 10f)
        {
            fps = VLib.RoundToDecimalPlaces(fps, 0);
        }
        else
        {
            fps = VLib.RoundToDecimalPlaces(fps, 2);
        }
        m_textRef.text = fps.ToString() + " fps";
    }

    // Update is called once per frame
    void Update()
    {
        m_elapsedTime += Time.unscaledDeltaTime;
        if (m_elapsedTime > m_sampleRateInSeconds)
        {
            m_elapsedTime = 0f;
            ReadToDisplay();
        }
    }
}

    

