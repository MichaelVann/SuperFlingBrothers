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

    public GameObject m_selectionRing;

    public void SetSelectionRingActive(bool a_active) { m_selectionRing.SetActive(a_active); }


    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_selectionRing.SetActive(false);
    }

    public void SetUp(MapNodeConnection a_connection, BattleNode a_battleNodeRef)
    {
        m_owningConnection = a_connection;
        m_battleNodeRef = a_battleNodeRef;
        m_difficulty = m_battleNodeRef.m_difficulty;
        m_difficultyBoostTier = m_battleNodeRef.m_difficultyBoostTier;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Select()
    {
        m_owningConnection.SelectBattleNode(this);
    }

    void OnMouseUpAsButton()
    {
        Select();
    }

}
