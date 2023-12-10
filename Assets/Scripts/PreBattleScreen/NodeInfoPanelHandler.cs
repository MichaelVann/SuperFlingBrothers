using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NodeInfoPanelHandler : MonoBehaviour
{
    public TextMeshProUGUI m_difficultyTextRef;
    public TextMeshProUGUI m_environmentalEffectsTextRef;
    BattleNode m_inspectedBattleNodeRef;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetUpEnvironmentalEffectsText(BattleNode a_inspectedBattleNode)
    {
        string environmentalEffectsString = "";

        bool commaNeeded = false;

        if (a_inspectedBattleNode.m_environmentalEffects.gravityWellsEnabled)
        {
            if (a_inspectedBattleNode.m_environmentalEffects.megaGravityWellEnabled)
            {
                environmentalEffectsString += "Mega Whirlpool";
            }
            else
            {
                environmentalEffectsString += "Whirlpools";
            }
            commaNeeded = true;
        }
        if (a_inspectedBattleNode.m_environmentalEffects.wallTrianglesEnabled)
        {
            if (commaNeeded)
            {
                environmentalEffectsString += ", ";
            }
            environmentalEffectsString += "Wall Triangles";
            commaNeeded = true;
        }

        m_environmentalEffectsTextRef.text = environmentalEffectsString;
    }

    public void SetUp(BattleNode a_inspectedBattleNode)
    {
        m_inspectedBattleNodeRef = a_inspectedBattleNode;
        m_difficultyTextRef.text = "Difficulty: " + m_inspectedBattleNodeRef.m_difficulty;
        SetUpEnvironmentalEffectsText(a_inspectedBattleNode);
    }
}
