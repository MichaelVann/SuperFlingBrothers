using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Damageable
{
    Player m_playerRef;

    enum EnemyType
    {
        Idlers,
        Strikers,
        Dodgers,
    }

    float m_duplicationTimer = 0f;
    float m_duplicationTimerMax = 12f;

    private float m_scoreValue = 1f;
    int m_xpReward = 5;

    float m_xpTextYOffset = 0.2f;

    float m_sightRadius = 4f;
    float m_flingTimer = 0f;
    float m_flingTimerMax = 3f;

    float m_flingAccuracy = 20f;

    bool m_flinging;

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        m_battleManagerRef.ChangeEnemyCount(1);
        m_playerRef = FindObjectOfType<Player>();
        m_flingTimer -= UnityEngine.Random.Range(0f, 0.3f);
    }

    public void Copy(Damageable a_ref)
    {
        m_stats.health = a_ref.m_stats.health;
        m_originalMass = a_ref.m_originalMass;
        m_originalColor = a_ref.m_originalColor;
        UpdateHealthColor();
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

    public override void OnCollisionEnter2D(Collision2D a_collision)
    {
        base.OnCollisionEnter2D(a_collision);
        if (a_collision.gameObject.GetComponent<Pocket>())
        {
            switch (m_gameHandlerRef.m_currentGameMode)
            {
                case GameHandler.eGameMode.TurnLimit:
                    break;
                case GameHandler.eGameMode.Health:
                    Damage(m_battleManagerRef.m_pocketDamage);
                    break;
                case GameHandler.eGameMode.Pockets:
                    Die();
                    break;
                case GameHandler.eGameMode.Hunger:
                    break;
                case GameHandler.eGameMode.ModeCount:
                    break;
                default:
                    break;
            }
        }
    }

    public override void Update()
    {
        base.Update();
        DuplicationUpdate();
        AIUpdate();
    }
    public override void Die()
    {
        m_battleManagerRef.ChangeScore(m_scoreValue);
        m_gameHandlerRef.ChangeXP(m_xpReward);

        RisingFadingText xpText = Instantiate(m_risingFadingTextTemplate, transform.position + new Vector3(0f, m_xpTextYOffset), new Quaternion(), FindObjectOfType<Canvas>().transform).GetComponent<RisingFadingText>();
        xpText.SetTextContent("XP +" + m_xpReward);
        xpText.SetOriginalColor(Color.cyan);

        m_battleManagerRef.ChangeEnemyCount(-1);
        base.Die();
    }

    public void Fling()
    {
        if (m_playerRef)
        {
            Vector3 playerPos = m_playerRef.transform.position;

            if ((playerPos - transform.position).magnitude <= m_sightRadius)
            {

                Vector3 inaccurateFlingVector = (playerPos - transform.position).normalized;

                Vector3 aimDisturbance = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, m_flingAccuracy) - m_flingAccuracy / 2f, Vector3.forward) * inaccurateFlingVector;

                Fling(aimDisturbance, m_stats.m_flingStrength);
            }
        }
       
    }

    //AI
    private void AIUpdate()
    {
        if(true)
        {
            m_flingTimer += Time.deltaTime;
            if (m_flingTimer >= m_flingTimerMax)
            {
                m_flingTimer -= m_flingTimerMax;

                Fling();
            }
            return;
        }

        //m_flingTimer += Time.deltaTime;
        if (m_battleManagerRef.m_frozen)//m_flingTimer >= m_flingTimerMax)
        {
            //m_flingTimer -= m_flingTimerMax;
            if (!m_flinging)
            {
                m_flinging = true;

                Vector3 playerPos = m_playerRef.transform.position;

                if ((playerPos - transform.position).magnitude <= m_sightRadius)
                {
                    Fling((playerPos - transform.position).normalized, m_stats.m_flingStrength);
                }
            }
        }
        else
        {
            m_flinging = false;
        }


    }

    public override void Fling(Vector3 a_flingVector, float a_flingStrength)
    {
        base.Fling(a_flingVector, a_flingStrength);
        //m_rigidBody.AddForce(a_flingVector * a_flingStrength);
        //m_battleManagerRef.SetFrozen(false);
    }


}
