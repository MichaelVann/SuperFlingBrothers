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
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_mapHandlerRef = FindObjectOfType<MapHandler>();
        m_bodyPartRef = m_gameHandlerRef.m_humanBody.m_activeBodyPart;
        for (int i = 0; i < m_mapNodes.Length; i++)
        {
            m_mapNodes[i].SetTown(m_bodyPartRef.m_towns[i]);
        }
        Refresh();
        //UIBattleNode[] battleNodes = m_nodesContainerRef.GetComponentsInChildren<UIBattleNode>();
        //for (int i = 0; i < battleNodes.Length; i++)
        //{
        //    m_UIBattleNodeList.Add(battleNodes[i]);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp(BodyPart a_part)
    {
        
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
