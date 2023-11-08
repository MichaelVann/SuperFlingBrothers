using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyEquipmentButton : MonoBehaviour
{
    public TextMeshProUGUI m_costTextRef;
    Button m_buttonRef;
    GameHandler m_gameHandlerRef;
    // Start is called before the first frame update
    void Awake()
    {
        m_buttonRef = GetComponent<Button>();
    }
    public void Refresh()
    {
        if (!m_gameHandlerRef)
        {
            m_gameHandlerRef = FindObjectOfType<GameHandler>();
        }
        m_buttonRef.interactable = m_gameHandlerRef.CanAffordJunkEquipment();

        m_costTextRef.text = "" + m_gameHandlerRef.GetJunkEquipmentCost();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
