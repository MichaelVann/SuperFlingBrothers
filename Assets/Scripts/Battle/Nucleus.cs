using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Nucleus : Damageable
{
    public override void Awake()
    {
        base.Awake();
        //m_statHandler.m_stats[(int)eCharacterStatIndices.constitution].finalValue = 80f;
        //UpdateLocalStatsFromStatHandler();
        m_damageTextColor = Color.red;
    }

    public override void Start()
    {
        base.Start();
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_statHandler.m_stats[(int)eCharacterStatIndices.constitution].m_finalValue = 1000f;
        UpdateLocalStatsFromStatHandler();
        if (m_healthBarRef) { m_healthBarRef.SetProgressValue(m_health); }
    }

    public override void Update()
    {
        base.Update();
    }

    public override bool OnCollisionEnter2D(Collision2D a_collision)
    {
        bool tookDamage = false;
        Player oppPlayer = a_collision.gameObject.GetComponent<Player>();
        if (oppPlayer)
        {
            if (oppPlayer.m_lastMomentumMagnitude >= m_lastMomentumMagnitude)
            {
                Heal(oppPlayer.m_statHandler.m_stats[(int)eCharacterStatIndices.strength].m_finalValue * oppPlayer.m_lastMomentumMagnitude / m_damagePerSpeedDivider);
            }
        }
        else
        {
            tookDamage = base.OnCollisionEnter2D(a_collision);
        }
        return tookDamage;
    }

    public override void Die()
    {
        m_battleManagerRef.StartEndingGame(eEndGameType.lose);
        base.Die();
    }
}
