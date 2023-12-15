using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CashDisplay : MonoBehaviour
{
    public Counter m_counterRef;
    GameHandler m_gameHandlerRef;
    float m_lastValue;
    bool m_lastValueInited = false;

    [SerializeField] GameObject m_risingTextPrefab;
    [SerializeField] Transform m_deltaTextSpawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
    }

    void Refresh()
    {
        float value = VLib.TruncateFloatsDecimalPlaces(m_gameHandlerRef.GetCurrentCash(), 2);
        m_counterRef.SetString("" + value);
        if (m_lastValueInited && value != m_lastValue)
        {
            float deltaValue = value - m_lastValue;
            RisingFadingText rft = Instantiate(m_risingTextPrefab,transform).GetComponent<RisingFadingText>();
            rft.SetOriginalPosition(m_deltaTextSpawnPoint.position);
            rft.transform.position = m_deltaTextSpawnPoint.position;
            rft.SetImageEnabled(false);
            rft.SetGravityAffected(false);
            rft.SetHorizontalSpeed(0f);
            rft.SetLifeTimerMax(1.35f);
            rft.SetTextContent(deltaValue > 0 ? "+" + deltaValue : "-" + deltaValue);
            rft.SetOriginalColor(deltaValue > 0 ? Color.green : Color.red);
            rft.SetOriginalScale(1.6f);
        }
        m_lastValue = value;
        m_lastValueInited = true;

    }

    // Update is called once per frame
    void Update()
    {
        Refresh();
    }
}
