using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class ArmorSegment : MonoBehaviour
{
    [SerializeField] SpriteRenderer m_baseSpriteRenderer;
    [SerializeField] SpriteRenderer m_crackSpriteRef;
    public Sprite[] m_equipmentSymbols = new Sprite[(int)EquipmentAbility.eAbilityType.Count];
    public Sprite[] m_segmentOutlineRefs = new Sprite[(int)Equipment.eRarityTier.Count];
    [SerializeField] Sprite[] m_crackSpriteRefs = new Sprite[(int)Equipment.eRarityTier.Count];

    public SpriteRenderer m_outlineSpriteRenderer;

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

    internal void RefreshFromHealth()
    {
        float healthScale = m_equipment.m_health / m_equipment.m_originalMaxHealth;
        m_baseSpriteRenderer.color = new Color(1f, healthScale, healthScale);
        int crackIndex = (int)((m_crackSpriteRefs.Length+1) * (1f-healthScale));
        crackIndex = Mathf.Clamp(crackIndex, 0, m_crackSpriteRefs.Length);
        if (crackIndex > 0)
        {
            m_crackSpriteRef.sprite = m_crackSpriteRefs[crackIndex-1];
        }
        m_crackSpriteRef.gameObject.SetActive(crackIndex > 0);
    }

    internal void AssignEquipment(Equipment a_equipment)
    {
        m_equipment = a_equipment;
        //m_outlineSpriteRenderer.sprite = m_segmentOutlineRefs[(int)a_equipment.m_rarity.tier];
        m_symbolSpriteRenderer.sprite = m_equipmentSymbols[(int)a_equipment.m_activeAbility.m_abilityType];

        RefreshFromHealth();
    }

}