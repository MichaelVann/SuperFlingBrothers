using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentScreenHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public EquipableSlotUI[] m_equipableSlotUIRefs;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        for (int i = 0; i < m_equipableSlotUIRefs.Length; i++)
        {
            m_equipableSlotUIRefs[i].SetEquipableRef(m_gameHandlerRef.m_playerEquipable);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
