using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityIndicator : MonoBehaviour
{
    Rigidbody2D m_rigidBodyRef;
    LineRenderer m_lineRendererRef;
    Vector3[] m_linePositions;
    float m_lineDivider = 8f;
    // Start is called before the first frame update

    bool m_enabled = true;

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

    }
}
