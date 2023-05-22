using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentDrop : Loot
{
    public int m_dropLevel = 0;
    public Equipment m_equipment;
    GameHandler m_gameHandlerRef;
    public SpriteRenderer m_rarityRingSpriteRendererRef;
    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_equipment = new Equipment(m_gameHandlerRef.m_playerStatHandler.m_level);
        m_rarityRingSpriteRendererRef.color = m_equipment.m_rarityTier.color;
    }

    // Update is called once per frame
    override public void Update()
    {
        base.Update();
        m_rarityRingSpriteRendererRef.gameObject.transform.eulerAngles += new Vector3(0f,Time.deltaTime*200f,0f);
    }
}
