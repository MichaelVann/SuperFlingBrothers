using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentInteractButton : MonoBehaviour
{
    public Button m_buttonRef;
    Equipment m_equipmentRef;
    GameHandler m_gameHandlerRef;
    SquadScreenHandler m_equipmentScreenHandlerRef;
    public Text m_equipButtonTextRef;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    internal void Init(GameHandler a_gameHandlerRef, SquadScreenHandler a_equipmentScreenHandler, Equipment a_equipmentRef)
    {
        m_gameHandlerRef = a_gameHandlerRef;
        m_equipmentScreenHandlerRef = a_equipmentScreenHandler;
        m_equipmentRef = a_equipmentRef;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void SetEquipButtonStatus()
    {
        Color equipButtonColor = Color.white;
        string equipButtonString = "";
        bool equipped = m_equipmentRef.m_equipped;
        bool sameSlot = m_equipmentScreenHandlerRef.m_openedEquipmentSlotId == m_equipmentRef.m_equippedSlotId;
        if (m_equipmentRef.IsBroken())
        {
            equipButtonColor = Color.grey;
            equipButtonString = "Repair" + "(" + m_equipmentRef.GetRepairCost() + ")";
            m_buttonRef.interactable = m_gameHandlerRef.GetCurrentCash() >= m_equipmentRef.GetRepairCost();
        }
        else
        {
            if (!equipped)
            {
                equipButtonColor = Color.white;
                equipButtonString = "Equip";
            }
            else if (equipped && sameSlot)
            {
                equipButtonColor = Color.yellow;
                equipButtonString = "UnEquip";
            }
            else if (equipped && !sameSlot)
            {
                equipButtonColor = Color.red;
                equipButtonString = "ReEquip";
            }
        }

        m_buttonRef.gameObject.GetComponent<Image>().color = equipButtonColor;
        m_equipButtonTextRef.text = equipButtonString;
    }
}
