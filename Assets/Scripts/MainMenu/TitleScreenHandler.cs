using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;

    public GameObject m_equipmentNotifierRef;
    public Text m_equipmentNotifierTextRef;
    public Text m_versionNumberText;
    [SerializeField] GameObject m_highScoreContentRef;
    [SerializeField] GameObject m_highscorePrefab;
    [SerializeField] GameObject m_noHighscoresTextRef;
    [SerializeField] Button m_playButtonRef;
    List<UIHighScore> m_highScoreList;

    bool m_inited = false;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        if (!m_inited)
        {
            m_highScoreList = new List<UIHighScore>();
            m_gameHandlerRef = FindObjectOfType<GameHandler>();
            m_versionNumberText.text = "Version " + GameHandler.MAIN_VERSION_NUMBER + "." + GameHandler.SUB_VERSION_NUMBER;
            RefreshHighscoreTable();
            CreateFirstTimeDialogs();
            m_inited = true;
        }
    }

    private void OnEnable()
    {
        Init();
        m_playButtonRef.interactable = m_gameHandlerRef.m_tutorialManager.IsTutorialCompleted();
    }

    void CreateFirstTimeDialogs()
    {
        bool messagePlayed = m_gameHandlerRef.m_tutorialManager.AttemptToSpawnMessage(TutorialManager.eMessage.StartUp);
        if (!messagePlayed) 
        {
            m_gameHandlerRef.m_tutorialManager.SpawnTutorial();
        }
    }

    void RefreshHighscoreTable()
    {
        for (int i = 0; i < m_highScoreList.Count; i++)
        {
            Destroy(m_highScoreList[i].gameObject);
        }
        m_highScoreList.Clear();

        m_noHighscoresTextRef.SetActive(m_gameHandlerRef.m_highscoreList.Count == 0);

        for (int i = 0; i < m_gameHandlerRef.m_highscoreList.Count; i++)
        {
            m_highScoreList.Add(Instantiate(m_highscorePrefab, m_highScoreContentRef.transform).GetComponent<UIHighScore>());
            m_highScoreList[i].Init((i + 1), m_gameHandlerRef.m_highscoreList[i].name, m_gameHandlerRef.m_highscoreList[i].score);
        }
    }

    void RefreshEquipmentNotifier()
    {
        int combinedNewStatsAndEquipment = m_gameHandlerRef.m_xCellSquad.m_statHandler.m_RPGLevel.m_allocationPoints + m_gameHandlerRef.m_lastGameStats.m_equipmentCollectedLastGame;

        m_equipmentNotifierRef.SetActive(combinedNewStatsAndEquipment > 0);

        if (m_equipmentNotifierRef.activeSelf)
        {
            if (combinedNewStatsAndEquipment > 9)
            {
                m_equipmentNotifierTextRef.text = "9+";
            }
            else
            {
                m_equipmentNotifierTextRef.text = "" + combinedNewStatsAndEquipment;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        RefreshEquipmentNotifier();
    }
}
