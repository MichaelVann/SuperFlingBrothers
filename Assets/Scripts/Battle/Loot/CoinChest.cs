using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinChest : Loot
{
    [SerializeField] GameObject m_coinPrefab;
    internal int m_coinCount = 4;
    
    // Start is called before the first frame update
    override public void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    override public void Update()
    {
        if (m_battleManagerRef.m_endingGame && m_battleManagerRef.m_endGameType == eEndGameType.win)
        {
            PopChest();
        }
    }

    public void Init(int a_coinCount)
    {
        m_coinCount = a_coinCount;
    }

    void PopChest()
    {
        Loot.SpawnLoot(m_battleManagerRef, m_coinPrefab, 0.3f, transform.position, m_coinCount);
        Destroy(gameObject);
    }

    public override void OnTriggerEnter2D(Collider2D a_collider)
    {
        if (a_collider.gameObject.GetComponent<Player>())
        {
            PopChest();
        }
    }
}
