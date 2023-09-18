using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Damageable
{
    Camera m_cameraRef;
    public GameObject m_velocityIndicatorRef;

    bool m_flinging = false;
    Vector3 m_originalFlingPos;
    const float m_maxFlingLength = 1f;

    LineRenderer m_flingLine;

    bool m_hitSlowdownActive = false;
    float m_enemyHitTimer;
    float m_enemyHitTimerMax = 0.3f;
    bool invertedTime = false;

    float m_hitTimeSlowdownRate = 0.05f;

    float m_upperLowerFlingPositionBounds = 3.0f;

    public SpriteRenderer m_invalidFlingCross;

    public GameObject m_projectileTemplate;

    //Coins
    float m_cumulativeCoinValue = 0f;
    float m_coinPickupTimeout = 0;
    float m_coinPickupTimeoutMax = 1.3f;
    RisingFadingText m_coinValueText;

    public SpriteRenderer m_shieldSpriteRenderer;
    //GameHandler.Shield m_shieldRef;
    float m_maxShieldOpacity = 0.64f;
    float m_projectileDamageMult = 3f;

    public override void Awake()
    {
        base.Awake();
        m_flingLine = GetComponent<LineRenderer>();
        m_flingLine.startColor = Color.red;
        m_flingLine.endColor = Color.white;
        m_flingLine.startWidth = 0.05f;
        m_flingLine.endWidth = 0.02f;
        m_cameraRef = FindObjectOfType<Camera>();
        //m_rotateToAlignToVelocity = true;
        //m_rigidBody.freezeRotation = false;

    }

    public override void Start()
    {
        base.Start();
        m_healthBarRef.SetMaxProgressValue(m_statHandler.m_stats[(int)eCharacterStatIndices.constitution].finalValue);
        m_battleManagerRef = FindObjectOfType<BattleManager>();
        m_statHandler = m_gameHandlerRef.m_playerStatHandler;
        UpdateLocalStatsFromStatHandler();
        m_damageTextColor = Color.red;
        SetupEquipmentShield();
        //Fling(new Vector3(0f, -600f, 0f), 1f);
        m_velocityIndicatorRef.SetActive(m_gameHandlerRef.m_upgrades[(int)GameHandler.UpgradeId.playerVector].m_owned);
        m_battleManagerRef.InitialiseUpgrades();
    }

    void SetupEquipmentShield()
    {
        List<EquipmentAbility> shieldAbilities = FindEquipmentAbilities(EquipmentAbility.eAbilityType.Shield);

        int cumulativeLevel = 0;
        
        for (int i = 0; i < shieldAbilities.Count; i++)
        {
            cumulativeLevel += shieldAbilities[i].m_level;
        }
        SetUpShield(cumulativeLevel);
        bool shieldOwned = cumulativeLevel > 0;
        if (shieldOwned)
        {
            m_shield.enabled = true;
            m_shield.delayTimer = 0f;
            m_shield.value = m_shield.capacity;
        }
        m_shieldSpriteRenderer.gameObject.SetActive(shieldOwned);
        m_shieldBarRef.gameObject.SetActive(shieldOwned);
        m_shieldBarRef.SetMaxProgressValue(m_shield.capacity);
    }

    EquipmentAbility FindActiveEquipmentAbility(EquipmentAbility.eAbilityType a_abilityType)
    {
        EquipmentAbility ability = null;
        List<EquipmentAbility> abilityList = FindEquipmentAbilities(a_abilityType);
        for (int i = 0; i < abilityList.Count; i++)
        {
            if (abilityList[i].m_activated && abilityList[i].m_abilityType == a_abilityType)
            {
                ability = abilityList[i];
            }
        }
        return ability;
    }

    List<EquipmentAbility> FindEquipmentAbilities(EquipmentAbility.eAbilityType a_abilityType)
    {
        List<EquipmentAbility> abilities = new List<EquipmentAbility>();
        for (int i = 0; i < m_battleManagerRef.m_activeAbilities.Length; i++)
        {
            if (m_battleManagerRef.m_activeAbilities[i] != null)
            {
                EquipmentAbility abil = m_battleManagerRef.m_activeAbilities[i];
                if (abil.m_abilityType == a_abilityType)
                {
                    abilities.Add(abil);
                }
            }
        }
        return abilities;
    }

    public override void Fling(Vector3 a_flingVector, float a_flingStrength)
    {
        base.Fling(a_flingVector, a_flingStrength);
        m_flinging = false;
        m_battleManagerRef.SetFrozen(false);

        //Handle Projectile Shooting
        EquipmentAbility abil = FindActiveEquipmentAbility(EquipmentAbility.eAbilityType.Projectile);
        if (abil != null)
        {
            abil.m_ammo--;
            abil.m_activated = false;
            ShootProjectile(a_flingVector, abil);
        }

        //for (int i = 0; i < m_battleManagerRef.m_activeAbilities.Length; i++)
        //{
        //    if (m_battleManagerRef.m_activeAbilities[i] != null)
        //    {
        //        EquipmentAbility abil = m_battleManagerRef.m_activeAbilities[i];
        //        if (abil.m_abilityType == EquipmentAbility.eAbilityType.Projectile && abil.m_activated)
        //        {
        //            abil.m_ammo--;
        //            abil.m_activated = false;
        //            ShootProjectile(a_flingVector, abil);
        //            break;
        //        }
        //    }
        //}
        m_battleManagerRef.RefreshAbilityButtons();
    }

    void HandleFlinging()
    {
        m_invalidFlingCross.enabled = false;
        if (!m_flinging)
        {
            m_flingLine.enabled = false;

            if (m_battleManagerRef.m_timeFrozen && Input.GetMouseButton(0))
            {
                m_originalFlingPos = m_cameraRef.ScreenToWorldPoint(Input.mousePosition);

                m_flinging = true;
            }
        }
        else
        {
            if (Input.touchCount >= 2)
            {
                m_flinging = false;
                return;
            }
            m_flingLine.enabled = true;
            Vector3 worldMousePoint = m_cameraRef.ScreenToWorldPoint(Input.mousePosition);

            if (worldMousePoint.y >= m_upperLowerFlingPositionBounds || worldMousePoint.y <= -m_upperLowerFlingPositionBounds)
            {
                m_invalidFlingCross.enabled = true;
                m_invalidFlingCross.gameObject.transform.position = new Vector3(worldMousePoint.x, worldMousePoint.y);
            }
            else
            {
                m_invalidFlingCross.enabled = false;
            }

            Vector3 deltaMousePos = m_originalFlingPos - worldMousePoint;
            if (deltaMousePos.magnitude > m_maxFlingLength)
            {
                deltaMousePos = deltaMousePos.normalized * m_maxFlingLength;
            }

            Vector3[] linePositions = new Vector3[2];

            linePositions[0] = transform.position;
            linePositions[1] = transform.position - deltaMousePos;

            m_flingLine.SetPositions(linePositions);
            
            //If the release point is outside the map, cancel the shot
            if (!Input.GetMouseButton(0))
            {
                if (worldMousePoint.y < m_upperLowerFlingPositionBounds && worldMousePoint.y > -m_upperLowerFlingPositionBounds)
                {
                    Fling(deltaMousePos, m_statHandler.m_stats[(int)eCharacterStatIndices.dexterity].finalValue);
                }
                else
                {
                    m_flinging = false;
                }
            }
        }
    }

    public override void Die()
    {
        if (!m_battleManagerRef.m_endingGame)
        {
            base.Die();
            m_battleManagerRef.StartEndingGame(eEndGameType.lose);
        }
    }

    public override bool OnCollisionEnter2D(Collision2D a_collision)
    {
        Enemy enemy = a_collision.gameObject.GetComponent<Enemy>();
        EscapeZone escapeZone = a_collision.gameObject.GetComponent<EscapeZone>();
        bool runningBaseCollision = false;
        bool tookDamage = false;

        if (enemy)//If collided with an enemy
        {
            if (m_health <= m_statHandler.m_stats[(int)eCharacterStatIndices.constitution].finalValue / 3f)//
            {
                if (!m_battleManagerRef.m_endingGame)
                {
                    m_hitSlowdownActive = true;
                    m_battleManagerRef.SetTimeScale(m_hitTimeSlowdownRate);
                }
            }
            m_battleManagerRef.UseExtraTurn();
        }
        else if (escapeZone)
        {
            if (!m_battleManagerRef.m_endingGame)
            {
                m_battleManagerRef.Escape();
                m_rigidBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
                transform.position = escapeZone.transform.position;
            }
        }

        switch (m_gameHandlerRef.m_currentGameMode)
        {
            case GameHandler.eGameMode.TurnLimit:
                break;
            case GameHandler.eGameMode.Health:
                if (a_collision.gameObject.GetComponent<Pocket>())
                {
                    TakePocketDamage();
                    PocketFling(a_collision.gameObject.transform.position);
                }
                else if (a_collision.gameObject.GetComponent<Enemy>() != null)
                {
                    if (!a_collision.gameObject.GetComponent<Enemy>().m_playerVulnerable)
                    {
                        runningBaseCollision = true;
                    }
                }
                else
                {
                    runningBaseCollision = true;
                }
                break;
            case GameHandler.eGameMode.Pockets:
                if (a_collision.gameObject.GetComponent<Pocket>())
                {
                    if (m_health <= 0f)
                    {
                        Die();
                    }
                    Damage();
                }
                break;
            default:
                break;
        }

        if (runningBaseCollision)
        {
            tookDamage = base.OnCollisionEnter2D(a_collision);
        }
        return tookDamage;
    }

    internal void ShootProjectile(Vector3 a_shootVector, EquipmentAbility a_ability)
    {
        GameObject projectile = Instantiate<GameObject>(m_projectileTemplate,transform.position, VLib.Vector2DirectionToQuaternion(a_shootVector));
        Projectile projectileComp = projectile.GetComponent<Projectile>();

        projectileComp.Initialise(a_ability, a_shootVector, m_rigidBody.velocity, m_statHandler.GetStatFinalValue((int)eCharacterStatIndices.strength) * m_projectileDamageMult);
    }

    public void OnTriggerEnter2D(Collider2D a_collider)
    {
        Coin coin = a_collider.gameObject.GetComponent<Coin>();
        EquipmentDrop equipmentDrop = a_collider.gameObject.GetComponent<EquipmentDrop>();
        if (coin != null && !coin.m_movingToTargetPos)
        {
            m_battleManagerRef.ChangeScore(m_battleManagerRef.m_coinValue);

            if (m_coinValueText == null)
            {
                m_coinValueText = Instantiate(m_risingFadingTextPrefab, transform.position + new Vector3(0f, m_damageTextYOffset), new Quaternion(), FindObjectOfType<Canvas>().transform).GetComponent<RisingFadingText>();
                m_cumulativeCoinValue = 0f;
                m_coinValueText.SetImageEnabled(true);
                m_coinValueText.SetOriginalColor(Color.magenta);
                m_coinValueText.SetOriginalScale(1.2f);
                m_coinValueText.SetLifeTimerMax(1.35f);
            }
            else
            {
                m_coinValueText.SetLifeTimer(0f);
                m_coinValueText.SetOriginalPosition(transform.position + new Vector3(0f, m_damageTextYOffset));
            }
            m_cumulativeCoinValue += m_battleManagerRef.m_coinValue;

            m_coinValueText.SetTextContent("+" + m_cumulativeCoinValue);
            Destroy(a_collider.gameObject);
        }
        else if (equipmentDrop != null && !equipmentDrop.m_movingToTargetPos)
        {
            m_battleManagerRef.PickUpEquipment(a_collider.gameObject.GetComponent<EquipmentDrop>().m_equipment);
            RisingFadingText equipmentRFT = Instantiate(m_risingFadingTextPrefab, transform.position + new Vector3(m_damageTextYOffset/2f, m_damageTextYOffset*1.5f), new Quaternion(), FindObjectOfType<Canvas>().transform).GetComponent<RisingFadingText>();
            equipmentRFT.SetImageEnabled(true);
            equipmentRFT.SetOriginalColor(Color.blue);
            equipmentRFT.SetOriginalScale(1.2f);
            equipmentRFT.SetLifeTimerMax(1.35f);
            equipmentRFT.SetTextContent("+1 Eq");
        }
    }

    void SpawnBlockText(float a_value)
    {
        RisingFadingText damageText = Instantiate(m_risingFadingTextPrefab, transform.position + new Vector3(m_damageTextYOffset, 0), new Quaternion(), FindObjectOfType<Canvas>().transform).GetComponent<RisingFadingText>();
        damageText.SetImageEnabled(false);
        damageText.SetGravityAffected(true);
        damageText.SetTextContent("Block " + a_value);
        damageText.gameObject.GetComponent<Text>().fontSize = 14;
        damageText.SetOriginalColor(Color.gray);
    }

    public override void Damage(float a_damage)
    {
        if (m_battleManagerRef.m_endingGame)
        {
            return;
        }
        float damage = a_damage;
        if (m_shield.enabled)
        {
            if (m_shield.value >= damage)
            {
                m_shield.value -= damage;
                damage = 0f;
            }
            else
            {
                damage -= m_shield.value;
                m_shield.value = 0f;
            }
            m_shield.delayTimer = 0f;
        }
        float armourProtectionAmount = m_statHandler.m_stats[(int)eCharacterStatIndices.protection].finalValue * 0.1f;
        float blockedAmount = armourProtectionAmount > damage ? damage : armourProtectionAmount;
        if (blockedAmount > 0f)
        {
            SpawnBlockText(blockedAmount);
        }
        damage -= blockedAmount;
        damage = Mathf.Clamp(damage, 0f, float.MaxValue);
        base.Damage(damage);
        m_battleManagerRef.m_healthBarRef.SetBarValue(m_health);
    }

    void UpdateShieldOpacity()
    {
        Color shieldColor = m_shieldSpriteRenderer.color;
        shieldColor.a = m_maxShieldOpacity * m_shield.value/ m_shield.capacity;
        m_shieldSpriteRenderer.color = shieldColor;
        m_shieldBarRef.SetProgressValue(m_shield.value);
    }

    void ShieldUpdate()
    {
        if (m_shield.enabled)
        {
            if (m_shield.delayTimer <= m_shield.delay)
            {
                m_shield.delayTimer += Time.deltaTime;
            }
            else if (m_shield.capacity >= m_shield.value)
            {
                m_shield.value = Mathf.Clamp(m_shield.value + m_shield.rechargeRate * Time.deltaTime, 0f, m_shield.capacity);
            }
            UpdateShieldOpacity();
            m_battleManagerRef.m_shieldBarRef.SetBarValue(m_shield.value);
        }
    }

    public override void Update()
    {
        base.Update();
        HandleFlinging();
        if (m_hitSlowdownActive)
        {
            m_enemyHitTimer += Time.deltaTime/m_hitTimeSlowdownRate;
            if (m_enemyHitTimer >= m_enemyHitTimerMax)
            {
                m_hitSlowdownActive = false;
                m_enemyHitTimer = 0f;
                m_battleManagerRef.SetTimeScale(1f);
            }
        }
        ShieldUpdate();

        if (Input.GetKey(KeyCode.M))
        {
             Damage(100f);
        }
    }
}