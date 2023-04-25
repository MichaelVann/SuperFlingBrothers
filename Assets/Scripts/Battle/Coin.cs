using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    BattleManager m_battleManagerRef;
    Player m_playerRef;
    public GameObject m_shadowRef;
    float m_endSpeed = 5f;

    float m_targetSpeed = 30f;
    bool m_movingToTargetPos = false;
    Vector3 m_targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        m_battleManagerRef = FindObjectOfType<BattleManager>();
        m_playerRef = FindObjectOfType<Player>();
    }

    public void Init(Vector3 a_targetPosition)
    {
        m_targetPosition = a_targetPosition;
        m_movingToTargetPos = true;
    }

    // Update is called once per frame
    void Update()
    {
        //If the player has won the game
        if (m_battleManagerRef.m_endingGame && m_battleManagerRef.m_endGameType == eEndGameType.win)
        {
            //Fly the coin towards the player
            if (m_playerRef == null)
            {
                m_playerRef = FindObjectOfType<Player>();
            }
            if (m_playerRef != null)
            {
                transform.position += (m_playerRef.transform.position - transform.position).normalized * m_endSpeed * Time.deltaTime;
                m_endSpeed = Mathf.Pow(m_endSpeed, 1.003f);
                m_movingToTargetPos = false;
            }

        }
        else if (m_movingToTargetPos)
        {
            Vector3 deltaPos = m_targetPosition - transform.position;

            float speed = Mathf.Clamp(m_targetSpeed, 0f, deltaPos.magnitude);

            transform.position += deltaPos.normalized * speed * Time.deltaTime;

            if (deltaPos.magnitude <= Mathf.Epsilon)
            {
                m_movingToTargetPos = false;
            }
        }

        float eulerAnglesForShadow = transform.eulerAngles.z + GameHandler.BATTLE_ShadowAngle;
        float x = Mathf.Sin(eulerAnglesForShadow * Mathf.PI / 180f) / transform.localScale.x;
        float y = Mathf.Cos(eulerAnglesForShadow * Mathf.PI / 180f) / transform.localScale.y;
        m_shadowRef.transform.localPosition = new Vector3(x, y) * BattleManager.m_shadowDistance;
    }

    public void OnTriggerEnter2D(Collider2D a_collider)
    {
        //If the coin collides with the player
        if (a_collider.gameObject.GetComponent<Player>() && !m_movingToTargetPos)
        {
            //Destroy the coin
            Destroy(this.gameObject);
        }
    }
}
