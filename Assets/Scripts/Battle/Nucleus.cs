using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Nucleus : Damageable
{
    [SerializeField] TextMeshProUGUI m_turnsLeftText;
    [SerializeField] GameObject m_shockwavePrefab;

    float GetTickDamage() {return m_maxHealth / GameHandler.BATTLE_NucleusTicks; }

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
        m_statHandler.m_stats[(int)eCharacterStatType.constitution].m_finalValue = 1000f;
        UpdateLocalStatsFromStatHandler();
        if (m_healthBarRef) { m_healthBarRef.SetProgressValue(m_health); }
        m_damageTextOriginalScale = 2f;
        m_deathExplosionScale = 7f;

        UpdateEstimatedTurnsLeftText();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void OnCollisionEnter2D(Collision2D a_collision)
    {
        Player oppPlayer = a_collision.gameObject.GetComponent<Player>();
        if (oppPlayer)
        {
            if (oppPlayer.m_lastMomentumMagnitude >= m_lastMomentumMagnitude)
            {
                Heal(oppPlayer.m_statHandler.m_stats[(int)eCharacterStatType.strength].m_finalValue * oppPlayer.m_lastMomentumMagnitude / m_damagePerSpeedDivider);
            }
        }
    }

    internal void Tick()
    {
        Damage(GetTickDamage(), Vector2.zero);
    }

    protected override void ChangeHealth(float a_damage)
    {
        base.ChangeHealth(a_damage);
        UpdateEstimatedTurnsLeftText();
        UpdateShakeAmount();
    }

    void UpdateEstimatedTurnsLeftText()
    {
        float estimatedTurnsLeft = VLib.RoundToDecimalPlaces(m_health / GetTickDamage(),1);
        m_turnsLeftText.text = estimatedTurnsLeft.ToString();
    }

    void UpdateShakeAmount()
    {
        float shakeAmount = (1f - GetHealthPercentage());
        shakeAmount = Mathf.Pow(shakeAmount, 5f);
        shakeAmount *= 0.012f;
        SetShakeAmount(shakeAmount);
        Debug.Log(shakeAmount);
    }

    public override void Die()
    {
        m_battleManagerRef.NucleusExplodes();
        Instantiate(m_shockwavePrefab, transform.position, Quaternion.identity);
        base.Die();
    }
}
