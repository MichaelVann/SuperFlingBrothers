using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBattleNode : MonoBehaviour
{
    // Start is called before the first frame update

    GameHandler m_gameHandlerRef;
    public List<GameObject> m_connectionList;
    BodyPartSelectionHandler m_bodySelectionHandlerRef;
    internal BattleNode m_battleNodeRef;
    BodyPart m_parentBodyPartRef;

    public int m_invaders = 0;
    public int m_id = 0;
    public int m_parentBodyPartID;

    GameObject[] m_uiLines;

    public GameObject m_uiLineRef;
    public GameObject m_lineContainerRef;
    float m_lineWidth = 1.7f;

    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_bodySelectionHandlerRef = FindObjectOfType<BodyPartSelectionHandler>();
        m_parentBodyPartRef = m_gameHandlerRef.m_humanBody.m_bodyPartList[m_parentBodyPartID];
        m_battleNodeRef = m_parentBodyPartRef.m_nodes[m_id];
        m_uiLines = new GameObject[m_connectionList.Count];

        for (int i = 0; i < m_connectionList.Count; i++)
        {
            m_uiLines[i] = Instantiate<GameObject>(m_uiLineRef, m_lineContainerRef.transform);
        }
        SetUpLines();

    }


    void SetUpLines()
    {
        for (int i = 0; i < m_connectionList.Count; i++)
        {
            float angle = Vector3.SignedAngle(new Vector3(0f, 1f, 0f), new Vector3(transform.localPosition.x, transform.localPosition.y, 0f) - new Vector3(m_connectionList[i].transform.localPosition.x, m_connectionList[i].transform.localPosition.y, 0f), Vector3.forward);
            float m_deltaMagnitude = (transform.localPosition - m_connectionList[i].transform.localPosition).magnitude;
            m_uiLines[i].transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
            m_uiLines[i].transform.position = transform.position;
            m_uiLines[i].GetComponent<RectTransform>().sizeDelta = new Vector2(m_lineWidth, m_deltaMagnitude);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //SetUpLines();
    }

    public void Select()
    {
        m_bodySelectionHandlerRef.SelectNode(m_id);
    }
}
