using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinChest : Damageable
{
    [SerializeField] GameObject m_coinPrefab;
    [SerializeField] GameObject m_equipmentDropPrefab;
    [SerializeField] GameObject m_popParticlesPrefab;
    internal int m_coinCount = 4;
    const float m_equipmentDropChance = 0.5f;
    float m_shake = 0f;
    const float m_shakeDecay = 0.98f;
    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();
    }

    // Update is called once per frame
     public override void Update()
    {
        //if (m_battleManagerRef.m_endingGame && m_battleManagerRef.m_endGameType == eEndGameType.win)
        //{
        //    PopChest(true);
        //}
        SetShakeAmount(m_shake);
        m_shake *= m_shakeDecay;
        base.Update();
    }

    public void Init(int a_coinCount)
    {
        m_coinCount = a_coinCount;
    }

    void PopChest(bool a_instantDispersal)
    {
        Loot.SpawnLoot(m_battleManagerRef, m_coinPrefab, 0.3f, transform.position, m_coinCount, a_instantDispersal);
        int equipmentDrops = 0;
        while (VLib.vRandom(0f, 1f) > m_equipmentDropChance)
        {
            equipmentDrops++;
        }
        Loot.SpawnLoot(m_battleManagerRef, m_equipmentDropPrefab, 0.3f, transform.position, equipmentDrops, a_instantDispersal);

        Instantiate(m_popParticlesPrefab, transform.position, Quaternion.AngleAxis(-90f, new Vector3(1f, 0f, 0f)));
        m_battleManagerRef.m_gameHandlerRef.m_audioHandlerRef.PlayChestPopSound();
        Destroy(gameObject);
    }

    internal override void ReceiveDamageFromCollision(GameObject a_collidingObject, float a_damage, Vector2 a_contactPoint)
    {
        if (a_collidingObject.gameObject.GetComponent<Player>())
        {
            base.ReceiveDamageFromCollision(a_collidingObject, a_damage,a_contactPoint);
        }
    }

    public override void OnCollisionEnter2D (Collision2D a_collider)
    {
        //Do not call base collision function which deals damage to oppDamageable
    }

    public override void Die()
    {
        PopChest(false);
        Destroy(gameObject);
    }
}
