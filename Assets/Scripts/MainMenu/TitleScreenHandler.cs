using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;

    public GameObject m_equipmentNotifierRef;
    public Text m_equipmentNotifierTextRef;
    public Text m_versionNumberText;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_versionNumberText.text = "Version " + GameHandler._VERSION_NUMBER_STRING;
    }

    // Update is called once per frame
    void Update()
    {

        int combinedNewStatsAndEquipment = m_gameHandlerRef.m_xCellSquad.m_statHandler.m_RPGLevel.m_allocationPoints + m_gameHandlerRef.m_equipmentCollectedLastGame;

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
}
