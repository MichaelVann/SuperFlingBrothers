using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    BattleManager m_battleManagerRef;
    Player m_playerRef;

    float m_speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        m_battleManagerRef = FindObjectOfType<BattleManager>();
        m_playerRef = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        //If the player has won the game
        if (m_battleManagerRef.m_endingGame && m_battleManagerRef.m_victory)
        {
            //Fly the coin towards the player
            transform.position += (m_playerRef.transform.position - transform.position).normalized * m_speed * Time.deltaTime;
            m_speed = Mathf.Pow(m_speed, 1.003f);
        }
    }

    public void OnTriggerEnter2D(Collider2D a_collider)
    {
        //If the coin collides with the player
        if (a_collider.gameObject.GetComponent<Player>())
        {
            //Destroy the coin
            Destroy(this.gameObject);
        }
    }
}
