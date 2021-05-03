using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Damageable
{
    float m_duplicationTimer = 0f;
    float m_duplicationTimerMax = 12f;

    BattleManager m_battleManagerHandlerRef;
    private float m_scoreValue = 1f;

    public override void Awake()
    {
        base.Awake();
        m_battleManagerHandlerRef = FindObjectOfType<BattleManager>();
        m_battleManagerHandlerRef.ChangeEnemyCount(1);
    }

    public void Copy(Damageable a_ref)
    {
        m_health = a_ref.m_health;
        //m_maximumHealth = a_ref.m_maximumHealth;
        m_originalMass = a_ref.m_originalMass;
        m_originalColor = a_ref.m_originalColor;
        //m_gameHandlerRef = FindObjectOfType<GameHandler>();

    }

    void Duplicate()
    {
        Vector3 normalisedVelocity = m_rigidBody.velocity.normalized;
        Vector3 spawnLocation = transform.position - normalisedVelocity * 0.4f;
        GameObject clonedObject = Instantiate(gameObject, spawnLocation, new Quaternion());
        Enemy clonedEnemy = clonedObject.GetComponent<Enemy>();
        clonedEnemy.m_rigidBody.velocity = -m_rigidBody.velocity;
        clonedEnemy.Copy(this);
    }

    void DuplicationUpdate()
    {
        m_duplicationTimer += Time.deltaTime;
        if (m_duplicationTimer >= m_duplicationTimerMax)
        {
            if (m_rigidBody.velocity.magnitude != 0.0f)
            {
                Duplicate();
            }
            m_duplicationTimer = 0.0f;
        }
    }

    public override void Die()
    {
        m_battleManagerHandlerRef.ChangeScore(m_scoreValue);
        m_battleManagerHandlerRef.ChangeEnemyCount(-1);
        base.Die();
    }

    public override void OnCollisionEnter2D(Collision2D a_collision)
    {
        base.OnCollisionEnter2D(a_collision);
        if (a_collision.gameObject.GetComponent<Pocket>())
        {
            Die();
        }
    }

    public override void Update()
    {
        base.Update();
        DuplicationUpdate();
    }
}
