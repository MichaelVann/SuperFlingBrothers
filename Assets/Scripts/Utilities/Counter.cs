using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    public TextMeshProUGUI m_text;

    public string m_string;

    public void SetString(string a_string)
    {
        m_string = a_string;
        m_text.text = a_string;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
}
