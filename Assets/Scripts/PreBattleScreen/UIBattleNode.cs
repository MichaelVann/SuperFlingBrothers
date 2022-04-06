using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBattleNode : MonoBehaviour
{
    // Start is called before the first frame update

    public List<GameObject> m_connectionList;

    public int m_invaders = 0;
    public int m_id = 0;

    LineRenderer[] m_lineRenderers;

    public Material m_lineMaterial;

    void Start()
    {
        m_lineRenderers = new LineRenderer[m_connectionList.Count];
        for (int i = 0; i < m_connectionList.Count; i++)
        {
            m_lineRenderers[i] = gameObject.AddComponent<LineRenderer>();
            m_lineRenderers[i].startWidth = m_lineRenderers[i].endWidth = 0.02f;
            m_lineRenderers[i].startColor = m_lineRenderers[i].endColor = Color.green;
            m_lineRenderers[i].material = m_lineMaterial;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < m_connectionList.Count; i++)
        {
            m_lineRenderers[i].SetPosition(0, new Vector3(transform.position.x, transform.position.y, 0f));
            m_lineRenderers[i].SetPosition(1, new Vector3(m_connectionList[i].transform.position.x, m_connectionList[i].transform.position.y, 0f));
        }
    }
}
