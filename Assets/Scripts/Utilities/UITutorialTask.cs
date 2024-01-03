using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITutorialTask : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_taskString;
    [SerializeField] UICheckBox m_checkBoxRef;
    vTimer m_destructionTimer;
    RectTransform m_rectTransform;
    float m_originalTransformHeight;
    internal delegate void OnDestroyDelegate();
    OnDestroyDelegate m_onDestroyDelegate;

    const float m_destructionTime = 0.4f;

    // Start is called before the first frame update
    void Awake()
    {
        ZoomExpandComponent startingZoom = gameObject.AddComponent<ZoomExpandComponent>();
        startingZoom.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_destructionTimer != null)
        {
            if (m_destructionTimer.Update())
            {
                if(m_onDestroyDelegate != null)
                {
                    m_onDestroyDelegate.Invoke();
                }
                Destroy(gameObject);
            }
            float yOffset = m_originalTransformHeight * m_destructionTimer.GetCompletionPercentage();
            if (m_rectTransform.pivot.y == 0)
            {
                yOffset *= -1f;
            }
            m_rectTransform.anchoredPosition = new Vector2(m_rectTransform.anchoredPosition.x, yOffset);
        }
    }

    internal void Init(string a_string, bool a_topSide, OnDestroyDelegate a_delegate)
    {
        m_taskString.text = "<color=black>Task:</color> " + a_string;
        SetCompleted(false);
        m_rectTransform = GetComponent<RectTransform>();
        if (a_topSide)
        {
            m_rectTransform.anchorMin = new Vector2(0f, 1f);
            m_rectTransform.anchorMax = new Vector2(1f, 1f);
            m_rectTransform.pivot = new Vector2(0.5f, 1f);
            m_rectTransform.anchoredPosition = new Vector3(m_rectTransform.anchoredPosition.x, 0f);
        }
        else
        {
            m_rectTransform.anchorMin = new Vector2(0f, 0f);
            m_rectTransform.anchorMax = new Vector2(1f, 0f);
            m_rectTransform.pivot = new Vector2(0.5f, 0f);
            m_rectTransform.anchoredPosition = new Vector3(m_rectTransform.anchoredPosition.x, 0f);
        }
        m_originalTransformHeight = m_rectTransform.rect.height;
        m_onDestroyDelegate = a_delegate;
    }

    internal void SetCompleted(bool a_completed)
    {
        m_checkBoxRef.SetToggled(a_completed);
        if (a_completed)
        {
            m_destructionTimer = new vTimer(m_destructionTime);
        }
    }
}
