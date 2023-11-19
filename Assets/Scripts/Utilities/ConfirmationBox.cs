using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConfirmationBox : MonoBehaviour
{
    public TextMeshProUGUI m_messageText;
    internal delegate void ConfirmationResponseDelegate();
    internal ConfirmationResponseDelegate m_confirmationResponseDelegate;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void SetMessageText(string a_string)
    {
        m_messageText.text = a_string;
    }

    public void Respond(bool a_response)
    {
        if (m_confirmationResponseDelegate != null && a_response) 
        {
            m_confirmationResponseDelegate.Invoke();
        }
        Destroy(gameObject);
    }
}
