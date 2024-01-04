using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBattleNode : MonoBehaviour
{
    // Start is called before the first frame update

    GameHandler m_gameHandlerRef;
    public List<GameObject> m_connectionList;
    //BodyPartSelectionHandler m_bodySelectionHandlerRef;
    internal BattleNode m_battleNodeRef;
    MapNodeConnection m_owningConnection;

    public int m_invaders = 0;
    public int m_id = 0;
    public int m_parentBodyPartID;
    public int m_difficulty = 5;
    public int m_difficultyBoostTier = 0;

    public GameObject m_selectionRingRef;
    public GameObject m_lockIconRef;
    [SerializeField] GameObject m_sheenRef;
    public GameObject m_shadowRef;

    //Bobbing Effect
    float m_bobTimer = 0f;
    float m_bobScale = 0.003f;
    Vector3 m_startingPosition;

    Vector3 m_shadowStartingPostion;
    Vector3 m_shadowStartingDirection;

    internal bool m_enabled;

    public void SetSelectionRingActive(bool a_active) { m_selectionRingRef.SetActive(a_active); }


    void Start()
    {
        m_bobTimer += VLib.vRandom(0, Mathf.PI);
        m_selectionRingRef.SetActive(false);
    }

    public void SetUp(MapNodeConnection a_connection, BattleNode a_battleNodeRef)
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_owningConnection = a_connection;
        m_battleNodeRef = a_battleNodeRef;
        m_difficulty = m_battleNodeRef.m_difficulty;
        m_difficultyBoostTier = m_battleNodeRef.m_difficultyBoostTier;
        m_enabled = m_battleNodeRef.m_available;

        if (m_enabled)
        {
            m_gameHandlerRef.m_humanBody.m_availableBattles++;
        }
        m_startingPosition = transform.position;
        m_shadowStartingPostion =  m_shadowRef.transform.position;
        m_shadowStartingDirection = m_shadowRef.transform.localPosition.normalized;
        m_lockIconRef.SetActive(!m_enabled);
        m_sheenRef.SetActive(m_enabled);
        SetNodeDifficultyColor();
    }

    // Update is called once per frame
    void Update()
    {
        m_bobTimer += Time.deltaTime;
        transform.position = m_startingPosition + new Vector3(0f,m_bobScale * Mathf.Sin(m_bobTimer*Mathf.PI));

        m_shadowRef.transform.position = m_shadowStartingPostion + m_shadowStartingDirection * (m_bobScale * Mathf.Sin(m_bobTimer*Mathf.PI));
    }

    public void Select()
    {
        m_owningConnection.SelectBattleNode(this);
    }

    internal void SetNodeDifficultyColor()
    {
        float difficultyPercentage = m_battleNodeRef.GetDifficultyPercentOfMaximum();
        Color nodeColor = VLib.RatioToColorRarity(difficultyPercentage);

        if (!m_enabled)
        {
            nodeColor *= 0.5f;
        }
        GetComponent<SpriteRenderer>().color = nodeColor;
    }


    void OnMouseUpAsButton()
    {
        //Select();
    }

    public void NodePressed()
    {
        if (m_enabled)
        {
            Select();
        }
    }

}
