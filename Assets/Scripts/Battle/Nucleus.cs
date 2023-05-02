using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nucleus : Damageable
{

    public override void Awake()
    {
        base.Awake();
        m_statHandler.m_stats[(int)eStatIndices.constitution].finalValue = 100f;
        UpdateLocalStatsFromStatHandler();
        m_damageTextColor = Color.red;
    }

    public override void Start()
    {
        base.Start();
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_statHandler.m_stats[(int)eStatIndices.constitution].finalValue *= Mathf.Pow(m_gameHandlerRef.m_battleDifficulty, 0.5f)/Mathf.Pow(10f, 0.5f);
        UpdateLocalStatsFromStatHandler();

    }

    public override void Update()
    {
        base.Update();
    }

    public override void Die()
    {
        m_battleManagerRef.StartEndingGame(eEndGameType.lose);
        base.Die();
    }
}
