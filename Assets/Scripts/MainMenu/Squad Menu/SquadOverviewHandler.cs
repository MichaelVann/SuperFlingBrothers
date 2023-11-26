using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SquadOverviewHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public GameObject m_playerStatueRef;
    public TextMeshProUGUI m_xCellNameText;

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();

        m_xCellNameText.text = "ID: " + m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_name.ToUpper();
        m_playerStatueRef.GetComponent<Image>().color = m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_colorShade;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
