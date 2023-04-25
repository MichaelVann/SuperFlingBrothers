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
    BodyPartInterface m_parentBodyPartInterfaceRef;

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
        m_bodySelectionHandlerRef = FindObjectOfType<BodyPartSelectionHandler>();
        m_parentBodyPartRef = m_gameHandlerRef.m_humanBody.m_bodyPartList[m_parentBodyPartID];
        //m_battleNodeRef = m_parentBodyPartRef.m_nodes[m_id];
        m_selectionRing.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //SetUpLines();
    }

    public void Select()
    {
        m_bodySelectionHandlerRef.SelectNode(m_id,m_parentBodyPartID, this);
    }

    void OnMouseUpAsButton()
    {
        Select();
    }

}
