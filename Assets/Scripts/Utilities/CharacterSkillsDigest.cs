using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterSkillsDigest : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public GameObject m_skillBarPrefab;
    List<CharacterSkillBar> m_skillBars;
    [SerializeField] private GameObject m_skillBarLayoutGroup;
    [SerializeField] private TextMeshProUGUI m_characterNameText;

    // Start is called before the first frame update
    void Start()
    {
        m_skillBars = new List<CharacterSkillBar>();
        m_gameHandlerRef = FindObjectOfType<GameHandler>();

        //Vector3 skillBarsStartPoint = new Vector3(0f, 432f, 0f);
        for (int i = 0; i < m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_statHandler.m_stats.Length; i++)
        {
            //skillBarsStartPoint += new Vector3(0f, -140f, 0);
            CharacterSkillBar skillBar = Instantiate<GameObject>(m_skillBarPrefab, m_skillBarLayoutGroup.transform).GetComponent<CharacterSkillBar>();
            //skillBar.gameObject.transform.localPosition = skillBarsStartPoint;
            //skillBar.gameObject.transform.localScale *= 1.2f;
            skillBar.Init(m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_statHandler.m_stats[i]);
            m_skillBars.Add(skillBar);
        }
        m_characterNameText.text = m_gameHandlerRef.m_xCellSquad.m_playerXCell.m_name.ToString();
    }

    internal bool SkillBarsFinishedAnimating()
    {
        bool retVal = true;

        for (int i = 0; i < m_skillBars.Count; i++)
        {
            retVal &= !m_skillBars[i].m_animating;
        }

        return retVal;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
