using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentDrop : MonoBehaviour
{
    public int m_dropLevel = 0;
    public Equipment m_equipment;
    GameHandler m_gameHandlerRef;
    public SpriteRenderer m_rarityRingSpriteRendererRef;
    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_equipment = new Equipment(m_gameHandlerRef.m_playerStatHandler.m_level);
        m_rarityRingSpriteRendererRef.color = m_equipment.m_rarityTier.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D a_collider)
    {
        //If the coin collides with the player
        if (a_collider.gameObject.GetComponent<Player>())// && !m_movingToTargetPos)
        {
            //Destroy the coin
            Destroy(this.gameObject);
        }
    }
}
