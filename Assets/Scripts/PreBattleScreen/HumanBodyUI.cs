using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HumanBodyUI : MonoBehaviour
{
    // Start is called before the first frame update

    public MapNode[] m_mapNodes;
    public MapNodeConnection[] m_mapNodeConnections;
    public MapConnectionAnchor[] m_mapConnectionAnchors;
    GameHandler m_gameHandlerRef;
    MapHandler m_mapHandlerRef;
    HumanBody m_humanBodyRef;


    //Battle Nodes


    void Start()
    {
        for (int i = 0; i < m_mapNodes.Length; i++)
        {
            m_mapNodes[i].Refresh();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp(HumanBody a_part)
    {
        
    }

    public void Initialise()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_mapHandlerRef = FindObjectOfType<MapHandler>();
        m_humanBodyRef = m_gameHandlerRef.m_humanBody;
        m_mapNodes = FindObjectsOfType<MapNode>();
        m_mapConnectionAnchors = FindObjectsOfType<MapConnectionAnchor>();
        m_mapNodeConnections = FindObjectsOfType<MapNodeConnection>();
        AssignTownsToMapNodes();
    }

    void AssignTownsToMapNodes()
    {
        for (int i = 0; i < m_mapNodes.Length; i++)
        {
            for (int j = 0; j < m_humanBodyRef.m_towns.Count; j++)
            {
                if (m_mapNodes[i].m_name == m_humanBodyRef.m_towns[j].m_name)
                {
                    m_mapNodes[i].SetTown(m_humanBodyRef.m_towns[j]);
                }
            }
        }
    }

    public MapNode GetResidingMapNode()
    {

        MapNode returnNode = null;
        returnNode = GetMapNodeByName(m_humanBodyRef.m_playerResidingTown.m_name);
        return returnNode;
    }

    public MapNode GetMapNodeByName(string a_name)
    {
        MapNode returnNode = null;
        for (int i = 0; i < m_mapNodes.Length; i++)
        {
            if (m_mapNodes[i].m_name == a_name)
            {
                returnNode = m_mapNodes[i];
            }
        }
        return returnNode;
    }

    public void SelectNode(UIBattleNode a_selectedNode)
    {
        m_mapHandlerRef.SelectNode(a_selectedNode);
    }

    public void Refresh()
    {
        for (int i = 0; i < m_mapNodes.Length; i++)
        {
            m_mapNodes[i].Refresh();
        }
        for (int i = 0; i < m_mapConnectionAnchors.Length; i++)
        {
            m_mapConnectionAnchors[i].Refresh();
        }
    }
}
