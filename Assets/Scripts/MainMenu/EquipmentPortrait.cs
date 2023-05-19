using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentPortrait : MonoBehaviour
{
    Image m_imageRef;
    Equipment m_equipmentRef;

    public void SetEquipmentRef(Equipment a_equipment)
    {
        m_equipmentRef = a_equipment;
        Refresh();
    }
    // Start is called before the first frame update
    void Awake()
    {
        m_imageRef = GetComponent<Image>();
    }

    void Refresh()
    {
        if (m_imageRef)
        {
            m_imageRef.color = m_equipmentRef.m_rarityTier.color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
