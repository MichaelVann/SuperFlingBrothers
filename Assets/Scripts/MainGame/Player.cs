﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Damageable
{
    Camera m_cameraRef;
    GameHandler m_gameHandlerRef;
    BattleManager m_battleManagerRef;

    bool m_flinging = false;
    Vector3 m_originalFlingPos;
    const float m_maxFlingLength = 1f;
    const float m_flingStrength = 500f;//actual should be 250-ish

    LineRenderer m_flingLine;

    public override void Awake()
    {
        base.Awake();
        m_flingLine = GetComponent<LineRenderer>();
        m_flingLine.startColor = Color.red;
        m_flingLine.endColor = Color.white;
        m_flingLine.startWidth = 0.05f;
        m_flingLine.endWidth = 0.02f;

        m_cameraRef = FindObjectOfType<Camera>();
    }

    public void Start()
    {
        m_battleManagerRef = FindObjectOfType<BattleManager>();
        m_gameHandlerRef = m_battleManagerRef.m_gameHandlerRef;
    }

    void Fling(Vector3 a_flingVector)
    {
        //m_rigidBodyRef.velocity = new Vector3();
        m_rigidBody.AddForce(a_flingVector * m_flingStrength);
        m_flinging = false;
        m_battleManagerRef.SetFrozen(false);
    }

    void HandleFlinging()
    {
        if (!m_flinging)
        {
            m_flingLine.enabled = false;

            if (m_battleManagerRef.m_frozen && Input.GetMouseButton(0))
            {
                m_originalFlingPos = m_cameraRef.ScreenToWorldPoint(Input.mousePosition);

                m_flinging = true;

            }

        }
        else
        {
            m_flingLine.enabled = true;
            Vector3 worldMousePoint = m_cameraRef.ScreenToWorldPoint(Input.mousePosition);
            Vector3 deltaMousePos = m_originalFlingPos - worldMousePoint;
            Debug.Log(deltaMousePos.magnitude);
            if (deltaMousePos.magnitude > m_maxFlingLength)
            {
                deltaMousePos = deltaMousePos.normalized * m_maxFlingLength;
            }

            Vector3[] linePositions = new Vector3[2];

            linePositions[0] = transform.position;
            linePositions[1] = transform.position - deltaMousePos;

            m_flingLine.SetPositions(linePositions);

            if (!Input.GetMouseButton(0))
            {
                Fling(deltaMousePos);
            }
        }

    }


    public override void Die()
    {

        base.Die();
        m_battleManagerRef.StartEndingGame(false);
    }

    public override void OnCollisionEnter2D(Collision2D a_collision)
    {
        switch (m_gameHandlerRef.m_currentGameMode)
        {
            case GameHandler.eGameMode.TurnLimit:
                break;
            case GameHandler.eGameMode.Health:
                base.OnCollisionEnter2D(a_collision);
                break;
            case GameHandler.eGameMode.Pockets:
                if (a_collision.gameObject.GetComponent<Pocket>())
                {
                    if (m_health <= m_minimumHealth)
                    {
                        Die();
                    }
                    Damage();
                }
                break;
            default:
                break;
        }
    }

    public override void Update()
    {
        base.Update();
        HandleFlinging();
    }
}
