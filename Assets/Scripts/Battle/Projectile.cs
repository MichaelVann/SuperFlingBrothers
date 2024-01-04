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
    bool m_canBounce = true;
    Rigidbody2D m_rigidbody;
    int m_powerUpLevel = 0;
    int m_maxPowerUpLevel = 3;
    EquipmentAbility m_parentAbility;
    bool m_friendly = true;
    bool m_stunning = false;
    float m_minimumVelocity = 1f;
    public GameObject m_explosionPrefab;

    public GameObject m_temporarySpritePrefab;
    public Sprite m_webSpriteRef;

    // Start is called before the first frame update
    void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_rigidbody.velocity.magnitude < m_minimumVelocity)
        {
            Die();
        }
        //transform.rotation = VLib.Vector2DirectionToQuaternion(m_rigidbody.velocity);
    }

    void Die()
    {
        Destroy(this.gameObject);
        Instantiate(m_explosionPrefab, transform.position, new Quaternion());
    }

    public void Initialise(EquipmentAbility a_ability, Vector2 a_shootVector, Vector2 a_parentVelocity, float a_damage)
    {
        m_parentAbility = a_ability;
        m_rigidbody.velocity = a_parentVelocity + a_shootVector.normalized * m_speed;
        m_damage = a_ability.m_abilityType == EquipmentAbility.eAbilityType.Bullet ? a_damage : 0f;
        if (m_parentAbility.HasAffix(EquipmentAbility.eAffix.bouncePowerup))
        {
            m_bouncePowerUp = true;
        }
        if (m_parentAbility.m_abilityType == EquipmentAbility.eAbilityType.Snare)
        {
            m_stunning = true;
            //m_canBounce = false;
        }
    }

    public void OnCollisionEnter2D(Collision2D a_collision)
    {
        if (a_collision.gameObject.GetComponent<Rigidbody2D>())
        {
            if (a_collision.gameObject.GetComponent<Damageable>())
            {
                float effect = m_damage * m_rigidbody.mass;
                bool hitFriendlyAligned = false;
                if (a_collision.gameObject.GetComponent<Nucleus>())
                {
                    hitFriendlyAligned = true;
                }

                bool hitEnemy = hitFriendlyAligned ^ m_friendly;

                if (hitEnemy)
                {
                    if (m_stunning)
                    {
                        float m_stunTime = 5f;
                        a_collision.gameObject.GetComponent<Damageable>().Stun(m_stunTime);
                        TemporarySprite web = Instantiate<GameObject>(m_temporarySpritePrefab,a_collision.gameObject.transform.position, Quaternion.identity, a_collision.gameObject.transform).GetComponent<TemporarySprite>();
                        web.Init(m_stunTime,m_webSpriteRef);
                    }
                    a_collision.gameObject.GetComponent<Damageable>().Damage(effect, a_collision.GetContact(0).point);
                }
                else
                {
                    a_collision.gameObject.GetComponent<Damageable>().Heal(effect);
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
