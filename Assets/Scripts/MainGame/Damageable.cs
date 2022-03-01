using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : BaseObject
{
    protected GameHandler m_gameHandlerRef;
    protected BattleManager m_battleManagerRef;

    public float m_lastMomentumMagnitude = 0f;
    float m_damagePerSpeedDivider = 8f;

    const float m_massDivider = 2f;
    public float m_originalMass;
    public Color m_originalColor;

    public GameObject m_explosionPrefab;
    public GameObject m_collisionSparkPrefab;

    public GameObject m_risingFadingTextPrefab;
    float m_damageTextYOffset = 0.2f;
    protected Color m_damageTextColor = Color.yellow;

    static Func<int, Collision2D> CollisionFuncPTR = null;

    public ProgressBar m_healthBarRef;

    bool m_secondFling = true;
    float m_bumpFlingStrengthMult = 0.25f;
    float m_secondFlingTimer = 0f;
    float m_secondFlingTimerMax = 0.09f;
    Vector3 m_storedFlingVector;
    float m_storedFlingStrength = 0f;

    bool m_clearVelocityOption = true;

    public StatHandler m_statHandler;
    int m_tempCount = 0;

    public float GetHealthPercentage() { return m_statHandler.m_stats[(int)eStatIndices.health].effectiveValue / m_statHandler.m_stats[(int)eStatIndices.maxHealth].effectiveValue; }

    public override void Awake()
    {
        base.Awake();
        m_statHandler = new StatHandler();
        m_statHandler.Init();
        m_originalMass = m_rigidBody.mass;
        m_originalColor = m_spriteRenderer.color;
    }

    public virtual void Start()
    {
        m_battleManagerRef = FindObjectOfType<BattleManager>();
        m_gameHandlerRef = m_battleManagerRef.m_gameHandlerRef;

        UpdateHealthColor();
        m_healthBarRef.SetMaxProgressValue(m_statHandler.m_stats[(int)eStatIndices.maxHealth].effectiveValue);

    }

    void BoundsCheck()
    {

    }

    void UpdateMass()
    {
        m_rigidBody.mass = m_originalMass * 0.33f + (GetHealthPercentage() * 0.77f);
    }

    protected void UpdateHealthColor()
    {
        float divider = 0.25f + 0.75f*GetHealthPercentage();
        m_spriteRenderer.color = new Color(m_originalColor.r * divider, m_originalColor.g * divider, m_originalColor.b * divider, m_originalColor.a);
    }

    void SecondFlingUpdate()
    {
        if (!m_secondFling)
        {
            m_secondFlingTimer += Time.deltaTime;
            if (m_secondFlingTimer >= m_secondFlingTimerMax)
            {
                m_secondFlingTimer = 0f;
                Fling(m_storedFlingVector, m_storedFlingStrength);
                m_secondFling = true;
            }
        }
    }

    public virtual void Fling(Vector3 a_flingVector, float a_flingStrength)
    {
        a_flingStrength *= (0.33f + (GetHealthPercentage() * 0.77f));
        if (m_secondFling)
        {
            if (m_clearVelocityOption)
            {
                m_rigidBody.velocity = new Vector2();
            }
            m_secondFling = false;
            m_rigidBody.AddForce(a_flingVector * a_flingStrength * m_bumpFlingStrengthMult);
            m_storedFlingVector = a_flingVector;
            m_storedFlingStrength = a_flingStrength;
        }
        else
        {
            m_rigidBody.AddForce(a_flingVector * a_flingStrength);
        }
    }

    public virtual void Damage(float a_damage)
    {
        if (m_statHandler.m_stats[(int)eStatIndices.health].effectiveValue > m_statHandler.m_stats[(int)eStatIndices.minHealth].effectiveValue)
        {
            m_statHandler.m_stats[(int)eStatIndices.health].effectiveValue -= a_damage;
            m_statHandler.m_stats[(int)eStatIndices.health].effectiveValue = Mathf.Clamp(m_statHandler.m_stats[(int)eStatIndices.health].effectiveValue, m_statHandler.m_stats[(int)eStatIndices.minHealth].effectiveValue, m_statHandler.m_stats[(int)eStatIndices.maxHealth].effectiveValue);
            Instantiate(m_collisionSparkPrefab, transform.position, new Quaternion(), transform);
            RisingFadingText damageText = Instantiate(m_risingFadingTextPrefab, transform.position + new Vector3(0f, m_damageTextYOffset), new Quaternion(), FindObjectOfType<Canvas>().transform).GetComponent<RisingFadingText>();
            damageText.SetImageEnabled(false);
            damageText.SetGravityAffected(true);
            damageText.SetTextContent(a_damage);
            damageText.SetOriginalColor(m_damageTextColor);
        }
        if(m_statHandler.m_stats[(int)eStatIndices.health].effectiveValue <= m_statHandler.m_stats[(int)eStatIndices.minHealth].effectiveValue)
        {
            if (m_gameHandlerRef.m_currentGameMode == GameHandler.eGameMode.Health)
            {
                Die();
            }
        }
        UpdateMass();
        UpdateHealthColor();
    }

    public void Damage()
    {
        Damage(1f);
    }

    public virtual void Die()
    {
        Instantiate(m_explosionPrefab, transform.position, new Quaternion());
        Destroy(gameObject);
    }

    public virtual void OnCollisionEnter2D(Collision2D a_collision)
    {
        Damageable oppDamageable = a_collision.gameObject.GetComponent<Damageable>();
        if (oppDamageable)
        {
            if (oppDamageable.m_lastMomentumMagnitude >= m_lastMomentumMagnitude)
            {
                Damage(oppDamageable.m_statHandler.m_stats[(int)eStatIndices.strength].effectiveValue * oppDamageable.m_lastMomentumMagnitude / m_damagePerSpeedDivider);
            }
        }
    }

    public override void Update()
    {
        m_tempCount++;
        base.Update();
        m_lastMomentumMagnitude = m_rigidBody.velocity.magnitude * m_rigidBody.mass;
        SecondFlingUpdate();

        if (m_healthBarRef) { m_healthBarRef.SetProgressValue(m_statHandler.m_stats[(int)eStatIndices.health].effectiveValue); }
        
    }
}
