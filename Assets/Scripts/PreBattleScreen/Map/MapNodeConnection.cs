using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapNodeConnection : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public MapNode[] m_mapNodes;
    public GameObject m_lineRendererRef;
    public List<MapNodeConnection> m_adjacentConnections;
    public GameObject[] m_anchorPoints; //These are always drawn to if the connection is activated
    List<GameObject> m_createdLineRenderers;
    HumanBodyUI m_owningBodyPart;
    public Collider2D m_colliderRef;

    //Battle Nodes
    public GameObject m_UIBattleNodeTemplate;
    public List<GameObject> m_nodeGameobjectList;
    int m_maxNodeSpawnAttempts = 20;
    int m_nodesSetupCount = 0;
    TownConnection m_representedTownConnection;

    static Vector3[] m_linePositionsHolder;

    public bool m_isaFront = false;
    bool m_spawningBattles = false;

    Vector3 m_connectionLine;
    Vector3 m_frontStartPos;
    Vector3 m_frontEndPos;

    public Material m_roadMaterial;

    public TextMeshPro m_winningPercentText;

    public void Awake()
    {
        //m_adjacentConnections = new List<MapNodeConnection>();
        m_owningBodyPart = FindObjectOfType<HumanBodyUI>();
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_linePositionsHolder = new Vector3[2];
        m_createdLineRenderers = new List<GameObject>();

        for (int i = 0; i < m_mapNodes.Length; i++)
        {
            m_mapNodes[i].AddConnection(this);
        }
        //CalculateIfIsFront();

        m_representedTownConnection = m_gameHandlerRef.m_humanBody.FindConnection(gameObject.name);

        SetUpLines();


    }

    // Start is called before the first frame update
    void Start()
    {
        //FindAdjacentConnections();
        //SetUpLines();
        //if (m_spawningBattles)
        //{
        //    SetUpBattleNodes();
        //}

        if (m_spawningBattles)
        {
            SetUpBattleNodes();
        }
    }

    void CalculateIfIsFront()
    {
        m_isaFront = m_mapNodes[0].m_overrun != m_mapNodes[1].m_overrun;
        bool firstTownIsPlayers = (m_mapNodes[0].GetTown() == m_gameHandlerRef.m_humanBody.m_playerResidingTown);
        bool secondTownIsPlayers = (m_mapNodes[1].GetTown() == m_gameHandlerRef.m_humanBody.m_playerResidingTown);
        m_spawningBattles = m_isaFront && (firstTownIsPlayers || secondTownIsPlayers);
        m_winningPercentText.gameObject.SetActive(m_isaFront);
        if (m_isaFront)
        {
            m_winningPercentText.text = "" + (m_representedTownConnection.m_warfrontBalance * 100) + "/" + 100;
        }
    }

    void CalculatePosition()
    {
        if (m_mapNodes[0] && m_mapNodes[1])
        {
            if (m_isaFront)
            {
                Vector3 friendlyTownPos;
                Vector3 enemyTownPos;

                if (m_mapNodes[1].GetTown().m_overrun)
                {
                    friendlyTownPos = m_mapNodes[0].transform.position;
                    enemyTownPos = m_mapNodes[1].transform.position;
                }
                else
                {
                    friendlyTownPos = m_mapNodes[1].transform.position;
                    enemyTownPos = m_mapNodes[0].transform.position;
                }

                m_connectionLine = enemyTownPos - friendlyTownPos;

                float bufferSize = 0.15f;

                m_frontStartPos = friendlyTownPos + m_connectionLine.normalized * bufferSize;
                m_frontEndPos = enemyTownPos - m_connectionLine.normalized * bufferSize;

                transform.position = Vector3.Lerp(m_frontStartPos, m_frontEndPos, m_representedTownConnection.m_warfrontBalance);
            }
            else
            {
                transform.position = m_mapNodes[0].transform.position + m_mapNodes[1].transform.position;
                transform.position /= 2f;
            }
        }
    }

    void SetUpBattleNodes()
    {
        if (m_nodeGameobjectList.Count > 0)
        {
            for (int i = 0; i < m_nodeGameobjectList.Count; i++)
            {
                Destroy(m_nodeGameobjectList[i].gameObject);
            }
        }


        MapNode friendlyTown = null;
        MapNode enemyTown = null;

        if (m_mapNodes[1].m_overrun)
        {
            friendlyTown = m_mapNodes[0];
            enemyTown = m_mapNodes[1];
            //connectionLine *= -1;
        }
        else
        {
            friendlyTown = m_mapNodes[0];
            enemyTown = m_mapNodes[1];
        }

        //Find a 90 degree front line
        Vector3 frontLine = new Vector3(m_connectionLine.normalized.y, -m_connectionLine.normalized.x, m_connectionLine.normalized.z);

        for (int i = 0; i < m_representedTownConnection.m_battles.Count; i++)
        {
            GameObject nodeGameObject = Instantiate<GameObject>(m_UIBattleNodeTemplate);//, this.transform);

            bool validSpawnFound = false;
            int spawnAttempts = 0;

            while (!validSpawnFound)
            {
                BattleNode battleNode = m_representedTownConnection.m_battles[i];
                Vector3 spawnPos = new Vector3();// -frontLine / 2f + VLib.vRandom(0f, 1f) * frontLine;
                Vector3 horizontalOffsetVector = -frontLine / 2f + VLib.vRandom(0f, 1f) * frontLine;
                horizontalOffsetVector *= 0.5f;
                Vector3 difficultyOffsetVector = Vector3.Lerp(m_frontStartPos, m_frontEndPos, Mathf.Clamp(battleNode.GetDifficultyPercent(), 0f, 1f));// -m_connectionLine.normalized/2f;
                //difficultyOffsetVector += m_connectionLine.normalized * Mathf.Clamp(battleNode.GetDifficultyPercent(), 0f, 1f);
                //difficultyOffsetVector *= 0.2f;
                //spawnPos =  difficultyOffsetVector;
                spawnPos += difficultyOffsetVector;
                spawnPos += horizontalOffsetVector;
                nodeGameObject.transform.localPosition = spawnPos;

                validSpawnFound = true;
                spawnAttempts++;
                for (int j = 0; j < m_nodeGameobjectList.Count; j++)
                {
                    Collider2D colliderA = nodeGameObject.GetComponent<Collider2D>();
                    Collider2D colliderB = m_nodeGameobjectList[j].GetComponent<Collider2D>();
                    float distMag = (nodeGameObject.transform.localPosition - m_nodeGameobjectList[j].transform.localPosition).magnitude;
                    float allowedRadius = 0.1f;// node.GetComponent<CircleCollider2D>().radius * transform.;
                    if (spawnAttempts > m_maxNodeSpawnAttempts)
                    {
                        nodeGameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                    }
                    if (distMag <= allowedRadius && spawnAttempts <= m_maxNodeSpawnAttempts)
                    {
                        validSpawnFound = false;
                        break;
                    }
                }

                float nameDistMag = (nodeGameObject.transform.position - transform.position).magnitude;
                float nameAllowedRadius = 0.12f;// node.GetComponent<CircleCollider2D>().radius * transform.;

                if (nameDistMag < nameAllowedRadius && spawnAttempts <= m_maxNodeSpawnAttempts)
                {
                    validSpawnFound = false;
                }
            }

            if (spawnAttempts > m_maxNodeSpawnAttempts)
            {
                Destroy(nodeGameObject);
                break;
            }
            else
            {
                //Spawn Node
                UIBattleNode node = nodeGameObject.GetComponent<UIBattleNode>();
                node.SetUp(this, m_representedTownConnection.m_battles[i]);
                node.m_id = m_nodesSetupCount;
                m_nodesSetupCount++;
                //node.m_parentBodyPartID = m_bodyPartID;

                float difficultyPercentage = node.m_battleNodeRef.GetDifficultyPercent();// (float)(node.m_difficulty) / (float)(m_representedTownConnection.m_frontDifficulty);
                Color nodeColor = VLib.PercentageToColor(1f-difficultyPercentage);// VLib.PercentageToColor(1f - (float)(node.m_difficulty - m_minBaseDifficulty) / (float)(m_maxBaseDifficulty - m_minBaseDifficulty));
                if (node.m_difficultyBoostTier > 0)
                {
                    switch (node.m_difficultyBoostTier)
                    {
                        default:
                            nodeColor = Color.black;
                            break;
                        case 1:
                            nodeColor = Color.blue;
                            break;
                        case 2:
                            nodeColor = Color.cyan;
                            break;
                        case 3:
                            nodeColor = Color.magenta;
                            break;
                        case 4:
                            nodeColor = Color.white;
                            break;
                    }
                }
                node.GetComponent<SpriteRenderer>().color = nodeColor;
                m_nodeGameobjectList.Add(nodeGameObject);
                //m_nodeList.Add(node);
            }

        }
        //m_bodyPartSelectionHandler.SetUpBodyPartNodes(m_bodyPartID);
    }

    internal void SelectBattleNode(UIBattleNode a_selectedNode)
    {
        m_owningBodyPart.SelectNode(a_selectedNode);
    }

    int ConnectionSort(MapNodeConnection a_first, MapNodeConnection a_last)
    {
        int returnVal = 0;
        float firstDistance = (a_first.transform.position - transform.position).magnitude;
        float lastDistance = (a_last.transform.position - transform.position).magnitude;
        returnVal = (int)(firstDistance - lastDistance*1000f);
        return returnVal;
    }

    void FindAdjacentConnections()
    {
        for (int i = 0; i < m_mapNodes.Length; i++)
        {
            List<MapNodeConnection> sortedConnectionList = new List<MapNodeConnection>();
            for (int j = 0; j < m_mapNodes[i].m_connectionList.Count; j++)
            {
                if (m_mapNodes[i].m_connectionList[j] != this)
                {
                    sortedConnectionList.Add(m_mapNodes[i].m_connectionList[j]);
                }
            }
            sortedConnectionList.Sort(ConnectionSort);
            while (sortedConnectionList.Count > 2)
            {
                sortedConnectionList.RemoveAt(sortedConnectionList.Count - 1);
            }
            for (int j = 0; j < sortedConnectionList.Count; j++)
            {
                m_adjacentConnections.Add(sortedConnectionList[j]);
            }
        }


        //For both map nodes, if they both have a neighbouring mapnode connected to eachother, add that connection as an adjacent connection
        MapNode nodeA = m_mapNodes[0];
        MapNode nodeB = m_mapNodes[1];
        for (int i = 0; i < nodeA.m_connectionList.Count; i++)
        {
            //Find the connections other node
            MapNode otherNodeA = nodeA.m_connectionList[i].m_mapNodes[0] == nodeA ? nodeA.m_connectionList[i].m_mapNodes[1] : nodeA.m_connectionList[i].m_mapNodes[0];
            if (otherNodeA == nodeB)
            {
                continue;
            }

            for (int j = 0; j < otherNodeA.m_connectionList.Count; j++)
            {
                for (int k = 0; k < nodeB.m_connectionList.Count; k++)
                {
                    MapNode otherNodeB = nodeB.m_connectionList[k].m_mapNodes[0] == nodeB ? nodeB.m_connectionList[k].m_mapNodes[1] : nodeB.m_connectionList[k].m_mapNodes[0];
                    if (otherNodeB == nodeA)
                    {
                        continue;
                    }

                    for (int l = 0; l < otherNodeB.m_connectionList.Count; l++)
                    {
                        if (otherNodeB.m_connectionList[l] == otherNodeA.m_connectionList[j])
                        {
                            m_adjacentConnections.Add(otherNodeB.m_connectionList[l]);
                        }
                    }
                }
            }
        }
    }

    void CreateLine(Vector3 a_startPos, Vector3 a_endPos, Color a_startColor, Color a_endColor, Material a_lineMat = null)
    {
        m_linePositionsHolder[0] = a_startPos;
        m_linePositionsHolder[1] = a_endPos;
        LineRenderer lineRenderer = Instantiate<GameObject>(m_lineRendererRef, this.transform).GetComponent<LineRenderer>();
        if (a_lineMat != null)
        {
            lineRenderer.material = a_lineMat;
            //lineRenderer.material.SetTextureScale("_MainTex", new Vector2(0.1f,0.1f));
        }
        m_createdLineRenderers.Add(lineRenderer.gameObject);

        lineRenderer.startColor = a_startColor;
        lineRenderer.endColor = a_endColor;

        lineRenderer.SetPositions(m_linePositionsHolder);
    }

    void CreateLine(Vector3 a_endPosition, Color a_lineColor, Material a_lineMat = null)
    {
      CreateLine(transform.position, a_endPosition, a_lineColor, a_lineColor, a_lineMat);
    }

    internal void CreateFrontLine(Vector3 a_endPos)
    {
        CreateLine(transform.position, a_endPos, Color.green, Color.green);

        Vector3 frontDirection = a_endPos - transform.position;
        Vector3 perpA = frontDirection.RotateVector3In2D(90f);
        Vector3 perpB = frontDirection.RotateVector3In2D(-90f);
        Vector3 enemyTownPos = m_mapNodes[0].m_overrun ? m_mapNodes[0].transform.position : m_mapNodes[1].transform.position;
        Vector3 enemyTowndirection = transform.position - enemyTownPos;


        float angleA = Vector3.Angle(perpA, enemyTowndirection);
        float angleB = Vector3.Angle(perpB, enemyTowndirection);

        Vector3 offsetVector = Vector3.zero;

        if (angleA >= angleB)
        {
            offsetVector = perpA;
        }
        else
        {
            offsetVector = perpB;
        }

        offsetVector = offsetVector.normalized;
        offsetVector *= m_lineRendererRef.GetComponent<LineRenderer>().startWidth;

        CreateLine(transform.position - offsetVector, a_endPos - offsetVector, Color.red, Color.red);
    }

    void ClearUpLines()
    {
        for (int i = 0; i < m_createdLineRenderers.Count; i++)
        {
            Destroy(m_createdLineRenderers[i]);
        }
        m_createdLineRenderers.Clear();
    }

    void SetUpLines()
    {
        ClearUpLines();
        Vector3[] linePositions = new Vector3[2];

        for (int i = 0; i < m_mapNodes.Length; i++)
        {
            Color lineColor = Color.white;

            lineColor = m_mapNodes[i].m_overrun ? Color.green : Color.red;

            CreateLine(m_mapNodes[i].transform.position, lineColor, m_roadMaterial);
        }
        if (m_isaFront)
        {
            for (int i = 0; i < m_anchorPoints.Length; i++)
            {
                //CreateLine(m_anchorPoints[i].transform.position, Color.red);
            }
            for (int i = 0; i < m_adjacentConnections.Count; i++)
            {
                if (m_adjacentConnections[i].m_isaFront)
                {
                    CreateFrontLine(m_adjacentConnections[i].transform.position);
                }
            }
        }
    }

    public void Refresh()
    {
        CalculateIfIsFront();
        CalculatePosition();
        SetUpLines();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.B))
        {
            Refresh();
        }
    }
}
