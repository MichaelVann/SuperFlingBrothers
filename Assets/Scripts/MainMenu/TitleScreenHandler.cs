using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public GameObject m_statAllocationNotifierRef;
    public Text m_statAllocationNotifierTextRef;
    public GameObject m_equipmentNotifierRef;
    public Text m_equipmentNotifierTextRef;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        m_statAllocationNotifierRef.SetActive(m_gameHandlerRef.m_playerStatHandler.m_allocationPoints > 0);
        if (m_statAllocationNotifierRef.activeSelf)
        {
            if (m_gameHandlerRef.m_playerStatHandler.m_allocationPoints > 9)
            {
                m_statAllocationNotifierTextRef.text = "9+";
            }
            else
            {
                m_statAllocationNotifierTextRef.text = "" + m_gameHandlerRef.m_playerStatHandler.m_allocationPoints;
            }
        }
        m_equipmentNotifierRef.SetActive(m_gameHandlerRef.m_equipmentCollectedLastGame > 0);

        if (m_equipmentNotifierRef.activeSelf)
        {
            if (m_gameHandlerRef.m_equipmentCollectedLastGame > 9)
            {
                m_equipmentNotifierTextRef.text = "9+";
            }
            else
            {
                m_equipmentNotifierTextRef.text = "" + m_gameHandlerRef.m_equipmentCollectedLastGame;
            }
        }
    }
}
