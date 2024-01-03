using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using static TutorialManager;

public class TutorialManager
{
    GameHandler m_gameHandlerRef;
    GameObject m_dialogBoxPrefab;
    GameObject m_tutorialTaskPrefab;
    GameObject m_dialogCanvasRef;
    GameObject m_tutorialTaskCanvasRef;
    UITutorialTask m_uiTutorialTask;

    internal enum eTutorial
    {
        FindSquadOverview,
        FindInventory,
        EquipEquipment,
        Count
    }
    internal eTutorial m_currentTutorial;

    internal enum eMessage
    {
        StartUp = 0,
        SquadOverview,
        Inventory,
        Count
    }
    internal bool[] m_messagesPlayed;

    internal bool IsTutorialCompleted()
    {
        bool retVal = (m_currentTutorial == eTutorial.Count);
        return retVal;
    }

    internal void AttemptToSpawnMessage(eMessage a_message)
    {
        if (!m_messagesPlayed[(int)a_message])
        {
            PlayMessage(a_message);
        }
    }

    internal void ProgressIfOnSpecificTutorial(eTutorial a_tutorial)
    {
        if (m_currentTutorial == a_tutorial)
        {
            ProgressTutorial();
        }
    }

    void SetUpMessages()
    {
        m_messagesPlayed = new bool[(int)eMessage.Count];
        for (int i = 0; i < m_messagesPlayed.Length; i++)
        {
            m_messagesPlayed[i] = false;
        }
    }

    void PlayMessage(eMessage a_messageIndex)
    {
        List<string> dialogStrings = new List<string>();
        DialogBox.OnCloseDelegate onCloseDelegate = null;
        switch (a_messageIndex)
        {
            case eMessage.StartUp:
                dialogStrings = new List<string>
                {
                    "Hello World! Welcome to <u><color=red>Probiotic!</color></u>",
                    "This is the early testing verion of the game, with many poorly explained elements, poorly balanced gameplay, and likely many bugs. Please enjoy what exists so far for as long as you like, and get in touch with any feedback you have.",
                    "I'll now kickstart you through a quick set of simple tasks to bring you up to speed on the game.",
                    "Good luck, and thank you for checking out the game. Much love."
                };
                onCloseDelegate = SpawnTutorial;
                break;
            case eMessage.SquadOverview:
                dialogStrings = new List<string>
                {
                    "This is the <color=red>Squad Overview</color> screen.",
                    "Here you can see the overall status of your squad, as well as access your equipment and assign it to the four available slots on your current character.",
                    "Below that at the bottom are some buttons that allow you to navigate to the <color=red>Attribute</color>, <color=red>Skills</color> and <color=red>Upgrades</color>' screens, as well as returning to the Main Menu."
                };
                break;
            case eMessage.Inventory:
                dialogStrings = new List<string>
                {
                    "This is the <color=red>Inventory Screen</color>.",
                    "Here you can view your available equipment and equip it to one of four available slots. Each bit of equipment provides either a passive or active ability, which can be used against the scourge."
                };
                break;
            case eMessage.Count:
                break;
            default:
                break;
        }
        m_messagesPlayed[(int)a_messageIndex] = true;
        CreateTutorialDialog(GameHandler.m_speakerCharacterName, dialogStrings, onCloseDelegate);
    }

    // Start is called before the first frame update
    internal TutorialManager(GameHandler a_gameHandlerRef, GameObject a_dialogBoxPrefab, GameObject a_tutorialTaskPrefab, GameObject a_dialogCanvasRef, GameObject a_tutorialTaskCanvasRef)
    {
        m_gameHandlerRef = a_gameHandlerRef;
        m_dialogBoxPrefab = a_dialogBoxPrefab;
        m_tutorialTaskPrefab = a_tutorialTaskPrefab;
        m_dialogCanvasRef = a_dialogCanvasRef;
        m_tutorialTaskCanvasRef = a_tutorialTaskCanvasRef;
        m_currentTutorial = eTutorial.FindSquadOverview;
        SetUpMessages();
    }

    internal void CleanUp()
    {
        if (m_uiTutorialTask != null)
        {
            GameObject.Destroy(m_uiTutorialTask.gameObject);
        }
    }

    internal void CreateTutorialDialog(string a_speakerName, List<string> a_dialogs, DialogBox.OnCloseDelegate a_onCloseDelegate = null)
    {
        DialogBox dialogBox = GameObject.Instantiate(m_dialogBoxPrefab, m_dialogCanvasRef.transform).GetComponentInChildren<DialogBox>();
        dialogBox.Init(a_speakerName, a_onCloseDelegate);
        dialogBox.AddDialogs(a_dialogs);
    }

    internal void CreateTutorialTask(string a_taskString, bool a_topSide)
    {
        if (m_uiTutorialTask != null)
        {
            GameObject.Destroy(m_uiTutorialTask.gameObject);
        }
        m_uiTutorialTask = GameObject.Instantiate(m_tutorialTaskPrefab, m_tutorialTaskCanvasRef.transform).GetComponent<UITutorialTask>();
        m_uiTutorialTask.Init(a_taskString, a_topSide, SpawnTutorial);
    }

    internal void SpawnTutorial()
    {
        switch (m_currentTutorial)
        {
            case eTutorial.FindSquadOverview:
                CreateTutorialTask("Find the squad overview screen.", true);
                break;
            case eTutorial.FindInventory:
                CreateTutorialTask("Navigate to the <color=red>Equipment</color> screen.", true);
                break;
            case eTutorial.EquipEquipment:
                CreateTutorialTask("Equip a piece of equipment.", false);
                break;
            case eTutorial.Count:
                if (m_uiTutorialTask != null)
                {
                    m_uiTutorialTask.SetCompleted(true);
                }
                break;
            default:
                break;
        }
    }

    internal void ProgressTutorial()
    {
        if (m_currentTutorial != eTutorial.Count)
        {
            m_currentTutorial++;
        }
        if (m_uiTutorialTask != null)
        {
            m_uiTutorialTask.SetCompleted(true);
        }
    }
}
