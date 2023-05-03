using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Damageable
{
    Player m_playerRef;
    Nucleus m_nucleusRef = null;
    public GameObject m_coinPrefab;

    public GameObject[] m_exclamationMarkRefs;
    public GameObject m_flingReadinessIndicatorRef;
    public GameObject m_velocityIndicatorRef;
    public GameObject m_visionConeRef;
    public Sprite m_idleBacteriaSprite;
    public Sprite m_idleBacteriaShadowSprite;
    public enum eEnemyType
    {
        //Null = -1,
        Idler,
        Dodger,
        Striker,
        Count
    }public eEnemyType m_enemyType;

    public struct TypeTrait
    {
        public Enemy.eEnemyType type;
        public int difficulty;
        public bool flinger;
        public bool dodger;
        public bool duplicator;
        public bool canRotate;
    } TypeTrait m_typeTrait;

    float m_duplicationTimer = 0f;
    float m_duplicationTimerMax = 12f;

    vTimer m_nucleusDrainTimer;

    private float m_scoreValue = 1f;
    int m_xpReward = 5;
    const float m_coinToXPRatio = 0.2f;

    float m_xpTextYOffset = 0.2f;

    float m_sightRadius = 4f;

    //Flinging
    public float m_flingTimer = 0f;
    public float m_flingTimerMax = 3f;
    float m_flingAccuracy = 20f;
    bool m_flinging;

    float m_coinSpawnOffset = 0.3f;
    int m_coinsToSpawn = 3;
    float m_closestCoinSpawnAngle = 15f;

    Vector2 m_duplicationPositionClampMax = new Vector2(1.956f,2.84f);
    Vector2 m_duplicationPositionClampMin = new Vector2(-1.956f,-3.46f);

    bool m_dead = false;

    public override void Awake()
    {
        base.Awake();
        m_typeTrait = new TypeTrait();
        m_flingTimer -= UnityEngine.Random.Range(0f, 0.3f);
        m_damageTextColor = Color.yellow;
    }

    public void SetUpType(eEnemyType a_type)
    {
        m_typeTrait = GameHandler.m_enemyTypeTraits[(int)a_type];
        m_enemyType = a_type;
        m_visionConeRef.SetActive(false);
        switch (m_enemyType)
        {
            case eEnemyType.Idler:
                m_originalColor = Color.yellow;
                m_statHandler.m_stats[(int)eStatIndices.constitution].finalValue /= 2f;
                transform.localScale *= 1f;
                m_rigidBody.mass *= 0.45f;
                m_rigidBody.drag = 0.1f;
                m_rigidBody.freezeRotation = false;
                CapsuleCollider2D capsuleCollider = GetComponent<CapsuleCollider2D>();
                capsuleCollider.offset = new Vector2(-0.013965f, -0.09077191f);
                capsuleCollider.size = new Vector2(0.75793f, 2.911544f);
                m_spriteRenderer.sprite = m_idleBacteriaSprite;
                m_spriteRenderer.color = Color.white;
                m_shadowRef.GetComponent<SpriteRenderer>().sprite = m_idleBacteriaShadowSprite;
                m_flingReadinessIndicatorRef.SetActive(false);
                m_nucleusDrainTimer = new vTimer(1.0f, false, false);
                break;
            case eEnemyType.Striker:
                m_originalColor = Color.red;
                m_visionConeRef.SetActive(true);
                break;
            case eEnemyType.Dodger:
                m_originalColor = Color.green;
                m_flingAccuracy = 360f;
                break;
            case eEnemyType.Count:
                break;
            default:
                break;
        }
        m_xpReward = (int)(m_typeTrait.difficulty * GameHandler.GAME_enemyXPRewardScale);
        m_scoreValue = (float)(m_xpReward) * m_coinToXPRatio;
        m_rigidBody.freezeRotation = !m_typeTrait.canRotate;
        UpdateLocalStatsFromStatHandler();
        UpdateHealthColor();
    }

    public override void Start()
    {
        base.Start();
        m_playerRef = FindObjectOfType<Player>();
        m_velocityIndicatorRef.SetActive(m_gameHandlerRef.m_upgrades[(int)GameHandler.UpgradeId.enemyVector].m_owned);
        for (int i = 0; i < m_exclamationMarkRefs.Length; i++)
        {
            m_exclamationMarkRefs[i].SetActive(false);
        }
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
        m_battleManagerRef.ChangeEnemyCount(1);
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

    public override void OnCollisionEnter2D(Collision2D a_collision)
    {
        base.OnCollisionEnter2D(a_collision);
        if (a_collision.gameObject.GetComponent<Pocket>())
        {
            switch (m_gameHandlerRef.m_currentGameMode)
            {
                case GameHandler.eGameMode.TurnLimit:
                    break;
                case GameHandler.eGameMode.Health:
                    TakePocketDamage();
                    PocketFling(a_collision.gameObject.transform.position);
                    break;
                case GameHandler.eGameMode.Pockets:
                    Die();
                    break;
                case GameHandler.eGameMode.Hunger:
                    break;
                case GameHandler.eGameMode.ModeCount:
                    break;
                default:
                    break;
            }
        }
        else if (a_collision.gameObject.GetComponent<Nucleus>() != null && m_enemyType == eEnemyType.Idler)
        {
            m_nucleusRef = a_collision.gameObject.GetComponent<Nucleus>();
            m_rigidBody.constraints = RigidbodyConstraints2D.FreezePosition;
            m_rigidBody.freezeRotation = true;
            m_nucleusDrainTimer.SetActive(true);
        }
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
        if (m_typeTrait.duplicator)
        {
            DuplicationUpdate();
        }
        AIUpdate();
        m_healthBarRef.transform.localEulerAngles = -transform.localEulerAngles;

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

        VisionConeUpdate();
    }
    public override void Die()
    {
        if(m_dead)
        {
            return;
        }
        m_dead = true;
        m_battleManagerRef.ChangeScore(m_scoreValue);
        m_gameHandlerRef.m_playerStatHandler.ChangeXP(m_xpReward);
        m_gameHandlerRef.m_xpEarnedLastGame += m_xpReward;
        m_battleManagerRef.ChangeXp(m_xpReward);
        m_battleManagerRef.ChangeInvaderStrength(-m_typeTrait.difficulty);

        RisingFadingText xpText = Instantiate(m_risingFadingTextPrefab, transform.position + new Vector3(0f, m_xpTextYOffset), new Quaternion(), FindObjectOfType<Canvas>().transform).GetComponent<RisingFadingText>();
        xpText.SetImageEnabled(false);
        xpText.SetGravityAffected(false);
        xpText.SetHorizontalSpeed(0f);
        xpText.SetLifeTimerMax(1.35f);
        xpText.SetTextContent("XP +" + m_xpReward);
        xpText.SetOriginalColor(Color.cyan);
        xpText.SetOriginalScale(1.2f);

        m_battleManagerRef.ChangeEnemyCount(-1);

        float[] spawnDirection;

        //Spawn coins
        for (int i = 0; i < m_coinsToSpawn; i++)
        {
            spawnDirection = new float[m_coinsToSpawn];

            spawnDirection[i] = UnityEngine.Random.Range(0f, 360f);
            for (int j = 0; j < i; j++)
            {
                if ((spawnDirection[i] - spawnDirection[j] <= m_closestCoinSpawnAngle) || (spawnDirection[i] - spawnDirection[j] >= 360f - m_closestCoinSpawnAngle))
                {
                    spawnDirection[i] = UnityEngine.Random.Range(0f, 360f);
                    j--;
                }
            }

            Vector3 spawnLocation = new Vector3(m_coinSpawnOffset, 0f, 0f);
            spawnLocation = Quaternion.AngleAxis(spawnDirection[i], Vector3.forward) * spawnLocation;
            spawnLocation = transform.position + spawnLocation;

            
            float xClamp = 2.1f;
            float yClamp = 3.52f;
            float xClampYCutoff = 3.148f;
            float yClampXCutoff = 1.527f;

            //Make sure the coins dont spawn inside the pockets
            if (spawnLocation.y > xClampYCutoff)
            {
                float sin = Mathf.Sin(((spawnLocation.y - xClampYCutoff) / (yClamp - xClampYCutoff)) * Mathf.PI / 2f);
                xClamp = yClampXCutoff + (1f-sin) * (xClamp-yClampXCutoff); 
            }
            else if (spawnLocation.y < -xClampYCutoff)
            {
                float sin = Mathf.Sin((spawnLocation.y + xClampYCutoff) / (-yClamp + xClampYCutoff) * Mathf.PI / 2f);
                xClamp = yClampXCutoff + (1f-sin) * (xClamp - yClampXCutoff);
            }

            spawnLocation = new Vector3(Mathf.Clamp(spawnLocation.x, -xClamp, xClamp), Mathf.Clamp(spawnLocation.y, -3.52f, 3.52f), spawnLocation.z);
            GameObject coin =Instantiate<GameObject>(m_coinPrefab, transform.position, new Quaternion());
            coin.GetComponent<Coin>().Init(spawnLocation);
        }

        base.Die();
    }

    public void Fling()
    {
        if (m_playerRef)
        {
            Vector3 playerPos = m_playerRef.transform.position;

            if ((playerPos - transform.position).magnitude <= m_sightRadius)
            {
                Vector3 accurateFlingVector = (playerPos - transform.position).normalized;

                Vector3 aimDisturbance = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, m_flingAccuracy) - m_flingAccuracy / 2f, Vector3.forward) * accurateFlingVector;

                Fling(aimDisturbance, m_statHandler.m_stats[(int)eStatIndices.dexterity].finalValue);
            }
        }
       
    }

    private void FlingUpdate()
    {
        Vector3 playerPos = m_playerRef.transform.position;

        if ((playerPos - transform.position).magnitude <= m_sightRadius)
        {
            if (m_typeTrait.flinger)
            {
                m_visionConeRef.SetActive(true);
            }
            for (int i = 0; i < m_exclamationMarkRefs.Length; i++)
            {
                m_exclamationMarkRefs[i].SetActive(true);
            }
            m_flingTimer += Time.deltaTime;
            if (m_flingTimer >= m_flingTimerMax)
            {
                m_flingTimer -= m_flingTimerMax;

                Fling();
            }
            return;
        }
        else
        {
            for (int i = 0; i < m_exclamationMarkRefs.Length; i++)
            {
                m_exclamationMarkRefs[i].SetActive(false);
            }
            m_visionConeRef.SetActive(false);
        }
    }

    //AI
    private void AIUpdate()
    {
        if (m_playerRef)
        {
            if (m_typeTrait.flinger || m_typeTrait.dodger)
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
