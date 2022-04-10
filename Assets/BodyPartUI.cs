using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BodyPartUI : MonoBehaviour
{
    // Start is called before the first frame update

    public List<UIBattleNode> m_UIBattleNodeList;
    public GameObject m_nodesContainerRef;
    public GameObject m_lockImageRef;

    void Start()
    {
        //UIBattleNode[] battleNodes = m_nodesContainerRef.GetComponentsInChildren<UIBattleNode>();
        //for (int i = 0; i < battleNodes.Length; i++)
        //{
        //    m_UIBattleNodeList.Add(battleNodes[i]);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUp(BodyPart a_part)
    {
        m_lockImageRef.SetActive(!a_part.m_unlocked);
        for (int i = 0; i < m_UIBattleNodeList.Count; i++)
        {
            m_UIBattleNodeList[i].gameObject.SetActive(a_part.m_unlocked);
        }
        GetComponent<Button>().interactable = a_part.m_unlocked;
    }
}
