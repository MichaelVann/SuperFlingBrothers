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

    //Coins
    float m_damageTextYOffset = 0.2f;
    float m_cumulativeCoinValue = 0f;
    float m_coinPickupTimeout = 0;
    float m_coinPickupTimeoutMax = 1.3f;
    RisingFadingText m_coinValueText;

    public SpriteRenderer m_shieldSpriteRenderer;
    bool m_shieldEnabled = false;
    GameHandler.Shield m_shieldRef;
    float m_maxShieldOpactiy = 0.64f;

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
        m_healthBarRef.SetMaxProgressValue(m_statHandler.m_stats[(int)eStatIndices.constitution].finalValue);
        m_battleManagerRef = FindObjectOfType<BattleManager>();
        m_statHandler = m_gameHandlerRef.m_playerStatHandler;
        UpdateLocalStatsFromStatHandler();
        m_damageTextColor = Color.red;
        SetupShield();
        Fling(new Vector3(0f, -600f, 0f), 1f);
    }

    void SetupShield()
    {
        m_shieldRef = m_gameHandlerRef.m_playerShield;
        if (m_gameHandlerRef.m_shieldUpgrade.m_owned)
        {
            m_shieldEnabled = true;
            m_shieldRef.delayTimer = 0f;
            m_shieldRef.value = m_gameHandlerRef.m_playerShield.capacity;
        }
        m_shieldSpriteRenderer.gameObject.SetActive(m_gameHandlerRef.m_shieldUpgrade.m_owned);
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
            m_battleManagerRef.StartEndingGame(eEndGameType.lose);
        }
    }

    public override void OnCollisionEnter2D(Collision2D a_collision)
    {
        Enemy enemy = a_collision.gameObject.GetComponent<Enemy>();
        EscapeZone escapeZone = a_collision.gameObject.GetComponent<EscapeZone>();
        if (enemy)//If collided with an enemy
        {
            if (m_health <= m_statHandler.m_stats[(int)eStatIndices.constitution].finalValue / 3f)//
            {
                if (!m_battleManagerRef.m_endingGame)
                {
                    m_hitSlowdownActive = true;
                    Time.timeScale = m_hitTimeSlowdownRate;
                    m_battleManagerRef.SetTimeScale(m_hitTimeSlowdownRate);
                }
            }
        }
        else if (escapeZone)
        {
            if (!m_battleManagerRef.m_endingGame)
            {
                m_battleManagerRef.Escape();
                m_rigidBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
                transform.position = escapeZone.transform.position;
            }
            
        }

        switch (m_gameHandlerRef.m_currentGameMode)
        {
            case GameHandler.eGameMode.TurnLimit:
                break;
            case GameHandler.eGameMode.Health:
                if (a_collision.gameObject.GetComponent<Pocket>())
                {
                    TakePocketDamage();
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
        float damage = a_damage;
        if (m_shieldEnabled)
        {
            if (m_shieldRef.value >= damage)
            {
                m_shieldRef.value -= damage;
                damage = 0f;
            }
            else
            {
                damage -= m_shieldRef.value;
                m_shieldRef.value = 0f;
            }
            m_shieldRef.delayTimer = 0f;
        }
        base.Damage(damage);
        m_battleManagerRef.m_healthBarRef.SetBarValue(m_health);
    }

    void UpdateShieldOpacity()
    {
        Color shieldColor = m_shieldSpriteRenderer.color;
        shieldColor.a = m_maxShieldOpactiy * m_shieldRef.value/m_shieldRef.capacity;
        m_shieldSpriteRenderer.color = shieldColor;
    }

    void ShieldUpdate()
    {
        if (m_shieldEnabled)
        {
            if (m_shieldRef.delayTimer <= m_shieldRef.delay)
            {
                m_shieldRef.delayTimer += Time.deltaTime;
            }
            else if (m_shieldRef.capacity >= m_shieldRef.value)
            {
                m_shieldRef.value = Mathf.Clamp(m_shieldRef.value + m_shieldRef.rechargeRate * Time.deltaTime, 0f, m_shieldRef.capacity);
            }
            UpdateShieldOpacity();
            m_battleManagerRef.m_shieldBarRef.SetBarValue(m_shieldRef.value);
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
                m_battleManagerRef.SetTimeScale(1f);
            }
        }
        ShieldUpdate();

        if (Input.GetKey(KeyCode.M))
        {
             Damage(100f);
        }
    }
}