using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Damageable
{
    Camera m_cameraRef;

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

    public override void Awake()
    {
        base.Awake();
        m_flingLine = GetComponent<LineRenderer>();
        m_flingLine.startColor = Color.red;
        m_flingLine.endColor = Color.white;
        m_flingLine.startWidth = 0.05f;
        m_flingLine.endWidth = 0.02f;

        m_cameraRef = FindObjectOfType<Camera>();
        m_stats.flingStrength = 500f;
        m_stats.strength = 100f;
    }

    public override void Start()
    {
        base.Start();
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
                    Fling(deltaMousePos, m_stats.flingStrength);
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
        if (enemy)
        {
            if (m_stats.health <= 1f)// || enemy.m_stats.health <= 1f)
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
                base.OnCollisionEnter2D(a_collision);
                if (a_collision.gameObject.GetComponent<Pocket>())
                {
                    Damage(m_battleManagerRef.m_pocketDamage);
                }
                break;
            case GameHandler.eGameMode.Pockets:
                if (a_collision.gameObject.GetComponent<Pocket>())
                {
                    if (m_stats.health <= DamageableStats.minHealth)
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
