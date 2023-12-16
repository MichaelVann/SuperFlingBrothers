using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIHandler : MonoBehaviour
{
    BattleManager m_battleManagerRef;
    GameHandler m_gameHandlerRef;

    [SerializeField] XpBarHandler m_xpBarHandlerRef;
    [SerializeField] Text m_scoreText;

    public bool m_gameOver = false;
    public GameObject m_endingText;
    public Text m_endingHighlightText;
    public bool m_playingEnding = false;


    //Ability Buttons
    public AbilityButton[] m_abilityButtons;
    public GameObject[] m_abilityButtonSlots;
    public ActivatedAbilityPanel m_activatedAbilityPanel;

    void Awake()
    {
        m_battleManagerRef = GetComponent<BattleManager>();
    }

    private void Start()
    {
        m_gameHandlerRef = m_battleManagerRef.m_gameHandlerRef;
    }

    public void StartEnding(eEndGameType a_type)
    {
        string endTextString = "";
        Color m_highlightColor = Color.white;
        Color m_bgColor = Color.white;

        switch (a_type)
        {
            case eEndGameType.retreat:
                endTextString = "Retreat!";
                m_highlightColor = Color.white;
                m_bgColor = Color.green;
                break;
            case eEndGameType.win:
                endTextString ="Victory!";
                m_highlightColor = Color.white;
                m_bgColor = Color.blue;
                break;
            case eEndGameType.lose:
                endTextString = "Defeat!";
                m_highlightColor = Color.red;
                m_bgColor = Color.black;
                break;
            default:
                break;
        }

        //Ending Text
        m_endingText.SetActive(true);
        m_endingText.GetComponent<Text>().color = m_bgColor;
        m_endingText.GetComponent<Text>().text = endTextString;
        m_endingHighlightText.text = endTextString;
        m_endingHighlightText.color = m_highlightColor;

        m_gameOver = true;
        m_playingEnding = true;
        m_endingText.SetActive(true);
    }

    public void PlayEnding()
    {
        float textScale = (m_battleManagerRef.m_gameEndTimer / m_battleManagerRef.GetMaxGameEndTimer())/ m_battleManagerRef.m_gameEndSlowdownFactor; //Mathf.Pow(m_battleManagerRef.m_gameEndTimer, 1f); 
        if (textScale > 1f)
        {
            textScale = 1f;
        }
        m_endingText.transform.localScale = new Vector3(textScale, textScale, 1f);
    }

    internal void SpawnXpRisingText(GameObject a_risingTextPrefab, float a_xpReward)
    {
        RisingFadingText xpText = Instantiate(a_risingTextPrefab, m_xpBarHandlerRef.gameObject.transform.position + new Vector3(400f, 0f, 0f), new Quaternion(), m_battleManagerRef.m_canvasRef.transform).GetComponent<RisingFadingText>();
        xpText.SetImageEnabled(false);
        xpText.SetGravityAffected(false);
        xpText.SetHorizontalSpeed(0f);
        xpText.SetLifeTimerMax(1.35f);
        xpText.SetTextContent("XP +" + a_xpReward);
        xpText.SetOriginalColor(Color.cyan);
        xpText.SetOriginalScale(1.2f);
    }

    internal void SpawnCoinRisingText(GameObject a_risingTextPrefab, float a_value)
    {

    }

    void Update()
    {
        m_scoreText.text = "" + m_battleManagerRef.m_score;

        if (m_gameOver)
        {
            if (m_playingEnding)
            {
                PlayEnding();
            }
        }
    }


    public void RefreshAbilityButtons()
    {
        bool activatedAbility = false;
        for (int i = 0; i < m_abilityButtons.Length; i++)
        {
            if (m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_equippedEquipment[i] != null)
            {
                m_abilityButtons[i].SetEquipmentRef(m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_equippedEquipment[i]);
                if (m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_equippedEquipment[i].IsBroken())
                {
                    m_abilityButtons[i].Disable();
                }
            }
            if (m_abilityButtons[i].m_equipmentRef != null)
            {
                if (m_abilityButtons[i].m_equipmentRef.m_activeAbility.m_activated)
                {
                    activatedAbility = true;
                    m_activatedAbilityPanel.SetEquipmentAbility(m_abilityButtons[i].m_equipmentRef.m_activeAbility);
                }
                m_abilityButtons[i].gameObject.SetActive(true);
                m_abilityButtonSlots[i].SetActive(false);
                m_abilityButtons[i].Refresh();
            }
            else
            {
                m_abilityButtons[i].gameObject.SetActive(false);
                m_abilityButtonSlots[i].SetActive(true);
            }
        }
        m_activatedAbilityPanel.gameObject.SetActive(activatedAbility);
         
    }

    public void ActivateAbility(int a_id)
    {
        m_battleManagerRef.ActivateAbility(a_id);
        m_activatedAbilityPanel.gameObject.SetActive(true);
        RefreshAbilityButtons();
    }


    public void DeactivateAbility()
    {
        m_battleManagerRef.ActivateAbility(-1);
        m_activatedAbilityPanel.gameObject.SetActive(false);
        RefreshAbilityButtons();
    }
}
