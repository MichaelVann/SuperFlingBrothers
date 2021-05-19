using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    BattleManager m_battleManagerRef;
    Player m_playerRef;
    float m_coinValue = 1f;

    float m_speed = 5f;

    public GameObject m_risingTextPrefab;
    float m_damageTextYOffset = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        m_battleManagerRef = FindObjectOfType<BattleManager>();
        m_playerRef = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_battleManagerRef.m_endingGame && m_battleManagerRef.m_victory)
        {
            transform.position += (m_playerRef.transform.position - transform.position).normalized * m_speed * Time.deltaTime;
            m_speed = Mathf.Pow(m_speed, 1.003f);
        }
    }

    public void OnTriggerEnter2D(Collider2D a_collider)
    {
        if (a_collider.gameObject.GetComponent<Player>())
        {
            FindObjectOfType<BattleManager>().ChangeScore(m_coinValue);

            RisingFadingText valueText = Instantiate(m_risingTextPrefab, transform.position + new Vector3(0f, m_damageTextYOffset), new Quaternion(), FindObjectOfType<Canvas>().transform).GetComponent<RisingFadingText>();
            valueText.SetTextContent("+" + m_coinValue);
            valueText.SetOriginalColor(Color.yellow);
            Destroy(this.gameObject);
        }
    }
}
