using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NodeInfoPanelHandler : MonoBehaviour
{
    public TextMeshProUGUI m_difficultyTextRef;
    public TextMeshProUGUI m_environmentalEffectsTextRef;
    BattleNode m_inspectedBattleNodeRef;
    vTimer m_environEffectsScrambleTimer;
    Vector3 m_environEffectsOriginalPos;
    Vector3 m_environEffectsOffsetVector;
    [SerializeField] GameObject m_unknownEnvironmentalEffectsTagRef;


    bool m_scramblingEnvironEffectString = false;
    string m_environEffectsString;
    StringBuilder m_environmentalEffectsStringBuilder;
    vTimer m_environEffectsJitterTimer;
    float m_jitterStrength = 2f;
    float m_jitterRegressionExponent = 0.7f;

    bool m_randomiseAll = true;

    private void Awake()
    {
        m_environEffectsOriginalPos = m_environmentalEffectsTextRef.transform.localPosition;
        m_environmentalEffectsStringBuilder = new StringBuilder();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_environEffectsScrambleTimer != null)
        {
            if (m_environEffectsScrambleTimer.Update())
            {
                RefreshEnvironmentalEffectsText();
            }

        }
        if (m_environEffectsJitterTimer != null)
        {
            if (m_environEffectsJitterTimer.Update())
            {
                float deviation = m_jitterStrength * VLib.vRandom(1f, 2f);
                m_environEffectsOffsetVector += new Vector3(VLib.vRandom(-deviation, deviation), VLib.vRandom(-deviation, deviation), 0f);
                m_environEffectsOffsetVector *= m_jitterRegressionExponent;
                m_environmentalEffectsTextRef.transform.localPosition = m_environEffectsOriginalPos + m_environEffectsOffsetVector;
            }
        }
    }

    void SetUpEnvironmentalEffectsString()
    {
        string environmentalEffectsString = "";

        bool commaNeeded = false;

        if (m_inspectedBattleNodeRef.m_environmentalEffects.gravityWellsEnabled)
        {
            if (m_inspectedBattleNodeRef.m_environmentalEffects.megaGravityWellEnabled)
            {
                environmentalEffectsString += "Mega Whirlpool";
            }
            else
            {
                environmentalEffectsString += "Whirlpools";
            }
            commaNeeded = true;
        }
        if (m_inspectedBattleNodeRef.m_environmentalEffects.wallTrianglesEnabled)
        {
            if (commaNeeded)
            {
                environmentalEffectsString += ", ";
            }
            environmentalEffectsString += "Wall Triangles";
            commaNeeded = true;
        }

        m_environEffectsString = environmentalEffectsString;
    }

    void RefreshEnvironmentalEffectsText()
    {
        if (m_scramblingEnvironEffectString && m_environmentalEffectsStringBuilder.Length > 0)
        {
            if (m_randomiseAll)
            {
                m_environmentalEffectsTextRef.text = VLib.ScrambleAlphabeticalString(m_environEffectsString);
            }
            else
            {
                int index = VLib.vRandom(0, m_environmentalEffectsStringBuilder.Length - 1);
                m_environmentalEffectsStringBuilder[index] = VLib.ScrambleAlphabeticalCharacter(m_environEffectsString[index]);
                m_environmentalEffectsTextRef.text = m_environmentalEffectsStringBuilder.ToString();
            }

        }
        else
        {
            m_environmentalEffectsTextRef.text = m_environEffectsString;
        }
    }

    void SetUpEnvironmentalEffectsText()
    {
        SetUpEnvironmentalEffectsString();

        bool hasScouting = GameHandler.m_staticAutoRef.m_upgradeTree.HasUpgrade(UpgradeItem.UpgradeId.battleScouting);
        m_unknownEnvironmentalEffectsTagRef.SetActive(!hasScouting);

        if (!hasScouting)
        {
            int roll = VLib.vRandom(1, 2);
            if (roll == 1)
            {
                m_environEffectsScrambleTimer = new vTimer(0.22f);
                m_randomiseAll = true;
            }
            else
            {
                m_environEffectsScrambleTimer = new vTimer(0.0008f);
                m_randomiseAll = false;
            }
            m_environEffectsScrambleTimer = new vTimer(0.22f);
            m_environEffectsJitterTimer = new vTimer(0.02f);
            m_environEffectsOffsetVector = new Vector3();
            m_scramblingEnvironEffectString = true;
            m_environmentalEffectsStringBuilder = new StringBuilder(VLib.ScrambleAlphabeticalString(m_environEffectsString));
        }

        RefreshEnvironmentalEffectsText();
    }

    public void SetUp(BattleNode a_inspectedBattleNode)
    {
        m_inspectedBattleNodeRef = a_inspectedBattleNode;
        m_difficultyTextRef.text = "Difficulty: " + m_inspectedBattleNodeRef.m_difficulty;
        SetUpEnvironmentalEffectsText();
    }
}
