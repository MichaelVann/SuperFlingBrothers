using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartInterface : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public BodyPartSelectionHandler m_bodyPartSelectionHandler;
    public int m_bodyPartID;
    public GameObject m_frontLineRef;
    public GameObject m_leftFrontLineRef;
    public GameObject m_rightFrontLineRef;
    public GameObject m_leftFrontLineColliderRef;
    public GameObject m_rightFrontLineColliderRef;
    public GameObject m_lockRef;
    float m_rightFrontLineOriginalXScale = 1f;
    float m_rightFrontLineColliderOriginalOffsetX = 0f;
    float m_leftFrontLineOriginalXScale = 1f;
    float m_leftFrontLineColliderOriginalOffsetX = 0f;
    int m_rightFrontLineSetupComplete = 0;
    int m_leftFrontLineSetupComplete = 0;
    int m_frontLineSetupCompletionsRequired = 2;
    Collider2D m_colliderRef;
    const int m_maximumFrontLineChecks = 1000;
    int m_frontLineChecks = 0;


    //Nodes
    public GameObject m_nodePrefabRef;
    public List<GameObject> m_nodeGameobjectList;
    public List<UIBattleNode> m_nodeList;
    bool m_nodeSetupComplete = false;
    int m_nodesToSpawn = 20;
    int m_maxNodeSpawnAttempts = 100;
    bool m_nodeSetupFrameComplete = false;

    public int m_minBaseDifficulty = 1;
    public int m_maxBaseDifficulty = 30;
    public int m_maxEnemyDifficulty = 5;

    const float m_deltaFrontLineScale = 0.001f;
    // Start is called before the first frame update
    void Start()
    {
        m_colliderRef = GetComponent<Collider2D>();
        m_bodyPartSelectionHandler = FindObjectOfType<BodyPartSelectionHandler>();
        m_rightFrontLineOriginalXScale = m_rightFrontLineRef.transform.localScale.x;
        m_rightFrontLineColliderOriginalOffsetX = m_rightFrontLineColliderRef.transform.localPosition.x - m_rightFrontLineRef.transform.localPosition.x;
        m_leftFrontLineOriginalXScale = m_leftFrontLineRef.transform.localScale.x;
        m_leftFrontLineColliderOriginalOffsetX = m_leftFrontLineColliderRef.transform.localPosition.x - m_leftFrontLineRef.transform.localPosition.x;

        m_nodeGameobjectList = new List<GameObject>();
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        if (!m_gameHandlerRef.m_humanBody.m_bodyPartList[m_bodyPartID].m_unlocked)
        {
            GetComponent<SpriteRenderer>().color = new Color(0.3f,0.3f,0.3f,1f);
        }
        m_lockRef.SetActive(!m_gameHandlerRef.m_humanBody.m_bodyPartList[m_bodyPartID].m_unlocked);
        //SetUpNodes();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_nodeSetupFrameComplete)
        {
            if (m_gameHandlerRef.m_humanBody.m_bodyPartList[m_bodyPartID].m_unlocked)
            {
                SetUpFrontLines();
            }
        }
        else
        {
            m_nodeSetupFrameComplete = true;
        }

    }

    void OnMouseUpAsButton()
    {
        m_bodyPartSelectionHandler.SelectPart(m_bodyPartID);
    }

    public void Setup(BodyPart a_bodyPart)
    {

    }

    void SetUpFrontLinePart(ref int a_frontLineSetupComplete, ref GameObject a_frontLineRef, ref GameObject a_frontLineColliderRef, float a_frontLineOriginalXScale, float a_frontLineColliderOriginalOffsetX)
    {
        //If the front line hasn't finished setting up yet
        if (a_frontLineSetupComplete < m_frontLineSetupCompletionsRequired)
        {

            a_frontLineRef.transform.localScale = new Vector3(a_frontLineRef.transform.localScale.x + m_deltaFrontLineScale, a_frontLineRef.transform.localScale.y, 1f);
            float colliderPosX = a_frontLineRef.transform.localPosition.x + a_frontLineColliderOriginalOffsetX * a_frontLineRef.transform.localScale.x / a_frontLineOriginalXScale;
            a_frontLineColliderRef.transform.localPosition = new Vector3(colliderPosX, a_frontLineColliderRef.transform.localPosition.y, a_frontLineColliderRef.transform.localPosition.z);
            if (Physics2D.IsTouching(a_frontLineColliderRef.GetComponent<Collider2D>(), m_colliderRef))
            {
                a_frontLineRef.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                a_frontLineRef.GetComponent<SpriteRenderer>().color = Color.green;
                a_frontLineSetupComplete++;
                if (a_frontLineSetupComplete >= m_frontLineSetupCompletionsRequired)
                {
                    m_nodeSetupComplete = true;
                    SetUpNodes();
                }
            }
        }

    }

    public void SetUpFrontLines()
    {
        SetUpFrontLinePart(ref m_rightFrontLineSetupComplete, ref m_rightFrontLineRef, ref m_rightFrontLineColliderRef, m_rightFrontLineOriginalXScale, m_rightFrontLineColliderOriginalOffsetX);
        SetUpFrontLinePart(ref m_leftFrontLineSetupComplete, ref m_leftFrontLineRef, ref m_leftFrontLineColliderRef, m_leftFrontLineOriginalXScale, m_leftFrontLineColliderOriginalOffsetX);
    }

    void SetUpNodes()
    {
        for (int i = 0; i < m_nodesToSpawn; i++)
        {
            GameObject nodeGameObject = Instantiate<GameObject>(m_nodePrefabRef, m_frontLineRef.transform);

            bool validSpawnFound = false;
            int spawnAttempts = 0;

            while (!validSpawnFound)
            {
                float xPos = UnityEngine.Random.Range(m_leftFrontLineColliderRef.transform.localPosition.x, m_rightFrontLineColliderRef.transform.localPosition.x);
                float yPos = UnityEngine.Random.Range(-0.1f, 0.1f);
                nodeGameObject.transform.localPosition = new Vector3(xPos, yPos, -1f);

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
                if (!validSpawnFound)
                {
                    continue;
                }

                if (true)
                {

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
                node.m_id = i;
                node.m_parentBodyPartID = m_bodyPartID;

                int DEBUGDifficultySystem = 0;
                if (DEBUGDifficultySystem == 0)
                {
                    node.m_difficulty = Random.Range(m_minBaseDifficulty, m_maxBaseDifficulty);
                }
                else if (DEBUGDifficultySystem == 1)
                {
                    int difficulty = ((m_maxBaseDifficulty - m_minBaseDifficulty) / 2) + m_minBaseDifficulty;
                    int difficultyDelta = 0;
                    
                    float roll = Random.Range(0f, 1f);
                    float rollCutoff = 0.1f;
                    while (roll >= rollCutoff)
                    {
                        roll = Random.Range(0f, 1f);
                        //rollCutoff += 0.1f;
                        difficultyDelta++;
                    }
                    difficultyDelta *= Random.Range(0f, 1f) >= 0.5f ? 1 : -1;
                    difficulty += difficultyDelta;
                    node.m_difficulty = 60;

                }
                else if (DEBUGDifficultySystem == 2)
                {
                    node.m_difficulty = Random.Range(m_minBaseDifficulty, m_maxBaseDifficulty);

                    float roll = Random.Range(0f, 1f);
                    float rollCutoff = 0.1f;
                    if (node.m_difficulty >= (int)((m_maxBaseDifficulty-m_minBaseDifficulty)*0.8f + m_minBaseDifficulty))
                    {
                        while (roll <= rollCutoff)
                        {
                            node.m_difficulty *= 2;
                            roll = Random.Range(0f, 1f);
                            node.m_difficultyBoostTier++;
                        }
                    }
                }
                Color nodeColor = VLib.PercentageToColor(1f - (float)(node.m_difficulty - m_minBaseDifficulty) / (float)(m_maxBaseDifficulty - m_minBaseDifficulty));
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
                m_nodeList.Add(node);
            }

        }
        m_bodyPartSelectionHandler.SetUpBodyPartNodes(m_bodyPartID);
    }
}
