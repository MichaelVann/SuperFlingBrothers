using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : BaseObject
{
    public float m_health = 4f;
    public const float m_maximumHealth = 4f;
    protected const float m_minimumHealth = 0f;

    public float m_lastVelocityMagnitude = 0f;
    float m_damagePerSpeedDivider = 8f;

    const float m_massDivider = 2f;
    public float m_originalMass;
    public Color m_originalColor;

    public GameObject m_explosionTemplate;
    public GameObject m_collisionSparkTemplate;

    public GameObject m_risingFadingTextTemplate;
    float m_damageTextYOffset = 0.2f;

    static Func<int, Collision2D> CollisionFuncPTR = null;

    Color[] m_healthColours;

    protected float m_flingStrength = 259f;//actual should be 250-ish
    bool m_secondFling = true;
    float m_bumpFlingStrengthMult = 0.25f;
    float m_flingTimer = 0f;
    float m_flingTimerMax = 0.1f;
    Vector3 m_storedFlingVector;
    float m_storedFlingStrength = 0f;

    bool m_clearVelocityOption = true;

    public float GetHealthPercentage() { return m_health / m_maximumHealth; }

    public override void Awake()
    {
        base.Awake();
        m_originalMass = m_rigidBody.mass;
        m_originalColor = m_spriteRenderer.color;
        m_health = m_maximumHealth;

        m_healthColours = new Color[]{Color.red, Color.yellow,Color.green,Color.blue};
        UpdateHealthColor();
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
        if (m_health > m_minimumHealth)
        {
            m_health -= a_damage;
            m_health = Mathf.Clamp(m_health, m_minimumHealth, m_maximumHealth);
            Instantiate(m_collisionSparkTemplate, transform.position, new Quaternion(), transform);
            RisingFadingText damageText = Instantiate(m_risingFadingTextTemplate, transform.position + new Vector3(0f, m_damageTextYOffset), new Quaternion(), FindObjectOfType<Canvas>().transform).GetComponent<RisingFadingText>();
            damageText.SetTextContent(a_damage);
            damageText.SetOriginalColor(Color.red);
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
            if (oppDamageable.m_lastVelocityMagnitude >= m_lastVelocityMagnitude)
            {
                Damage(oppDamageable.m_lastVelocityMagnitude/ m_damagePerSpeedDivider);
            }
        }
    }

    public override void Update()
    {
        base.Update();
        m_lastVelocityMagnitude = m_rigidBody.velocity.magnitude;
        SecondFlingUpdate();
    }
}
