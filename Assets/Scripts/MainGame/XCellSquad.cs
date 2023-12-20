using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class XCellSquad
{
    [SerializeField]
    internal string m_name;
    [SerializeField]
    internal CharacterStatHandler m_statHandler;
    [SerializeField]
    internal XCell m_playerXCell;
    [SerializeField]
    internal int m_playerCellIteration = 1;

    [SerializeField]
    const int m_prestigeNameLevel = 10;
    internal bool m_prestigeNamed = false;


    static string[] m_teamNamePrefixes =
    {
        "Dendritic",
        "Antigen",
        "Innate",
        "Bright",
        "Valiant",
        "Indomitable",
        "Unyielding",
        "Dauntless",
        "Selfless",
        "Steadfast",
        "Devoted",
        "Stalwart",
        "Determined",
        "Intrepid",
        "Enduring",
        "Lionheart"
    };

    static string[] m_teamNameSuffixes =
{
        "Heroes",
        "Phalanx",
        "Legion",
        "Force",
        "Vanguard",
        "Unit",
        "Killers",
        "Guardians",
        "Brotherhood",
        "Fellowship",
        "Companions",
        "Cohort",
        "Brigade",
        "Team"
    };

    public XCellSquad()
    {

    }

    public void Init()
    {
        m_name = "Squad " + VLib.vRandom(0,100);
        m_statHandler = new CharacterStatHandler();
        m_statHandler.Init(false);
        m_statHandler.ClearPostAddedValues();
        m_playerXCell = new XCell();
        m_playerXCell.Init();
        ApplyStatsToPlayer();
    }

    void ApplyStatsToPlayer()
    {
        m_playerXCell.m_statHandler.SetParentValues(m_statHandler);
    }

    public void KillPlayer()
    {
        m_playerXCell = new XCell();
        m_playerXCell.Init();
        m_playerCellIteration++;
        ApplyStatsToPlayer();
    }

    public void AttemptToIncreaseStat(eCharacterStatType a_index)
    {
        m_statHandler.AttemptToIncreaseStat(a_index);
        ApplyStatsToPlayer();
    }

    void GenerateTeamName()
    {
        m_name = "The ";
        m_name += m_teamNamePrefixes[VLib.vRandom(0,m_teamNamePrefixes.Length-1)];
        m_name += " " + m_teamNameSuffixes[VLib.vRandom(0, m_teamNameSuffixes.Length - 1)];
        for (int i = 0; i < GameHandler.m_staticAutoRef.m_highscoreList.Count; i++)
        {
            if (m_name == GameHandler.m_staticAutoRef.m_highscoreList[i].name)
            {
                GenerateTeamName();
                return;
            }
        }
    }

    internal void Refresh()
    {
        if (!m_prestigeNamed && m_statHandler.m_RPGLevel.m_level > m_prestigeNameLevel)
        {
            GenerateTeamName();
            m_prestigeNamed = true;
            GameHandler.m_staticAutoRef.m_squadRenameNotificationPending = true;
        }
    }
}