using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    BattleManager m_battleManagerRef;
    Player m_playerRef;
    float m_endSpeed = 5f;

    float m_targetSpeed = 30f;
    internal bool m_movingToTargetPos = false;
    Vector3 m_startingPosition;
    Vector3 m_shadowStartingPosition;
    Vector3 m_targetPosition;
    Vector3 m_originalScale;
    const float m_jumpHeight = 0.5f;
    float m_jumpTimer = 0f;
    float m_jumpTimerMax = 1f;
    public ObjectShadow m_shadowRef;
    const float m_pushOutSpeed = 10f;

    // Start is called before the first frame update
    virtual public void Start()
    {
        m_battleManagerRef = FindObjectOfType<BattleManager>();
        m_playerRef = FindObjectOfType<Player>();
    }

    virtual public void Init(Vector3 a_targetPosition)
    {
        m_targetPosition = a_targetPosition;
        m_startingPosition = transform.position;
        m_originalScale = transform.localScale;
        m_movingToTargetPos = true;
        GetComponent<CircleCollider2D>().enabled = false;
    }

    // Update is called once per frame
    virtual public void Update()
    {
        //If the player has won the game
        if (m_battleManagerRef.m_endingGame && m_battleManagerRef.m_endGameType == eEndGameType.win)
        {
            GetComponent<CircleCollider2D>().enabled = true;
            //Fly the coin towards the player
            if (m_playerRef == null)
            {
                m_playerRef = FindObjectOfType<Player>();
            }
            if (m_playerRef != null)
            {
                Vector3 deltaPos = m_playerRef.transform.position - transform.position;
                transform.position += deltaPos.normalized * m_endSpeed * Time.deltaTime;
                m_endSpeed = Mathf.Pow(m_endSpeed, 1f + 2f * Time.deltaTime);
                m_endSpeed = Mathf.Clamp(m_endSpeed, 0f, 30f);
                m_movingToTargetPos = false;
            }

        }
        else if (m_movingToTargetPos)
        {
            m_jumpTimer = Mathf.Clamp(m_jumpTimer + Time.deltaTime, 0f, m_jumpTimerMax);

            transform.position = Vector3.Lerp(m_startingPosition, m_targetPosition, m_jumpTimer);
            Vector3 jumpVector = new Vector3(0f, Mathf.Sin(m_jumpTimer * Mathf.PI) * m_jumpHeight, 0f);
            transform.position += jumpVector;
            m_shadowRef.m_height = jumpVector.y;
            transform.localScale = m_originalScale * (1f +(jumpVector.y/1f));


            if (m_jumpTimer >= m_jumpTimerMax)
            {
                m_movingToTargetPos = false;
                GetComponent<CircleCollider2D>().enabled = true;
            }
        }
    }

    public void OnTriggerStay2D(Collider2D a_collider)
    {
        if (!m_battleManagerRef.m_endingGame)
        {
            Vector3 pushOutVector = new Vector3();
            if (a_collider.gameObject.GetComponent<Nucleus>())
            {
                Vector3 nucleusPos = a_collider.gameObject.transform.position.ToVector2();
                Vector3 pos = transform.position.ToVector2();
                pushOutVector = (pos - nucleusPos).normalized;
            }
            else if (a_collider.gameObject.GetComponent<Pocket>())
            {
                Vector3 pos = transform.position.ToVector2();
                pushOutVector = -pos.normalized;
            }
            pushOutVector *= Time.deltaTime * m_pushOutSpeed;
            transform.position += pushOutVector;
        }
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
