using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : BaseObject
{
    protected GameHandler m_gameHandlerRef;
    protected BattleManager m_battleManagerRef;

    public Vector3 m_lastVelocity;
    public float m_lastMomentumMagnitude = 0f;
    protected float m_damagePerSpeedDivider = GameHandler.DAMAGEABLE_damagePerSpeedDivider;

    const float m_massDivider = 2f;
    public float m_originalMass;
    public Color m_originalColor;

    public GameObject m_explosionPrefab;
    public GameObject m_collisionSparkPrefab;

    public GameObject m_risingFadingTextPrefab;
    protected float m_damageTextYOffset = 0.2f;
    protected Color m_damageTextColor = Color.yellow;
    protected Color m_healTextColor = Color.green;

    static Func<int, Collision2D> CollisionFuncPTR = null;

    public ProgressBar m_healthBarRef;
    public ProgressBar m_shieldBarRef;
    public GameObject m_shadowRef;

    public GameObject m_bloodSplatterTemplate;
    public Color m_bloodColor;

    public GameObject m_puffOfSmokeTemplate;

    protected float m_lastDamageTaken = 0f;

    RigidbodyConstraints2D m_originalConstraints;

    bool m_stunned = false;
    vTimer m_stunTimer;

    public struct Shield
    {
        internal bool enabled;
        internal float delay;
        internal float delayTimer;
        internal float rechargeRate;

        internal float value;
        internal float capacity;
    }
    internal Shield m_shield;

    //Fling
    bool m_secondFlingEnabled = false;
    bool m_secondFling = true;
    float m_bumpFlingStrengthMult = GameHandler.DAMAGEABLE_bumpFlingStrengthMult;
    float m_secondFlingTimer = 0f;
    float m_secondFlingTimerMax = 0.09f;
    Vector3 m_storedFlingVector;
    float m_storedFlingStrength = 0f;

    float m_pocketFlingStrength = GameHandler.DAMAGEABLE_pocketFlingStrength;

    bool m_clearVelocityOption = false;

    internal CharacterStatHandler m_statHandler;
    internal float m_health;
    internal float m_maxHealth;

    protected bool m_rotateToAlignToVelocity = false;

    //Damage flash
    bool m_damageFlashOverrideRunning = false;
    vTimer m_damageFlashOverrideTimer;
    float m_damageFlashTimerMax = 0.1f;

    protected Material m_defaultMaterialRef;

    public float GetHealthPercentage() { return m_health / m_maxHealth; }

    public override void Awake()
    {
        base.Awake();
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_battleManagerRef = FindObjectOfType<BattleManager>();
        m_statHandler = new CharacterStatHandler();
        m_statHandler.Init(false);
        m_originalColor = m_spriteRenderer.color;
        m_rigidBody.mass = m_originalMass = GameHandler.DAMAGEABLE_defaultMass;
        m_damageFlashOverrideTimer = new vTimer(m_damageFlashTimerMax,false);
        m_defaultMaterialRef = m_spriteRenderer.material;
        UpdateLocalStatsFromStatHandler();
        m_originalConstraints = m_rigidBody.constraints;
    }

    public virtual void Start()
    {
        if (m_shieldBarRef)
        {
            m_shieldBarRef.SetHealthColoring(false);
        }
    }

    void UpdateRotationToFollowVelocity()
    {
        m_rigidBody.MoveRotation(VLib.Vector2ToEulerAngle(m_rigidBody.velocity));// VLib.Vector2ToEulerAngle(m_rigidBody.velocity));
    }

    protected void UpdateLocalStatsFromStatHandler()
    {
        m_health = m_maxHealth = m_statHandler.m_stats[(int)eCharacterStatIndices.constitution].m_finalValue;
        UpdateHealthColor();
        m_healthBarRef.SetMaxProgressValue(m_maxHealth);
    }

    void BoundsCheck()
    {

    }

    void UpdateMass()
    {
        //m_rigidBody.mass = m_originalMass * 0.33f + (GetHealthPercentage() * 0.77f);
    }

    protected void UpdateHealthColor()
    {
        float divider = 0.8f + 0.2f*GetHealthPercentage();
        m_spriteRenderer.color = new Color(m_originalColor.r * divider, m_originalColor.g * divider, m_originalColor.b * divider, m_originalColor.a);
        if (m_healthBarRef) { m_healthBarRef.SetProgressValue(m_health); }
    }

    void SecondFlingUpdate()
    {
        if (!m_secondFling)
        {
            m_secondFlingTimer += Time.deltaTime;
            if (m_secondFlingTimer >= m_secondFlingTimerMax)
            {
                m_secondFlingTimer = 0f;
                Fling(m_storedFlingVector, m_storedFlingStrength);
                m_secondFling = true;
            }
        }
    }

    public virtual void Fling(Vector3 a_flingVector, float a_flingStrength)
    {
        GameObject smoke = Instantiate<GameObject>(m_puffOfSmokeTemplate, transform.position, new Quaternion());
        smoke.transform.localScale *= a_flingStrength/ 200f;
        //a_flingStrength *= (0.67f + (GetHealthPercentage() * 0.33f));
        if (m_secondFling)
        {
            if (m_clearVelocityOption)
            {
                m_rigidBody.velocity = new Vector2();
            }
            m_secondFling = false;
            m_rigidBody.AddForce(a_flingVector * a_flingStrength * m_bumpFlingStrengthMult);
            m_storedFlingVector = a_flingVector;
            m_storedFlingStrength = a_flingStrength;
        }
        else
        {
            m_rigidBody.AddForce(a_flingVector * a_flingStrength);
        }
    }

    private void StartDamageFlashTimer()
    {
        m_damageFlashOverrideRunning = true;
    }

    protected void SpawnDamageText(float a_value)
    {
        RisingFadingText damageText = Instantiate(m_risingFadingTextPrefab, transform.position + new Vector3(0f, m_damageTextYOffset), new Quaternion(), FindObjectOfType<Canvas>().transform).GetComponent<RisingFadingText>();
        damageText.SetImageEnabled(false);
        damageText.SetGravityAffected(true);
        damageText.SetTextContent(a_value);
        damageText.SetOriginalColor(a_value >= 0f ? m_healTextColor : m_damageTextColor);
    }

    private void ChangeHealth(float a_change)
    {
        //If the game is ending, disable damage
        if (m_battleManagerRef.m_endingGame)
        {
            return;
        }

        if (a_change < 0f)
        {
            StartDamageFlashTimer();
        }

        //If the damageables health is above it's minimum health
        if (m_health > 0f)
        {
            //Damage it
            m_health += a_change;
            m_health = Mathf.Clamp(m_health, 0f, m_maxHealth);

            //Spawn collision sparks
            Instantiate(m_collisionSparkPrefab, transform.position, new Quaternion(), transform);

            //Spawn damage text
            SpawnDamageText(a_change);
        }

        //If the dmgble is below minimum health
        if(m_health <= 0f)
        {
            Die();
        }
        UpdateMass();
        UpdateHealthColor();
        if (m_healthBarRef) { m_healthBarRef.SetProgressValue(m_health); }

    }

    //Damages the damageable
    public virtual void Damage(float a_damage, Vector2 a_damagePoint)
    {
        ChangeHealth(-a_damage);
        Bleed(a_damage/m_maxHealth);
    }

    public void Heal(float a_healing)
    {
        ChangeHealth(a_healing);
    }

    public void Damage()
    {
        Damage(1f, transform.position);
    }

    private void Bleed(float a_bleedAmount)
    {
        if (m_bloodSplatterTemplate != null)
        {
            GameObject blood = Instantiate(m_bloodSplatterTemplate, transform.position, new Quaternion());
            blood.GetComponent<SpriteRenderer>().color = m_bloodColor;
            blood.transform.localScale *= a_bleedAmount * 1f;
        }
    }

    internal void Stun(float a_stunTime)
    {
        m_stunTimer = new vTimer(a_stunTime, true, true);
        SetStunStatus(true);
    }

    void SetStunStatus(bool a_stunned)
    {
        m_stunned = a_stunned;
        m_rigidBody.constraints = a_stunned? RigidbodyConstraints2D.FreezeAll : m_originalConstraints;
    }

    public virtual void Die()
    {
        Instantiate(m_explosionPrefab, transform.position, new Quaternion());
        Destroy(gameObject);
    }

    public float GetChanceToHit(Vector2 a_contactPoint)
    {
        Vector3 positionVector = new Vector3(transform.position.x, transform.position.y, 0f);
        Vector3 contactVector = new Vector3(a_contactPoint.x, a_contactPoint.y, 0f) - positionVector;
        float velocityAngle = VLib.Vector2ToEulerAngle(m_lastVelocity.normalized);
        float contactAngle = VLib.Vector2ToEulerAngle(contactVector);
        float glance = Mathf.DeltaAngle(velocityAngle, contactAngle);
        if (glance < 0f)
        {
            glance *= -1f;
        }
        glance /= 180f;
        glance = 1f - glance;
        //SpawnDamageText(glance);
        float chanceToHit = m_lastMomentumMagnitude * (glance);
        return chanceToHit;
    }

    public virtual bool OnCollisionEnter2D(Collision2D a_collision)
    {
        bool tookDamage = false;
        Damageable oppDamageable = a_collision.gameObject.GetComponent<Damageable>();
        if (oppDamageable)
        {
            Vector2 contactPoint = a_collision.GetContact(0).point;
            float contactStrength = GetChanceToHit(contactPoint);

            float oppContactStrength = oppDamageable.GetChanceToHit(contactPoint);
            if (oppContactStrength >= contactStrength)
            {
                //Damage(oppDamageable.m_statHandler.m_stats[(int)eCharacterStatIndices.strength].finalValue * oppDamageable.m_lastMomentumMagnitude / m_damagePerSpeedDivider);

                float oppStrength = oppDamageable.m_statHandler.m_stats[(int)eCharacterStatIndices.strength].m_finalValue;
                float oppSpeed = 1f / m_damagePerSpeedDivider;
                float damage = oppStrength * oppSpeed * oppContactStrength;
                Damage(oppStrength * oppSpeed * oppContactStrength, a_collision.contacts[0].point);
                tookDamage = true;
                m_lastDamageTaken = damage;
            }
        }
        return tookDamage;
    }

    private void DamageFlashOverrideUpdate()
    {
        if (m_damageFlashOverrideRunning)
        {
            m_spriteRenderer.material = m_battleManagerRef.m_whiteFlashMaterialRef;
            if (m_damageFlashOverrideTimer.Update())
            {
                m_damageFlashOverrideRunning = false;
                m_spriteRenderer.material = m_defaultMaterialRef;
                m_damageFlashOverrideTimer.Reset();
                UpdateHealthColor();
            }
        }
    }

    public override void Update()
    {
        base.Update();
        m_lastVelocity = m_rigidBody.velocity;
        m_lastMomentumMagnitude = m_rigidBody.velocity.magnitude * m_rigidBody.mass;
        SecondFlingUpdate();


        DamageFlashOverrideUpdate();
        float eulerAnglesForShadow = transform.eulerAngles.z + GameHandler.BATTLE_ShadowAngle;
        float x = Mathf.Sin(eulerAnglesForShadow * Mathf.PI / 180f) / transform.localScale.x;
        float y = Mathf.Cos(eulerAnglesForShadow * Mathf.PI / 180f) / transform.localScale.y;
        m_shadowRef.transform.localPosition = new Vector3(x, y) * BattleManager.m_shadowDistance;

        if (m_rotateToAlignToVelocity)
        {
            UpdateRotationToFollowVelocity();
        }

        m_healthBarRef.transform.localEulerAngles = -transform.localEulerAngles;

        if (m_stunTimer != null)
        {
            if (m_stunTimer.Update())
            {
                SetStunStatus(false);
            }
        }
    }

    protected void TakePocketDamage(Vector2 a_contactPoint)
    {
        Damage(m_health * 0.45f + m_maxHealth * 0.05f, a_contactPoint);
    }

    protected void PocketFling(Vector3 a_pocketPos)
    {
        m_rigidBody.AddForce((transform.position - a_pocketPos).normalized * m_pocketFlingStrength);
    }
}
