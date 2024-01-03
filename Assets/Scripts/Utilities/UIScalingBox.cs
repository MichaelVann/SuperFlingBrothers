using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIScalingBox : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI m_titleTextRef;
    [SerializeField]TextMeshProUGUI m_descriptionTextRef;
    [SerializeField]GameObject m_container;

    internal delegate void OnCloseDelegate();
    OnCloseDelegate m_onCloseDelegate;

    internal void SetOnCloseDelegate(OnCloseDelegate a_delegate)
    {
        m_onCloseDelegate = a_delegate;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_container.AddComponent<ZoomExpandComponent>().Init(0f,1f,0.3f, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void SetUp(string a_titleText, string a_descriptionString)
    {
        m_titleTextRef.text = a_titleText;
        m_descriptionTextRef.text = a_descriptionString;
    }

    internal void AddDescriptionString(string a_descriptionString)
    {
        m_descriptionTextRef.text += '\n' + a_descriptionString;
    }

    void Exit()
    {
        if (m_onCloseDelegate != null)
        {
            m_onCloseDelegate.Invoke();
        }
        Destroy(gameObject);
    }

    public void Close()
    {
        m_container.AddComponent<ZoomExpandComponent>().Init(1f, 0f, 0.3f, 2f, Exit);
    }
}
