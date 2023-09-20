using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MapConnectionAnchor : MonoBehaviour
{
    public MapNodeConnection[] m_mapNodeConnections;
    public GameObject m_lineRendererRef;
    List<GameObject> m_createdLineRenderers;
    Vector3 m_originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        m_originalPosition = transform.position;
        m_createdLineRenderers = new List<GameObject>();
        Refresh();
    }

    void CreateLine(Vector3 a_endPosition, Color a_startColor, Color a_endColor)
    {
        Vector3[] linePositions = new Vector3[2];
        linePositions[0] = transform.position;
        linePositions[1] = a_endPosition;
        LineRenderer lineRenderer = Instantiate<GameObject>(m_lineRendererRef, this.transform).GetComponent<LineRenderer>();
        m_createdLineRenderers.Add(lineRenderer.gameObject);

        lineRenderer.startColor = a_startColor;
        lineRenderer.endColor = a_endColor;

        lineRenderer.SetPositions(linePositions);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void Refresh()
    {
        for (int i = 0; i < m_createdLineRenderers.Count; i++)
        {
            Destroy(m_createdLineRenderers[i].gameObject);
        }
        m_createdLineRenderers.Clear();

        bool resetPosition = false;

        if (m_mapNodeConnections.Length > 1)
        {
            if (m_mapNodeConnections[0].m_isaFront && m_mapNodeConnections[1].m_isaFront)
            {
                transform.position = Vector3.Lerp(m_mapNodeConnections[0].transform.position, m_mapNodeConnections[1].transform.position, 0.5f);
            }
            else
            {
                resetPosition = true;
            }
        }
        else
        {
            resetPosition = true;
        }

        if (resetPosition)
        {
            transform.position = m_originalPosition;
        }

        for (int i = 0; i < m_mapNodeConnections.Length; i++)
        {
            if (m_mapNodeConnections[i].m_isaFront)
            {
                m_mapNodeConnections[i].CreateFrontLine(transform.position);
                //CreateLine(m_mapNodeConnections[i].transform.position, Color.green, Color.green);
            }
        }
    }
}
