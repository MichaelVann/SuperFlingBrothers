using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nucleus : Damageable
{

    public override void Awake()
    {
        base.Awake();
        m_statHandler.m_stats[(int)eStatIndices.constitution].finalValue = 500f;
        UpdateLocalStatsFromStatHandler();
        m_damageTextColor = Color.red;
    }

    public override void Start()
    {
        base.Start();
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
