using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeInfoPanelHandler : MonoBehaviour
{
    public Text m_invadersTextRef;
    BattleNode m_inspectedBattleNodeRef;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp(BattleNode a_inspectedBattleNode)
    {
        m_inspectedBattleNodeRef = a_inspectedBattleNode;
        m_invadersTextRef.text = "" + m_inspectedBattleNodeRef.m_invaders;
    }
}
