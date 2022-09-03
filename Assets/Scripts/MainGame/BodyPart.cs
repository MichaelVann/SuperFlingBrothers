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
    public float m_invasionOwnershipPercentage = 0f;
    public int m_difficulty;
    public BattleNode[] m_connectedNodes;

    public BattleNode(int a_id, eState a_state, int a_invaders, int a_difficulty)
    {
        m_id = a_id;
        m_state = a_state;
        m_invaders = a_invaders;
        m_difficulty = a_difficulty;
        m_connectedNodes = null;
    }
}

public class BodyPart
{
    public BattleNode[] m_nodes;

    public static string GetPartName(BodyPart.eType a_type) { return Enum.GetName(typeof(BodyPart.eType), a_type); }

    public string m_name;
    public bool m_unlocked;
    internal int m_maxEnemyDifficulty;

    public struct Health
    {
        public float value;
        public float max;
    }
    public Health m_health;
    public int m_invaderStrength;

    public enum eType
    {
        Chest,
        Neck,
        Face,
        Head,
        Waist,
        Pelvis,
        LeftShoulder,
        LeftForeArm,
        LeftHand,
        RightShoulder,
        RightForeArm,
        RightHand,
        LeftThigh,
        LeftKnee,
        LeftFoot,
        RightThigh,
        RightKnee,
        RightFoot,
        Count
    }
    public eType m_type;

    public void ChangeInvaderStrength(int a_value) { m_invaderStrength += a_value; }

    public BodyPart(BodyPart.eType a_type, BodyPart.Health a_health, int a_invaderStrength, bool a_unlocked)
    {
        m_name = GetPartName(a_type);
        m_type = a_type;
        m_health = a_health;
        m_invaderStrength = a_invaderStrength;
        m_unlocked = a_unlocked;
        m_nodes = null;
    }

    public void SetUpFromUI(List<UIBattleNode> a_nodeGameObjects, int a_maxEnemyDifficulty)
    {
        m_maxEnemyDifficulty = a_maxEnemyDifficulty;
        m_nodes = new BattleNode[a_nodeGameObjects.Count];
        for (int i = 0; i < a_nodeGameObjects.Count; i++)
        {
            UIBattleNode partNode = a_nodeGameObjects[i];
            m_nodes[i] = new BattleNode(i, BattleNode.eState.locked, partNode.m_invaders, partNode.m_difficulty);
            m_nodes[i].m_connectedNodes = new BattleNode[partNode.m_connectionList.Count];
        }


        //for (int i = 0; i < a_nodeGameObjects.Count; i++)
        //{
        //    UIBattleNode partNode = a_nodeGameObjects[i];
        //    for (int j = 0; j < partNode.m_connectionList.Count; j++)
        //    {
        //        m_nodes[i].m_connectedNodes[j] = a_nodeGameObjects[i].m_connectionList[j].GetComponent<UIBattleNode>().m_battleNodeRef;
        //    }
        //}
    }
}
