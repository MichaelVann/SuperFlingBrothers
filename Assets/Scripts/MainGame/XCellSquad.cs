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

    public XCellSquad()
    {

    }

    public void Init()
    {
        m_name = "Team " + VLib.vRandom(0,100);
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

    public void AttemptToIncreaseStat(eCharacterStatIndices a_index)
    {
        m_statHandler.AttemptToIncreaseStat(a_index);
        ApplyStatsToPlayer();
    }
}