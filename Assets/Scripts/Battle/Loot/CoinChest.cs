using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinChest : Loot
{
    [SerializeField] GameObject m_coinPrefab;
    [SerializeField] GameObject m_popParticlesPrefab;
    internal int m_coinCount = 4;
    
    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    override public void Update()
    {
        //if (m_battleManagerRef.m_endingGame && m_battleManagerRef.m_endGameType == eEndGameType.win)
        //{
        //    PopChest(true);
        //}
    }

    public void Init(int a_coinCount)
    {
        m_coinCount = a_coinCount;
    }

    void PopChest(bool a_instantDispersal)
    {
        Loot.SpawnLoot(m_battleManagerRef, m_coinPrefab, 0.3f, transform.position, m_coinCount, a_instantDispersal);
        Instantiate(m_popParticlesPrefab, transform.position, Quaternion.AngleAxis(-90f, new Vector3(1f, 0f, 0f)));
        m_battleManagerRef.m_gameHandlerRef.m_audioHandlerRef.PlayChestPopSound();
        Destroy(gameObject);
    }

    public override void OnTriggerEnter2D(Collider2D a_collider)
    {
        if (a_collider.gameObject.GetComponent<Player>())
        {
            PopChest(false);
        }
    }
}
