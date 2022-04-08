using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartUI : MonoBehaviour
{
    // Start is called before the first frame update

    public List<UIBattleNode> m_battleNodeList;
    public GameObject m_nodesContainerRef;

    void Start()
    {
        UIBattleNode[] battleNodes = m_nodesContainerRef.GetComponentsInChildren<UIBattleNode>();
        for (int i = 0; i < battleNodes.Length; i++)
        {
            m_battleNodeList.Add(battleNodes[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
