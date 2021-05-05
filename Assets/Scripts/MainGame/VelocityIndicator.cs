﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityIndicator : MonoBehaviour
{
    Rigidbody2D m_rigidBodyRef;
    LineRenderer m_lineRendererRef;
    Vector3[] m_linePositions;
    float m_lineDivider = 8f;
    // Start is called before the first frame update

    void Start()
    {
        m_rigidBodyRef = GetComponentInParent<Rigidbody2D>();
        m_lineRendererRef = GetComponent<LineRenderer>();
        //m_lineRendererRef.startColor = Color.green;
        //m_lineRendererRef.endColor = Color.green;
        m_lineRendererRef.startWidth = 0.05f;
        m_lineRendererRef.endWidth = 0.02f;

        m_linePositions = new Vector3[2];
    }

    // Update is called once per frame
    void Update()
    {
        m_lineRendererRef.enabled = m_rigidBodyRef.velocity.magnitude <= 0.3f ? false : true;

        m_linePositions[0] = m_rigidBodyRef.transform.position;
        m_linePositions[1] = m_rigidBodyRef.transform.position + new Vector3(m_rigidBodyRef.velocity.x, m_rigidBodyRef.velocity.y, 0.0f) / m_lineDivider;
        m_lineRendererRef.SetPositions(m_linePositions);
    }
}
