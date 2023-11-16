using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Damageable
{
    XCell m_playerCellRef;

    Camera m_cameraRef;
    public GameObject m_velocityIndicatorRef;

    bool m_flinging = false;
    Vector3 m_originalFlingPos;
    const float m_maxFlingLength = 1f;
    const float m_minFlingLength = 0.15f;
    const int m_flingDexterityXP = 7;
    const int m_abilityUsageDexterityXP = 10;

    LineRenderer m_flingLine;

    bool m_hitSlowdownActive = false;
    float m_enemyHitTimer;
    float m_enemyHitTimerMax = 0.14f;
    bool invertedTime = false;

    float m_hitTimeSlowdownRate = 0.05f;


    public SpriteRenderer m_invalidFlingCross;

    public GameObject m_projectileTemplate;

    //Coins
    float m_cumulativeCoinValue = 0f;
    float m_coinPickupTimeout = 0;
    float m_coinPickupTimeoutMax = 1.3f;
    RisingFadingText m_coinValueText;

    //Shield
    public SpriteRenderer m_shieldSpriteRenderer;
    float m_maxShieldOpacity = 0.64f;
    float m_projectileDamageMult = 50f;
    bool m_firstTimeShieldSetup = true;

    public GameObject m_armorSegmentPrefab;
    GameObject[] m_armorSegments;
    float m_armorSegmentOffset = 0.13f;



    public override void Awake()
    {
        base.Awake();
        m_flingLine = GetComponent<LineRenderer>();
        m_flingLine.startColor = Color.red;
        m_flingLine.endColor = Color.white;
        m_flingLine.startWidth = 0.05f;
        m_flingLine.endWidth = 0.02f;
        m_cameraRef = FindObjectOfType<Camera>();
        m_playerCellRef = m_gameHandlerRef.m_xCellTeam.m_playerXCell;
        //m_rotateToAlignToVelocity = true;
        //m_rigidBody.freezeRotation = false;
    }

    public override void Start()
    {
        base.Start();
        m_originalColor = m_gameHandlerRef.m_xCellTeam.m_playerXCell.m_colorShade;
        m_statHandler = m_gameHandlerRef.m_xCellTeam.m_playerXCell.m_statHandler;
        m_healthBarRef.SetMaxProgressValue(m_statHandler.m_stats[(int)eCharacterStatIndices.constitution].m_finalValue);
        m_battleManagerRef = FindObjectOfType<BattleManager>();
        UpdateLocalStatsFromStatHandler();
        m_damageTextColor = Color.red;
        SetupEquipmentShield();
        m_velocityIndicatorRef.SetActive(m_gameHandlerRef.m_upgrades[(int)GameHandler.UpgradeId.playerVector].m_owned);
        m_battleManagerRef.InitialiseUpgrades();
        SetUpArmorSegments();
    }


    void SetUpShield(List<EquipmentAbility> a_shieldAbilityList)
    {
        m_shield.delay = 0f;
        m_shield.capacity = 0f;
        m_shield.rechargeRate = 0f;
        for (int i = 0; i < a_shieldAbilityList.Count; i++)
        {
            if (!a_shieldAbilityList[i].m_parentEquipment.IsBroken())
            {
                m_shield.delay += 1f / a_shieldAbilityList[i].m_capacitor.rechargeDelay;
                m_shield.capacity += a_shieldAbilityList[i].m_capacitor.capacity;
                m_shield.rechargeRate += a_shieldAbilityList[i].m_capacitor.rechargeRate;
            }
        }
        m_shield.delay = 1f / m_shield.delay;

        m_shield.enabled = true;
        m_shield.delayTimer = 0f;
        if (m_firstTimeShieldSetup)
        {
            m_shield.value = m_shield.capacity;
            m_firstTimeShieldSetup = false;
        }
        else
        {
            m_shield.value = Mathf.Clamp(m_shield.value, 0f, m_shield.capacity);
        }
    }

    void SetupEquipmentShield()
    {
        List<EquipmentAbility> shieldAbilities = FindEquipmentAbilities(EquipmentAbility.eAbilityType.Shield);

        bool shieldOwned = shieldAbilities.Count > 0;
        if (shieldOwned)
        {
            SetUpShield(shieldAbilities);
        }
        m_shieldSpriteRenderer.gameObject.SetActive(shieldOwned);
        m_shieldBarRef.gameObject.SetActive(shieldOwned);
        m_shieldBarRef.SetMaxProgressValue(m_shield.capacity);
        m_battleManagerRef.RefreshUIShieldBar();
    }

    void SetUpArmorSegments()
    {
        m_armorSegments = new GameObject[m_playerCellRef.m_equippedEquipment.Length];
        for (int i = 0; i < m_playerCellRef.m_equippedEquipment.Length; i++)
        {
            if (m_playerCellRef.m_equippedEquipment[i] != null)
            {
                Vector3 spawnPos = Vector3.zero;
                switch (i)
                {
                    case 0:
                        spawnPos = new Vector3(-1f, 1f).normalized;
                        break;
                    case 1:
                        spawnPos = new Vector3(1f, 1f).normalized;
                        break;
                    case 2:
                        spawnPos = new Vector3(-1f, -1f).normalized;
                        break;
                    case 3:
                        spawnPos = new Vector3(1f, -1f).normalized;
                        break;
                }
                //float angle = i * 90f - 45f;
                Quaternion spawnRot = Quaternion.Euler(0f, 0f, VLib.Vector2ToEulerAngle(spawnPos));
                spawnPos.z = transform.position.z;
                spawnPos *= m_armorSegmentOffset;
                spawnPos += transform.position;
                m_armorSegments[i] = Instantiate<GameObject>(m_armorSegmentPrefab, spawnPos, spawnRot, transform);
            }
        }
        RefreshArmorSegments();
    }

    void RefreshArmorSegments()
    {
        for (int i = 0;i < m_armorSegments.Length;i++)
        {
            if (m_armorSegments[i] != null)
            {
                float healthScale = m_playerCellRef.m_equippedEquipment[i].m_health / m_playerCellRef.m_equippedEquipment[i].m_maxHealth;
                m_armorSegments[i].GetComponent<SpriteRenderer>().color = new Color(1f, healthScale, healthScale);
            }
        }
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

    EquipmentAbility FindActiveEquipmentAbility()
    {
        EquipmentAbility ability = null;
        EquipmentAbility[] abilityList = m_battleManagerRef.m_activeAbilities;
        for (int i = 0; i < abilityList.Length; i++)
        {
            if (abilityList[i] != null && abilityList[i].m_activated)
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
        m_statHandler.m_stats[(int)eCharacterStatIndices.dexterity].ChangeXP(m_flingDexterityXP * GameHandler.BATTLE_SkillXPScale);

        //Handle Projectile Shooting
        EquipmentAbility abil = FindActiveEquipmentAbility();
        if (abil != null)
        {
            if (abil.m_abilityType == EquipmentAbility.eAbilityType.Bullet)
            {
                abil.m_ammo--;
                abil.m_activated = false;
                ShootProjectile(a_flingVector, abil);
            }
            else if (abil.m_abilityType == EquipmentAbility.eAbilityType.Snare)
            {
                abil.m_ammo--;
                abil.m_activated = false;
                ShootProjectile(a_flingVector, abil);
            }

        }
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
                if (m_originalFlingPos.y < m_battleManagerRef.m_upperLowerFlingPositionBounds && m_originalFlingPos.y > -m_battleManagerRef.m_upperLowerFlingPositionBounds)
                {
                    m_flinging = true;
                }
            }
        }
        else
        {
            bool m_validFling = true;
            if (Input.touchCount >= 2)
            {
                m_flinging = false;
                return;
            }
            m_flingLine.enabled = true;
            Vector3 worldMousePoint = m_cameraRef.ScreenToWorldPoint(Input.mousePosition);


            if (worldMousePoint.y >= m_battleManagerRef.m_upperLowerFlingPositionBounds || worldMousePoint.y <= -m_battleManagerRef.m_upperLowerFlingPositionBounds)
            {
                m_validFling = false;
            }

            Vector3 deltaMousePos = m_originalFlingPos - worldMousePoint;
            if (deltaMousePos.magnitude > m_maxFlingLength)
            {
                deltaMousePos = deltaMousePos.normalized * m_maxFlingLength;
            }
            else if (deltaMousePos.magnitude < m_minFlingLength)
            {
                m_validFling = false;
            }

            Vector3[] linePositions = new Vector3[2];

            linePositions[0] = transform.position;
            linePositions[1] = transform.position - deltaMousePos;


            if (!m_validFling)
            {
                m_invalidFlingCross.gameObject.transform.position = new Vector3(worldMousePoint.x, worldMousePoint.y);
            }
            m_invalidFlingCross.enabled = !m_validFling;


            m_flingLine.SetPositions(linePositions);
            
            //If the release point is outside the map, cancel the shot
            if (!Input.GetMouseButton(0))
            {
                if (m_validFling)
                {
                    Fling(deltaMousePos, GameHandler.BATTLE_FlingStrength);
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
        RetreatZone retreatZone = a_collision.gameObject.GetComponent<RetreatZone>();
        bool runningBaseCollision = false;
        bool tookDamage = false;

        Vector2 collisionPoint2d = a_collision.GetContact(0).point;
        Vector3 collisionPoint = new Vector3(collisionPoint2d.x, collisionPoint2d.y, transform.position.z);



        if (enemy)//If collided with an enemy
        {
            if (true)//m_health <= m_statHandler.m_stats[(int)eCharacterStatIndices.constitution].m_finalValue / 3f)//
            {
                if (!m_battleManagerRef.m_endingGame)
                {
                    m_hitSlowdownActive = true;
                    m_battleManagerRef.SetTimeScale(m_hitTimeSlowdownRate);
                }
            }
            m_battleManagerRef.UseExtraTurn();
        }
        else if (retreatZone)
        {
            if (!m_battleManagerRef.m_endingGame)
            {
                m_battleManagerRef.Retreat();
                m_rigidBody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
                transform.position = retreatZone.transform.position;
            }
        }

        switch (m_gameHandlerRef.m_currentGameMode)
        {
            case GameHandler.eGameMode.TurnLimit:
                break;
            case GameHandler.eGameMode.Health:
                if (a_collision.gameObject.GetComponent<Pocket>())
                {
                    TakePocketDamage(a_collision.contacts[0].point);
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
        m_statHandler.m_stats[(int)eCharacterStatIndices.dexterity].ChangeXP(m_abilityUsageDexterityXP * GameHandler.BATTLE_SkillXPScale);

        projectileComp.Initialise(a_ability, a_shootVector, m_rigidBody.velocity, m_projectileDamageMult);
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

    void DestroyArmorSegment(int a_id)
    {
        Destroy(m_armorSegments[a_id]);
        m_armorSegments[a_id] = null;
        SetupEquipmentShield();
    }

    public override void Damage(float a_damage, Vector2 a_damagePoint)
    {
        if (m_battleManagerRef.m_endingGame)
        {
            return;
        }
        float damage = a_damage;
        //Shield
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

        //Armor Protection
        float armourProtectionAmount = m_statHandler.m_stats[(int)eCharacterStatIndices.protection].m_finalValue * 0.1f;
        float blockedAmount = armourProtectionAmount > damage ? damage : armourProtectionAmount;
        if (blockedAmount > 0f)
        {
            SpawnBlockText(blockedAmount);
        }
        m_statHandler.m_stats[(int)eCharacterStatIndices.protection].ChangeXP((int)blockedAmount);
        damage -= blockedAmount;

        //Equipment Segment
        float damageAngle = VLib.Vector2ToEulerAngle(a_damagePoint - transform.position.ToVector2());
        damageAngle = 360f - damageAngle;
        if (damageAngle < 0f)
        {
            damageAngle += 360f;
        }
        else if (damageAngle > 360f)
        {
            damageAngle -= 360f;
        }
        damageAngle += 90f;
        int damagedEquipmentSlot = (int)((damageAngle % 360f) / 90f);

        //Change to left to right top to bottom instead of rotating like a clock
        if (damagedEquipmentSlot == 2)
        {
            damagedEquipmentSlot = 3;
        }
        else if (damagedEquipmentSlot == 3)
        {
            damagedEquipmentSlot = 2;
        }
        Debug.Log(damagedEquipmentSlot);
        Equipment affectedEquipment = m_playerCellRef.m_equippedEquipment[damagedEquipmentSlot];
        if (m_armorSegments[damagedEquipmentSlot] != null)
        {
            damage = affectedEquipment.Damage(damage);
            if (damage > 0)
            {
                DestroyArmorSegment(damagedEquipmentSlot);
            }
        }
        damage = Mathf.Clamp(damage, 0f, float.MaxValue);
        m_statHandler.m_stats[(int)eCharacterStatIndices.constitution].ChangeXP((int)damage * GameHandler.BATTLE_SkillXPScale);
        base.Damage(damage, a_damagePoint);
        m_battleManagerRef.m_healthBarRef.SetBarValue(m_health);
        RefreshArmorSegments();
        m_battleManagerRef.RefreshAbilityButtons();
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
             Damage(100f, Vector2.zero);
        }
    }

    internal void ReportDamageDealt(float m_lastDamageTaken)
    {
        m_statHandler.m_stats[(int)eCharacterStatIndices.strength].ChangeXP((int)(m_lastDamageTaken * GameHandler.BATTLE_SkillXPScale));
    }
}