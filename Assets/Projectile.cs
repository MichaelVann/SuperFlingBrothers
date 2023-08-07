using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    internal float m_damage = 1f;
    internal float m_speed = 10f;
    public bool m_playerDamaging = false;
    public bool m_enemyDamaging = false;
    Rigidbody2D m_rigidbody;

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

    public void Initialise(Vector2 a_shootVector, Vector2 a_parentVelocity, float a_damage)
    {

        m_rigidbody.velocity = a_parentVelocity + a_shootVector.normalized * m_speed;
        m_damage = a_damage;
    }

    public void OnCollisionEnter2D(Collision2D a_collision)
    {
        if (a_collision.gameObject.GetComponent<Rigidbody2D>())
        {
            if (a_collision.gameObject.GetComponent<Damageable>())
            {
                a_collision.gameObject.GetComponent<Damageable>().Damage(m_damage * m_rigidbody.mass);
            }
            Destroy(this.gameObject);
        }
    }
}
