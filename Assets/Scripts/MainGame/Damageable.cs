using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : BaseObject
{
    public float m_health = 4f;
    public float m_maximumHealth = 4f;

    public float m_lastVelocityMagnitude = 0f;

    public float m_originalMass;
    public Color m_originalColor;

    public GameObject m_explosionTemplate;

    public float GetHealthPercentage() { return m_health / m_maximumHealth; }

    public override void Awake()
    {
        base.Awake();
        m_originalMass = m_rigidBody.mass;
        m_originalColor = m_spriteRenderer.color;
        m_health = m_maximumHealth;
    }

    public override void Update()
    {
        base.Update();
        m_lastVelocityMagnitude = m_rigidBody.velocity.magnitude;
    }

    void UpdateMass()
    {
        m_rigidBody.mass = m_originalMass *= (GetHealthPercentage());
    }

    void UpdateHealthColor()
    {
        float divider = GetHealthPercentage();
        m_spriteRenderer.color = new Color(m_originalColor.r * divider, m_originalColor.g * divider, m_originalColor.b * divider, m_originalColor.a);
    }

    public void Damage()
    {
        if (m_health >= 2f)
        {
            m_health -= 1f;
        }
        UpdateMass();
        UpdateHealthColor();
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
        else if(a_collision.gameObject.GetComponent<Pocket>())
        {
            Die();
        }
    }
}
