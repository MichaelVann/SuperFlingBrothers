using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Damageable
{
    Camera m_cameraRef;
    BattleManager m_battleManagerRef;

    bool m_flinging = false;
    Vector3 m_originalFlingPos;
    const float m_maxFlingLength = 1f;

    LineRenderer m_flingLine;

    bool m_hitSlowdownActive = false;
    float m_enemyHitTimer;
    float m_enemyHitTimerMax = 0.3f;
    bool invertedTime = false;

    float m_hitTimeSlowdownRate = 0.05f;

    float m_upperLowerFlingPositionBounds = 3.7f;

    public SpriteRenderer m_invalidFlingCross;

    //Coins
    float m_damageTextYOffset = 0.2f;
    float m_cumulativeCoinValue = 0f;
    float m_coinPickupTimeout = 0;
    float m_coinPickupTimeoutMax = 1.3f;
    RisingFadingText m_coinValueText;

    public override void Awake()
    {
        base.Awake();
        m_flingLine = GetComponent<LineRenderer>();
        m_flingLine.startColor = Color.red;
        m_flingLine.endColor = Color.white;
        m_flingLine.startWidth = 0.05f;
        m_flingLine.endWidth = 0.02f;
        m_cameraRef = FindObjectOfType<Camera>();
    }

    public override void Start()
    {
        base.Start();
        m_statHandler = m_gameHandlerRef.m_playerStatHandler;
        m_health = m_statHandler.m_stats[(int)eStatIndices.constitution].finalValue;
        m_healthBarRef.SetMaxProgressValue(m_statHandler.m_stats[(int)eStatIndices.constitution].finalValue);
        m_battleManagerRef = FindObjectOfType<BattleManager>();
        m_damageTextColor = Color.red;
    }

    public override void Fling(Vector3 a_flingVector, float a_flingStrength)
    {
        base.Fling(a_flingVector, a_flingStrength);
        m_flinging = false;
        m_battleManagerRef.SetFrozen(false);
    }

    void HandleFlinging()
    {
        m_invalidFlingCross.enabled = false;
        if (!m_flinging)
        {
            m_flingLine.enabled = false;

            if (m_battleManagerRef.m_frozen && Input.GetMouseButton(0))
            {
                m_originalFlingPos = m_cameraRef.ScreenToWorldPoint(Input.mousePosition);

                m_flinging = true;
            }
        }
        else
        {
            m_flingLine.enabled = true;
            Vector3 worldMousePoint = m_cameraRef.ScreenToWorldPoint(Input.mousePosition);

            if (worldMousePoint.y >= m_upperLowerFlingPositionBounds || worldMousePoint.y <= -m_upperLowerFlingPositionBounds)
            {
                m_invalidFlingCross.enabled = true;
                m_invalidFlingCross.gameObject.transform.position = new Vector3(worldMousePoint.x, worldMousePoint.y);
            }
            else
            {
                m_invalidFlingCross.enabled = false;
            }

            Vector3 deltaMousePos = m_originalFlingPos - worldMousePoint;
            if (deltaMousePos.magnitude > m_maxFlingLength)
            {
                deltaMousePos = deltaMousePos.normalized * m_maxFlingLength;
            }

            Vector3[] linePositions = new Vector3[2];

            linePositions[0] = transform.position;
            linePositions[1] = transform.position - deltaMousePos;

            m_flingLine.SetPositions(linePositions);
            
            //If the release point is outside the map, cancel the shot
            if (!Input.GetMouseButton(0))
            {
                if (worldMousePoint.y < m_upperLowerFlingPositionBounds && worldMousePoint.y > -m_upperLowerFlingPositionBounds)
                {
                    Fling(deltaMousePos, m_statHandler.m_stats[(int)eStatIndices.dexterity].finalValue);
                }
                else
                {
                    m_flinging = false;
                }
            }
        }

    }

    public override void Die()
    {
        if (!m_battleManagerRef.m_endingGame)
        {
            base.Die();
            m_battleManagerRef.StartEndingGame(false);
        }
    }

    public override void OnCollisionEnter2D(Collision2D a_collision)
    {
        Enemy enemy = a_collision.gameObject.GetComponent<Enemy>();
        if (enemy)//If collided with an enemy
        {
            if (m_health <= m_statHandler.m_stats[(int)eStatIndices.constitution].finalValue / 3f)//
            {
                if (!m_battleManagerRef.m_endingGame)
                {
                    m_hitSlowdownActive = true;
                    Time.timeScale = m_hitTimeSlowdownRate;
                }
            }
        }

        switch (m_gameHandlerRef.m_currentGameMode)
        {
            case GameHandler.eGameMode.TurnLimit:
                break;
            case GameHandler.eGameMode.Health:
                if (a_collision.gameObject.GetComponent<Pocket>())
                {
                    Damage(m_battleManagerRef.m_pocketDamage);
                }
                else
                {
                    base.OnCollisionEnter2D(a_collision);
                }
                break;
            case GameHandler.eGameMode.Pockets:
                if (a_collision.gameObject.GetComponent<Pocket>())
                {
                    if (m_health <= 0f)
                    {
                        Die();
                    }
                    Damage();
                }
                break;
            default:
                break;
        }
    }
    public void OnTriggerEnter2D(Collider2D a_collider)
    {
        if (a_collider.gameObject.GetComponent<Coin>())
        {
            m_battleManagerRef.ChangeScore(m_battleManagerRef.m_coinValue);

            if (m_coinValueText == null)
            {
                m_coinValueText = Instantiate(m_risingFadingTextPrefab, transform.position + new Vector3(0f, m_damageTextYOffset), new Quaternion(), FindObjectOfType<Canvas>().transform).GetComponent<RisingFadingText>();
                m_cumulativeCoinValue = 0f;
                m_coinValueText.SetImageEnabled(true);
                m_coinValueText.SetOriginalColor(Color.magenta);
                m_coinValueText.SetOriginalScale(1.2f);
                m_coinValueText.SetLifeTimerMax(1.35f);
            }
            else
            {
                m_coinValueText.SetLifeTimer(0f);
                m_coinValueText.SetOriginalPosition(transform.position + new Vector3(0f, m_damageTextYOffset));
            }
            m_cumulativeCoinValue += m_battleManagerRef.m_coinValue;

            m_coinValueText.SetTextContent("+" + m_cumulativeCoinValue);
            Destroy(a_collider.gameObject);
        }
    }

    public override void Damage(float a_damage)
    {
        base.Damage(a_damage);
        m_battleManagerRef.m_healthBarRef.SetBarValue(m_health);
    }

    public override void Update()
    {
        base.Update();
        HandleFlinging();
        if (m_hitSlowdownActive)
        {
            m_enemyHitTimer += Time.deltaTime/m_hitTimeSlowdownRate;
            if (m_enemyHitTimer >= m_enemyHitTimerMax)
            {
                m_hitSlowdownActive = false;
                m_enemyHitTimer = 0f;
                Time.timeScale = 1f;
            }
        }
    }
}