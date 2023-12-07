using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentPortrait : MonoBehaviour
{
    Equipment m_equipmentRef;
    public ArmorSegment m_armorSegmentRef;

    public void SetEquipmentRef(Equipment a_equipment)
    {
        m_equipmentRef = a_equipment;
        m_armorSegmentRef.AssignEquipment(m_equipmentRef);
        Refresh();
    }
    // Start is called before the first frame update
    void Awake()
    {
    }

    void Refresh()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
