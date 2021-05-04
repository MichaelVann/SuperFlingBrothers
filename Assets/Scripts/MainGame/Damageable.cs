using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : BaseObject
{
    public float m_health = 4f;
    public const float m_maximumHealth = 4f;
    protected const float m_minimumHealth = 1f;

    public float m_lastVelocityMagnitude = 0f;

    const float m_massDivider = 2f;
    public float m_originalMass;
    public Color m_originalColor;

    public GameObject m_explosionTemplate;
    public GameObject m_collisionSparkTemplate;

    static Func<int, Collision2D> CollisionFuncPTR = null;

    Color[] m_healthColours;

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

    public override void Update()
    {
        base.Update();
        m_lastVelocityMagnitude = m_rigidBody.velocity.magnitude;
    }

    void UpdateMass()
    {
        m_rigidBody.mass = m_originalMass * (GetHealthPercentage());
    }

    protected void UpdateHealthColor()
    {
        //m_spriteRenderer.color = m_healthColours[(int)(m_health-1f)];
        float divider = GetHealthPercentage();
        m_spriteRenderer.color = new Color(m_originalColor.r * divider, m_originalColor.g * divider, m_originalColor.b * divider, m_originalColor.a);
    }

    public void Damage(float a_damage)
    {
        if (m_health > m_minimumHealth)
        {
            m_health -= a_damage;
            m_health = Mathf.Clamp(m_health, m_minimumHealth, m_maximumHealth);
            Instantiate(m_collisionSparkTemplate, transform.position, new Quaternion(), transform);
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
        Instantiate(m_explosionTemplate,transform.position, new Quaternion());
        Destroy(gameObject);
    }

    public virtual void OnCollisionEnter2D(Collision2D a_collision)
    {
        Damageable oppDamageable = a_collision.gameObject.GetComponent<Damageable>();
        if (oppDamageable)
        {
            if (oppDamageable.m_lastVelocityMagnitude >= m_lastVelocityMagnitude)
            {
                Damage();
            }
        }
        //else if(a_collision.gameObject.GetComponent<Pocket>())
        //{
        //    Die();
        //}
    }
}
