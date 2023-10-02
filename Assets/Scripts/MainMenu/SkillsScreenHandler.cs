using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsScreenHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public GameObject m_skillBarPrefab;
    List<CharacterSkillBar> m_skillBars;

    // Start is called before the first frame update
    void Start()
    {
        m_skillBars = new List<CharacterSkillBar>();
        m_gameHandlerRef = FindObjectOfType<GameHandler>();

        Vector3 skillBarsStartPoint = new Vector3(0f, 632f, 0f);
        for (int i = 0; i < m_gameHandlerRef.m_xCellTeam.m_playerXCell.m_statHandler.m_stats.Length; i++)
        {
            skillBarsStartPoint += new Vector3(0f, -140f, 0);
            CharacterSkillBar skillBar = Instantiate<GameObject>(m_skillBarPrefab,transform).GetComponent<CharacterSkillBar>();
            skillBar.gameObject.transform.localPosition = skillBarsStartPoint;
            skillBar.gameObject.transform.localScale *= 1.2f;
            skillBar.Init(m_gameHandlerRef.m_xCellTeam.m_playerXCell.m_statHandler.m_stats[i]);
            m_skillBars.Add(skillBar);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Back()
    {
        gameObject.SetActive(false);
    }
}
