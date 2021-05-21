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

    public GameObject m_explosionTemplate;
    public GameObject m_collisionSparkTemplate;

    public GameObject m_risingFadingTextTemplate;
    float m_damageTextYOffset = 0.2f;

    static Func<int, Collision2D> CollisionFuncPTR = null;

    public ProgressBar m_healthBarRef;

    bool m_secondFling = true;
    float m_bumpFlingStrengthMult = 0.25f;
    float m_flingTimer = 0f;
    float m_flingTimerMax = 0.09f;
    Vector3 m_storedFlingVector;
    float m_storedFlingStrength = 0f;

    bool m_clearVelocityOption = true;

    public Stat[] m_stats;

    public float GetHealthPercentage() { return m_stats[(int)eStatIndices.health].value / m_stats[(int)eStatIndices.maxHealth].value; }

    public override void Awake()
    {
        base.Awake();
        m_stats = new Stat[(int)eStatIndices.count];

        m_originalMass = m_rigidBody.mass;
        m_originalColor = m_spriteRenderer.color;
    }

    public virtual void Start()
    {
        m_battleManagerRef = FindObjectOfType<BattleManager>();
        m_gameHandlerRef = m_battleManagerRef.m_gameHandlerRef;

        m_gameHandlerRef.m_playerStatHandler.SetDefaultStats(ref m_stats);

        UpdateHealthColor();
        m_healthBarRef.SetMaxProgressValue(m_stats[(int)eStatIndices.maxHealth].value);

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
            m_flingTimer += Time.deltaTime;
            if (m_flingTimer >= m_flingTimerMax)
            {
                m_flingTimer = 0f;
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

    public void Damage(float a_damage)
    {
        if (m_stats[(int)eStatIndices.health].value > m_stats[(int)eStatIndices.minHealth].value)
        {
            m_stats[(int)eStatIndices.health].value -= a_damage;
            m_stats[(int)eStatIndices.health].value = Mathf.Clamp(m_stats[(int)eStatIndices.health].value, m_stats[(int)eStatIndices.minHealth].value, m_stats[(int)eStatIndices.maxHealth].value);
            Instantiate(m_collisionSparkTemplate, transform.position, new Quaternion(), transform);
            RisingFadingText damageText = Instantiate(m_risingFadingTextTemplate, transform.position + new Vector3(0f, m_damageTextYOffset), new Quaternion(), FindObjectOfType<Canvas>().transform).GetComponent<RisingFadingText>();
            damageText.SetTextContent(a_damage);
            damageText.SetOriginalColor(Color.white);
        }
        if(m_stats[(int)eStatIndices.health].value <= m_stats[(int)eStatIndices.minHealth].value)
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
        Instantiate(m_explosionTemplate, transform.position, new Quaternion());
        Destroy(gameObject);
    }

    public virtual void OnCollisionEnter2D(Collision2D a_collision)
    {
        Damageable oppDamageable = a_collision.gameObject.GetComponent<Damageable>();
        if (oppDamageable)
        {
            if (oppDamageable.m_lastMomentumMagnitude >= m_lastMomentumMagnitude)
            {
                Damage(oppDamageable.m_stats[(int)eStatIndices.strength].value * oppDamageable.m_lastMomentumMagnitude / m_damagePerSpeedDivider);
            }
        }
    }

    public override void Update()
    {
        base.Update();
        m_lastMomentumMagnitude = m_rigidBody.velocity.magnitude * m_rigidBody.mass;
        SecondFlingUpdate();

        if (m_healthBarRef) { m_healthBarRef.SetProgressValue(m_stats[(int)eStatIndices.health].value); }
        
    }
}
