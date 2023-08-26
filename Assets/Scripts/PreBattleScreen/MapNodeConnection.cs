using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNodeConnection : MonoBehaviour
{

    public MapNode[] m_mapNodes;
    public LineRenderer[] m_lineRenderers;
    public GameObject[] m_anchorPoints;

    // Start is called before the first frame update
    void Start()
    {
        SetUpLines();
    }

    void SetUpLines()
    {
        Vector3[] linePositions = new Vector3[2];
        bool isFront = m_mapNodes[0].m_occupied != m_mapNodes[1].m_occupied;

        for (int i = 0; i < m_mapNodes.Length; i++)
        {
            linePositions[0] = transform.position;
            linePositions[1] = m_mapNodes[i].transform.position;
            Color lineColor = Color.white;

            lineColor = m_mapNodes[i].m_occupied ? Color.red : Color.green;


            m_lineRenderers[i].startColor = m_lineRenderers[i].endColor = lineColor;


            m_lineRenderers[i].SetPositions(linePositions);
        }
        if (isFront)
        {
            for (int i = 0; i < m_anchorPoints.Length; i++)
            {
                linePositions[0] = transform.position;
                linePositions[1] = m_anchorPoints[i].transform.position;

                m_lineRenderers[i+2].startColor = m_lineRenderers[i+2].endColor = Color.red;

                m_lineRenderers[i+2].SetPositions(linePositions);
            }
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
