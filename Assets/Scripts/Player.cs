﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Damageable
{
    Camera m_cameraRef;

    bool m_frozen = false;
    float m_freezeTimer;
    const float m_freezeTimerMax = 2f;
    bool m_freezing = false;
    float m_freezingTimer = 0f;
    const float m_freezingTimerMax = 0.2f;
    const float m_slowableTime = 0.8f;

    bool m_flinging = false;
    Vector3 m_originalFlingPos;
    const float m_maxFlingLength = 1f;
    const float m_flingStrength = 250f;//actual should be 250-ish

    LineRenderer m_flingLine;

    public override void Awake()
    {
        base.Awake();
        m_freezeTimer = m_freezeTimerMax;
        m_flingLine = GetComponent<LineRenderer>();
        m_flingLine.startColor = Color.red;
        m_flingLine.endColor = Color.white;
        m_flingLine.startWidth = 0.05f;
        m_flingLine.endWidth = 0.02f;

        m_cameraRef = FindObjectOfType<Camera>();
    }

    void Fling(Vector3 a_flingVector)
    {
        //m_rigidBodyRef.velocity = new Vector3();
        m_rigidBody.AddForce(a_flingVector * m_flingStrength);
        m_flinging = false;
        SetFrozen(false);
    }

    void HandleFlinging()
    {
        if (!m_flinging)
        {
            m_flingLine.enabled = false;

            if (m_frozen && Input.GetMouseButton(0))
            {
                m_originalFlingPos = m_cameraRef.ScreenToWorldPoint(Input.mousePosition);

                m_flinging = true;

            }

        }
        else
        {
            m_flingLine.enabled = true;
            Vector3 worldMousePoint = m_cameraRef.ScreenToWorldPoint(Input.mousePosition);
            Vector3 deltaMousePos = worldMousePoint - m_originalFlingPos;
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

    void SetFrozen(bool a_frozen)
    {
        m_frozen = a_frozen;
        Time.timeScale = a_frozen ? 0.0f : 1.0f;
    }

    void UpdateFreezeTimer()
    {
        if (!m_frozen)
        {
            m_freezeTimer += Time.deltaTime;
            if (m_freezeTimer >= m_freezeTimerMax)
            {
                m_freezing = true;
                m_freezeTimer = 0f;
            }

            if (m_freezing)
            {
                m_freezingTimer += Time.deltaTime;
                Time.timeScale = 1f - m_slowableTime * m_freezingTimer / m_freezingTimerMax;
                if (m_freezingTimer >= m_freezingTimerMax)
                {
                    m_freezingTimer = 0f;
                    m_freezing = false;
                    SetFrozen(true);
                }
            }
        }
    }

    public override void OnCollisionEnter2D(Collision2D a_collision)
    {

    }

    public override void Update()
    {
        base.Update();
        UpdateFreezeTimer();
        HandleFlinging();
    }
}
