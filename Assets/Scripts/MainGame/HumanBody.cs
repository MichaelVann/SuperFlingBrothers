using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBody
{
    
    public BodyPart[] m_bodyPartList;

    float m_basePartHealth = 100;

    public bool m_bodyInitialised = false;

    public void SetBodyPartLockedStatus(BodyPart.eType a_type, bool a_unlocked) { m_bodyPartList[(int)a_type].m_unlocked = a_unlocked; }

    public HumanBody()
    {
        SetUpBodyParts();
    }

    private void SetUpBodyParts()
    {
        m_bodyPartList = new BodyPart[(int)BodyPart.eType.Count];
        for (int i = 0; i < (int)BodyPart.eType.Count; i++)
        {
            SetUpDefaultBodyPart(ref m_bodyPartList[i], i);
        }
    }

    public void SetUpBodyPartNodes(List<BodyPartUI> a_UIPartList)
    {
        for (int i = 0; i < a_UIPartList.Count; i++)
        {
            m_bodyPartList[i].SetUpPartNodesFromUI(a_UIPartList[i].m_UIBattleNodeList);
        }
    }

    public void SetUpDefaultBodyPart(ref BodyPart a_part, int a_index)
    {
        ref BodyPart part = ref a_part;
        int invaders = 100;
        BodyPart.Health health = new BodyPart.Health();
        health.max = 0;
        bool unlocked = false;

        switch ((BodyPart.eType)a_index)
        {
            case BodyPart.eType.Chest:
                health.max = 1.8f;
                break;
            case BodyPart.eType.Face:
                health.max = 0.5f;
                break;
            case BodyPart.eType.Head:
                health.max = 0.5f;
                break;
            case BodyPart.eType.Neck:
                health.max = 0.5f;
                break;
            case BodyPart.eType.Pelvis:
                health.max = 0.6f;
                break;
            case BodyPart.eType.Waist:
                health.max = 1f;
                break;
            case BodyPart.eType.LeftFoot:
            case BodyPart.eType.RightFoot:
                health.max = 0.4f;
                unlocked = true;
                break;
            case BodyPart.eType.LeftForeArm:
            case BodyPart.eType.RightForeArm:
                health.max = 0.4f;
                break;
            case BodyPart.eType.LeftHand:
            case BodyPart.eType.RightHand:
                health.max = 0.4f;
                unlocked = true;
                break;
            case BodyPart.eType.LeftKnee:
            case BodyPart.eType.RightKnee:
                health.max = 0.6f;
                break;
            case BodyPart.eType.LeftShoulder:
            case BodyPart.eType.RightShoulder:
                health.max = 0.7f;
                break;
            case BodyPart.eType.LeftThigh:
            case BodyPart.eType.RightThigh:
                health.max = 0.6f;
                break;

            case BodyPart.eType.Count:
                break;
            default:
                break;
        }
        health.value = health.max *= m_basePartHealth;

        a_part = new BodyPart((BodyPart.eType)a_index, health, unlocked);
    }
}
