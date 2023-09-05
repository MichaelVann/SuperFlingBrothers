using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BodyPartUI : MonoBehaviour
{
    // Start is called before the first frame update

    public MapNode[] m_mapNodes;
    public MapNodeConnection[] m_mapNodeConnections;
    GameHandler m_gameHandlerRef;
    MapHandler m_mapHandlerRef;
    BodyPart m_bodyPartRef;


    //Battle Nodes


    void Start()
    {
        Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp(BodyPart a_part)
    {
        
    }

    public void Initialise()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_mapHandlerRef = FindObjectOfType<MapHandler>();
        m_bodyPartRef = m_gameHandlerRef.m_humanBody.m_activeBodyPart;
        for (int i = 0; i < m_mapNodes.Length; i++)
        {
            m_mapNodes[i].SetTown(m_bodyPartRef.m_towns[i]);
        }
    }

    public MapNode GetResidingMapNode()
    {

        MapNode returnNode = null;
        returnNode = GetMapNodeByName(m_bodyPartRef.m_owningHumanBodyRef.m_playerResidingTown.m_name);
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
    }
}
