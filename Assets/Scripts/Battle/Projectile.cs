using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Light m_lightRef;
    internal float m_damage = 1f;
    internal float m_speed = 10f;
    public bool m_playerDamaging = false;
    public bool m_enemyDamaging = false;
    bool m_bouncePowerUp = false;
    Rigidbody2D m_rigidbody;
    int m_powerUpLevel = 0;
    int m_maxPowerUpLevel = 3;
    ActiveAbility m_parentAbility;
    bool m_healsNucleus = true;

    // Start is called before the first frame update
    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = VLib.Vector2DirectionToQuaternion(m_rigidbody.velocity);
    }

    public void Initialise(ActiveAbility a_ability, Vector2 a_shootVector, Vector2 a_parentVelocity, float a_damage)
    {
        m_parentAbility = a_ability;
        m_rigidbody.velocity = a_parentVelocity + a_shootVector.normalized * m_speed;
        m_damage = a_damage;
        if (m_parentAbility.HasAffix(ActiveAbility.eAffix.bouncePowerup))
        {
            m_bouncePowerUp = true;
        }
    }

    public void OnCollisionEnter2D(Collision2D a_collision)
    {
        if (a_collision.gameObject.GetComponent<Rigidbody2D>())
        {
            if (a_collision.gameObject.GetComponent<Damageable>())
            {
                float effect = m_damage * m_rigidbody.mass;
                if (a_collision.gameObject.GetComponent<Nucleus>())
                {
                    a_collision.gameObject.GetComponent<Damageable>().Heal(effect);
                }
                else
                {
                    a_collision.gameObject.GetComponent<Damageable>().Damage(effect);
                }
                Destroy(this.gameObject);
            }
        }

        if (m_bouncePowerUp && m_powerUpLevel < m_maxPowerUpLevel)
        {
            m_powerUpLevel++;
            m_damage *= 2f;
            Color powerColor = Color.white;
            switch (m_powerUpLevel)
            {
                case 1:
                    powerColor = Color.yellow;
                    break;
                case 2:
                    powerColor = new Color(1f, 137f/256f, 0f,1f); //Orange
                    break;
                case 3:
                    powerColor = Color.red; //Orange
                    break;
                default:
                    break;
            }
            gameObject.GetComponent<SpriteRenderer>().color = powerColor;
            m_lightRef.color = powerColor;
        }
    }
}
