using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentPortrait : MonoBehaviour
{
    Image m_imageRef;
    Equipable m_equipableRef;

    public void SetEquipableRef(Equipable a_equipable)
    {
        m_equipableRef = a_equipable;
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
            m_imageRef.color = m_equipableRef.m_rarityTier.color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
