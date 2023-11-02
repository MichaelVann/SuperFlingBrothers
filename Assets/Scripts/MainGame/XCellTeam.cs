using UnityEditor;
using UnityEngine;

public class XCellTeam
{
    internal string m_name;
    internal int m_level;
    internal CharacterStatHandler m_statHandler;
    internal XCell m_playerXCell;
    internal int m_playerCellIteration = 1;

    public XCellTeam()
    {
        m_name = "";
        m_level = 0;
        m_statHandler = new CharacterStatHandler();
        m_statHandler.Init();
        m_statHandler.ClearPostAddedValues();
        m_playerXCell = new XCell();
    }

    void ApplyStatsToPlayer()
    {
        m_playerXCell.m_statHandler.SetParentValues(m_statHandler);
    }

    public void KillPlayer()
    {
        m_playerXCell = new XCell();
        m_playerCellIteration++;
        ApplyStatsToPlayer();
    }

    public void AttemptToIncreaseStat(eCharacterStatIndices a_index)
    {
        m_statHandler.AttemptToIncreaseStat(a_index);
        ApplyStatsToPlayer();
    }
}