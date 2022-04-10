using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public class BattleNode
{
    public int m_id;
    public enum eState
    {
        locked, unlocked, beaten
    }
    public eState m_state;
    public int m_invaders;
    public BattleNode[] m_connectedNodes;

    public BattleNode(int a_id, eState a_state, int a_invaders)
    {
        m_id = a_id;
        m_state = a_state;
        m_invaders = a_invaders;
        m_connectedNodes = null;
    }
}

public class BodyPart
{
    public BattleNode[] m_nodes;

    public static string GetPartName(BodyPart.eType a_type) { return Enum.GetName(typeof(BodyPart.eType), a_type); }

    public string m_name;
    public bool m_unlocked;

    public struct Health
    {
        public float value;
        public float max;
    }
    public Health m_health;

    public enum eType
    {
        Chest,
        Face,
        Head,
        LeftFoot,
        LeftForeArm,
        LeftHand,
        LeftKnee,
        LeftShoulder,
        LeftThigh,
        Neck,
        Pelvis,
        RightFoot,
        RightForeArm,
        RightHand,
        RightKnee,
        RightShoulder,
        RightThigh,
        Waist,
        Count
    }
    public eType m_type;

    public BodyPart(BodyPart.eType a_type, BodyPart.Health a_health, bool a_unlocked)
    {
        m_name = GetPartName(a_type);
        m_health = a_health;
        m_type = a_type;
        m_unlocked = a_unlocked;
        m_nodes = null;
    }

    public void SetUpPartNodesFromUI(List<UIBattleNode> a_nodeGameObjects)
    {
        m_nodes = new BattleNode[a_nodeGameObjects.Count];
        for (int i = 0; i < a_nodeGameObjects.Count; i++)
        {
            UIBattleNode partNode = a_nodeGameObjects[i];
            m_nodes[i] = new BattleNode(i, BattleNode.eState.locked, partNode.m_invaders);
            m_nodes[i].m_connectedNodes = new BattleNode[partNode.m_connectionList.Count];
        }


        for (int i = 0; i < a_nodeGameObjects.Count; i++)
        {
            UIBattleNode partNode = a_nodeGameObjects[i];
            for (int j = 0; j < partNode.m_connectionList.Count; j++)
            {
                m_nodes[i].m_connectedNodes[j] = a_nodeGameObjects[i].m_connectionList[j].GetComponent<UIBattleNode>().m_battleNodeRef;
            }
        }
    }
}
