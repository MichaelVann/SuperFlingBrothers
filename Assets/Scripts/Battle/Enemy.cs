using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Damageable
{
    Player m_playerRef;
    Nucleus m_nucleusRef = null;
    public GameObject m_coinPrefab;
    public GameObject m_equipmentDropPrefab;

    public GameObject m_flingReadinessIndicatorRef;
    public GameObject m_velocityIndicatorRef;
    public GameObject m_visionConeRef;
    public Sprite m_idleBacteriaSprite;
    public Sprite[] m_enemySprites;
    public Sprite m_idleBacteriaShadowSprite;
    public FlingPieIndicator m_flingPieIndicatorRef;
    public GameObject m_flingDirectionIndicatorRef;

    Vector3 m_nextFlingDirection;

    public enum eEnemyType
    {
        //Null = -1,
        Idler,
        InertiaDasher,
        Dodger,
        Healer,
        Striker,
        Count
    }public eEnemyType m_enemyType;

    public struct TypeTrait
    {
        public Enemy.eEnemyType type;
        public int difficulty;
        public bool flinger;
        public bool inertiaDasher;
        public bool dodger;
        public bool duplicator;
        public bool canRotate;
        public bool healer;
    } TypeTrait m_typeTrait;

    public static Enemy.TypeTrait[] m_enemyTypeTraits = new Enemy.TypeTrait[(int)Enemy.eEnemyType.Count];

    //Player Vulnerability
    internal bool m_playerVulnerable = false;
    const float m_playerVulnerableLength = 0.3f;
    vTimer m_playerVulnerableTimer;

    float m_duplicationTimer = 0f;
    float m_duplicationTimerMax = 12f;

    vTimer m_nucleusDrainTimer;

    private float m_scoreValue = 1f;
    int m_xpReward = 5;

    float m_xpTextYOffset = 0.2f;

    float m_sightRadius = 4f;

    //Flinging
    public float m_flingTimer = 0f;
    public float m_flingTimerMax = 3f;
    float m_flingAccuracy = 20f;

    //Loot
    float m_lootSpawnOffset = 0.3f;
    int m_coinsToSpawn = 3;
    float m_equipmentDropRate = 0.1f;

    Vector2 m_duplicationPositionClampMax = new Vector2(1.956f,2.84f);
    Vector2 m_duplicationPositionClampMin = new Vector2(-1.956f,-3.46f);

    bool m_dead = false;

    public override void Awake()
    {
        base.Awake();
        m_typeTrait = new TypeTrait();
        m_flingTimer -= UnityEngine.Random.Range(0f, 0.3f);
        m_damageTextColor = Color.yellow;

        m_playerVulnerableTimer = new vTimer(m_playerVulnerableLength, true);

    }

    public void SetUpType(eEnemyType a_type)
    {
        m_typeTrait = m_enemyTypeTraits[(int)a_type];
        m_enemyType = a_type;
        m_visionConeRef.SetActive(false);
        switch (m_enemyType)
        {
            case eEnemyType.Idler:
                m_originalColor = Color.white;
                m_statHandler.m_stats[(int)eCharacterStatType.constitution].m_finalValue /= 2f;
                transform.localScale *= 1f;
                m_rigidBody.mass *= 0.75f;
                m_rigidBody.drag = 1f;
                m_nucleusDrainTimer = new vTimer(1.0f, false, false);
                m_equipmentDropRate = 0.1f;
                break;
            case eEnemyType.InertiaDasher:
                m_rotateToAlignToVelocity = true;
                m_equipmentDropRate = 0.3f;
                break;
            case eEnemyType.Dodger:
                m_originalColor = Color.green;
                m_equipmentDropRate = 0.3f;
                break;
            case eEnemyType.Healer:
                m_originalColor = Color.white;
                m_equipmentDropRate = 0.3f;
                break;
            case eEnemyType.Striker:
                m_visionConeRef.SetActive(true);
                m_equipmentDropRate = 0.5f;
                break;
            case eEnemyType.Count:
                break;
            default:
                break;
        }

        m_equipmentDropRate *= GameHandler.BATTLE_EnemyEquipmentDropChanceScale;

        m_xpReward = (int)(m_typeTrait.difficulty * GameHandler.GAME_enemyXPRewardScale);
        m_scoreValue = 0f;
        m_rigidBody.freezeRotation = !m_typeTrait.canRotate;
        if (m_typeTrait.dodger)
        {
            m_flingAccuracy = 360f;
            m_flingDirectionIndicatorRef.SetActive(true);
        }
        else
        {
            m_flingDirectionIndicatorRef.SetActive(false);
        }


        if (m_typeTrait.flinger)
        {
            m_visionConeRef.SetActive(true);
        }

        //Store next fling direction ahead of time, rotate fling direction indicator towards it
        FindNextRandomFlingDirection();

        //m_rotateToAlignToVelocity = m_typeTrait.canRotate;
        UpdateLocalStatsFromStatHandler();
        UpdateHealthColor();
    }

    internal static void SetUpEnemyTypes()
    {
        //Defaults
        for (int i = 0; i < (int)Enemy.eEnemyType.Count; i++)
        {
            m_enemyTypeTraits[i].type = Enemy.eEnemyType.Idler;
            m_enemyTypeTraits[i].difficulty = 1;
            m_enemyTypeTraits[i].inertiaDasher = false;
            m_enemyTypeTraits[i].flinger = false;
            m_enemyTypeTraits[i].dodger = false;
            m_enemyTypeTraits[i].duplicator = false;
            m_enemyTypeTraits[i].canRotate = false;
        }

        //Idler
        m_enemyTypeTraits[(int)eEnemyType.Idler].type = Enemy.eEnemyType.Idler;
        m_enemyTypeTraits[(int)eEnemyType.Idler].difficulty = 1;
        m_enemyTypeTraits[(int)eEnemyType.Idler].canRotate = true;

        //Inertia Dasher
        m_enemyTypeTraits[(int)eEnemyType.InertiaDasher].type = Enemy.eEnemyType.InertiaDasher;
        m_enemyTypeTraits[(int)eEnemyType.InertiaDasher].inertiaDasher = true;
        m_enemyTypeTraits[(int)eEnemyType.InertiaDasher].difficulty = 3;
        m_enemyTypeTraits[(int)eEnemyType.InertiaDasher].canRotate = true;

        //Dodger
        m_enemyTypeTraits[(int)eEnemyType.Dodger].type = Enemy.eEnemyType.Dodger;
        m_enemyTypeTraits[(int)eEnemyType.Dodger].dodger = true;
        m_enemyTypeTraits[(int)eEnemyType.Dodger].difficulty = 5;

        //Healer
        m_enemyTypeTraits[(int)eEnemyType.Healer].type = Enemy.eEnemyType.Healer;
        m_enemyTypeTraits[(int)eEnemyType.Healer].dodger = true;
        m_enemyTypeTraits[(int)eEnemyType.Healer].difficulty = 8;
        m_enemyTypeTraits[(int)eEnemyType.Healer].canRotate = true;
        m_enemyTypeTraits[(int)eEnemyType.Healer].duplicator = false;
        m_enemyTypeTraits[(int)eEnemyType.Healer].healer = true;

        //Striker
        m_enemyTypeTraits[(int)eEnemyType.Striker].type = Enemy.eEnemyType.Striker;
        m_enemyTypeTraits[(int)eEnemyType.Striker].flinger = true;
        m_enemyTypeTraits[(int)eEnemyType.Striker].difficulty = 12;
    }

    static internal int GetHighestEnemyDifficulty()
    {
        int maxDifficulty = 0;
        for (int i = 0; i < m_enemyTypeTraits.Length; i++)
        {
            maxDifficulty = maxDifficulty < m_enemyTypeTraits[i].difficulty ? m_enemyTypeTraits[i].difficulty : maxDifficulty;
        }
        return maxDifficulty;
    }

    public override void Start()
    {
        base.Start();
        m_flingPieIndicatorRef.SetPieFillAmount(0);
        m_playerRef = FindObjectOfType<Player>();
        m_velocityIndicatorRef.SetActive(m_gameHandlerRef.m_upgradeTree.m_upgradeItemList[(int)UpgradeItem.UpgradeId.enemyVector].m_owned);
    }

    public void Copy(Enemy a_ref)
    {
        SetUpType(a_ref.m_enemyType);
        m_health = a_ref.m_health;
        m_originalMass = a_ref.m_originalMass;
        m_originalColor = a_ref.m_originalColor;
        UpdateHealthColor();
    }

    //Duplicates a new enemy heading in the opposite direction
    void Duplicate()
    {
        Vector3 normalisedVelocity = m_rigidBody.velocity.normalized;
        Vector3 spawnLocation = transform.position - normalisedVelocity * 0.4f;
        spawnLocation.x = Mathf.Clamp(spawnLocation.x, m_duplicationPositionClampMin.x, m_duplicationPositionClampMax.x);
        spawnLocation.y = Mathf.Clamp(spawnLocation.y, m_duplicationPositionClampMin.y, m_duplicationPositionClampMax.y);

        GameObject clonedObject = Instantiate(gameObject, spawnLocation, new Quaternion());
        Enemy clonedEnemy = clonedObject.GetComponent<Enemy>();
        clonedEnemy.m_rigidBody.velocity = -m_rigidBody.velocity;
        clonedEnemy.Copy(this);
        m_battleManagerRef.ChangeEnemyCount(1, clonedEnemy.m_enemyType);
    }

    void DuplicationUpdate()
    {
        m_duplicationTimer += Time.deltaTime;
        if (m_duplicationTimer >= m_duplicationTimerMax)
        {
            if (m_rigidBody.velocity.magnitude != 0.0f)
            {
                Duplicate();
            }
            m_duplicationTimer = 0.0f;
        }
    }

    void FindNextRandomFlingDirection()
    {
        float angle = VLib.vRandom(0f, 360f);
        m_nextFlingDirection = VLib.RotateVector3In2D(new Vector3(1f,0f,0f), angle);
        m_flingDirectionIndicatorRef.transform.eulerAngles = new Vector3(0f,0f,VLib.Vector2ToEulerAngle(m_nextFlingDirection.ToVector2())) - transform.eulerAngles;
        //m_flingDirectionIndicatorRef.transform.rotation = VLib.Vector2DirectionToQuaternion(m_nextFlingDirection);
    }

    public override void OnCollisionEnter2D(Collision2D a_collision)
    {
        bool runningBaseCollision = true;
        Enemy oppEnemy = a_collision.gameObject.GetComponent<Enemy>();
        Player oppPlayer = a_collision.gameObject.GetComponent<Player>();
        if (a_collision.gameObject.GetComponent<Pocket>())
        {
            TakePocketDamage(a_collision.contacts[0].point);
            PocketFling(a_collision.gameObject.transform.position);
            m_musicPlayerRef.PlayPocketSound();
        }
        else if (a_collision.gameObject.GetComponent<Nucleus>() != null && m_enemyType == eEnemyType.Idler)
        {
            m_nucleusRef = a_collision.gameObject.GetComponent<Nucleus>();
            m_rigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
            m_rigidBody.freezeRotation = true;
            m_nucleusDrainTimer.SetActive(true);
            runningBaseCollision = true;
        }
        else if (oppEnemy != null)
        {
            if (oppEnemy.m_typeTrait.healer)
            {
                runningBaseCollision = false;
                Heal(oppEnemy.m_statHandler.m_stats[(int)eCharacterStatType.strength].m_finalValue * oppEnemy.m_lastMomentumMagnitude / m_damagePerSpeedDivider);
            }
            runningBaseCollision = false;// !m_typeTrait.healer;
        }
        else if (oppPlayer != null)
        {
            if (m_playerVulnerable)
            {
                runningBaseCollision = false;
            }

            //Counter check
            float damageDealt = CalculateDamageDealtFromCollision(oppPlayer, a_collision);
            if (damageDealt > 0)
            {
                EquipmentAbility activeAbility = oppPlayer.FindActiveEquipmentAbility();
                if (activeAbility != null && activeAbility.m_abilityType == EquipmentAbility.eAbilityType.Parry)
                {
                    activeAbility.Expend();

                    runningBaseCollision = false;
                    FindObjectOfType<BattleUIHandler>().RefreshAbilityButtons();

                    //Take own damage
                    if (damageDealt != 0f)
                    {
                        ReceiveDamageFromCollision(oppPlayer.gameObject, damageDealt, a_collision.contacts[0].point);
                        oppPlayer.m_rigidBody.AddForce((a_collision.contacts[0].point - oppPlayer.transform.position.ToVector2()).normalized * GameHandler.BATTLE_FlingStrength/3f);
                    }
                }
            }
        }

        if (runningBaseCollision)
        {
            base.OnCollisionEnter2D(a_collision);
        }
    }

    internal override void ReceiveDamageFromCollision(GameObject a_collidingGameObject, float a_damage, Vector2 a_contactPoint)
    {
        Player oppPlayer = a_collidingGameObject.GetComponent<Player>();
        base.ReceiveDamageFromCollision(a_collidingGameObject, a_damage, a_contactPoint);
        if (oppPlayer != null)
        {
            m_playerVulnerableTimer.Reset();
            m_playerVulnerable = true;
            oppPlayer.ReportDamageDealt(m_lastDamageTakenFromCollision);
        }
    }

    public override void Damage(float a_damage, Vector2 a_damagePoint)
    {
        base.Damage(a_damage, a_damagePoint);
    }

    private void VisionConeUpdate()
    {
        if (m_visionConeRef.activeSelf && m_playerRef)
        {
            Vector3 deltaVec = (m_playerRef.transform.position - transform.position).normalized;
            float angle = VLib.Vector2ToEulerAngle(deltaVec);
            m_visionConeRef.transform.eulerAngles = new Vector3(0f, 0f,angle);
            float deltaMag = (m_playerRef.transform.position - transform.position).magnitude;
            m_visionConeRef.transform.localScale = new Vector3(m_visionConeRef.transform.localScale.x, deltaMag / 1.6f, 1f);
        }
    }

    public override void Update()
    {
        base.Update();
        if (m_playerVulnerableTimer.Update())
        {
            m_playerVulnerable = false;
        }

        if (m_typeTrait.duplicator)
        {
            DuplicationUpdate();
        }

        AIUpdate();

        if (m_enemyType == eEnemyType.Idler)
        {
            if (m_nucleusDrainTimer.Update())
            {
                if (m_nucleusRef != null)
                {
                    m_nucleusRef.Damage();
                }
            }
        }

        if (m_typeTrait.canRotate && m_flingDirectionIndicatorRef.gameObject.activeSelf)
        {
            m_flingDirectionIndicatorRef.transform.eulerAngles = new Vector3(0f, 0f, VLib.Vector2ToEulerAngle(m_nextFlingDirection.ToVector2()));
            //m_flingDirectionIndicatorRef.transform.eulerAngles = -transform.eulerAngles;
        }

        VisionConeUpdate();
    }

    private void SpawnLoot(GameObject a_lootTemplate, int a_lootToSpawn = 1)
    {
        Loot.SpawnLoot(m_battleManagerRef, a_lootTemplate, m_lootSpawnOffset, transform.position, a_lootToSpawn);
    }

    void SpawnXpText()
    {
        m_battleManagerRef.m_uiHandlerRef.SpawnXpRisingText(m_risingFadingTextPrefab, m_xpReward);
    }

    void SpawnDeathLoot()
    {
        //Spawn coins
        SpawnLoot(m_coinPrefab, m_coinsToSpawn);
        int equipmentDrops = 0;
        while (UnityEngine.Random.Range(0f, 1f) < m_equipmentDropRate)
        {
            equipmentDrops++;
        }
        SpawnLoot(m_equipmentDropPrefab, equipmentDrops);

        if (UnityEngine.Random.Range(0f, 1f) < 0.1f)
        {
            Instantiate<GameObject>(m_equipmentDropPrefab, transform.position, new Quaternion());
        }
    }

    public override void Die()
    {
        if(m_dead)
        {
            return;
        }
        m_dead = true;
        m_battleManagerRef.ChangeScore(m_scoreValue);
        m_gameHandlerRef.m_xCellSquad.m_statHandler.m_RPGLevel.ChangeXP(m_xpReward);
        m_gameHandlerRef.m_lastGameStats.m_xpEarnedLastGame += m_xpReward;
        m_battleManagerRef.ChangeXp(m_xpReward);
        m_battleManagerRef.ChangeInvaderStrength(-m_typeTrait.difficulty);

        SpawnXpText();

        m_battleManagerRef.ChangeEnemyCount(-1, m_enemyType);

        SpawnDeathLoot();

        base.Die();
    }

    public void Fling()
    {
        float flingStrength = GameHandler.BATTLE_FlingStrength;

        if (m_typeTrait.inertiaDasher)
        {
            Fling(m_rigidBody.velocity.normalized, flingStrength);
        }
        else if (m_typeTrait.dodger)
        {
            Fling(m_nextFlingDirection, flingStrength);
            FindNextRandomFlingDirection();
        }
        else if (m_playerRef)
        {
            Vector3 playerPos = m_playerRef.transform.position;

            Vector3 accurateFlingVector = (playerPos - transform.position).normalized;

            Vector3 aimDisturbance = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, m_flingAccuracy) - m_flingAccuracy / 2f, Vector3.forward) * accurateFlingVector;

            Fling(aimDisturbance, flingStrength);
        }
    }

    void RunFlingTimer()
    {
        m_flingTimer += Time.deltaTime;
        if (m_flingTimer >= m_flingTimerMax)
        {
            m_flingTimer -= m_flingTimerMax;
            Fling();
        }
    }

    private void FlingUpdate()
    {
        m_flingPieIndicatorRef.SetPieFillAmount(m_flingTimer / m_flingTimerMax);
        Vector3 playerPos = m_playerRef.transform.position;
        RunFlingTimer();
    }

    //AI
    private void AIUpdate()
    {
        if (m_playerRef)
        {
            if ((m_typeTrait.inertiaDasher && m_rigidBody.velocity.magnitude > Mathf.Epsilon) || m_typeTrait.flinger || m_typeTrait.dodger)
            {
                FlingUpdate();
            }
        }
    }

    public override void Fling(Vector3 a_flingVector, float a_flingStrength)
    {
        base.Fling(a_flingVector, a_flingStrength);
    }
}
