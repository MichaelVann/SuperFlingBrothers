using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class ArmorSegment : MonoBehaviour
{
    public Sprite[] m_equipmentSymbols = new Sprite[(int)EquipmentAbility.eAbilityType.Count];
    //public Sprite m_armorSprite;
    public SpriteRenderer m_symbolSpriteRenderer;
    GameHandler m_gameHandlerRef;
    Equipment m_equipment;
    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void AssignEquipment(Equipment a_equipment)
    {
        m_equipment = a_equipment;
        m_symbolSpriteRenderer.sprite = m_equipmentSymbols[(int)a_equipment.m_activeAbility.m_abilityType];
    }

}
