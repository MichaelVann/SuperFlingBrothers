using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    BodyPartUI m_bodyPartUIRef;
    Town m_townRef;
    GameHandler m_gameHandlerRef;
    public bool m_overrun = false;
    public string m_name;
    public TextMeshPro m_text;
    public List<MapNodeConnection> m_connectionList;
    bool m_playersResidingTown;

    public void AddConnection(MapNodeConnection a_mapConnection) { m_connectionList.Add(a_mapConnection); }

    public Town GetTown() { return m_townRef; }

    public void SetTown(Town a_town)
    {
        m_townRef = a_town;
        m_overrun = m_townRef.m_overrun;
    }

    public void Awake()
    {
        m_bodyPartUIRef = FindObjectOfType<BodyPartUI>();
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_name = gameObject.name;
        m_text.text = m_name;
        //m_connectionList = new List<MapNodeConnection>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (m_townRef == m_gameHandlerRef.m_humanBody.m_playerResidingTown)
        {
            m_playersResidingTown = true;
        }
        if (m_playersResidingTown)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = m_overrun ? Color.green : Color.red;
        }
        //m_name = "Town";
        //m_text.text = m_name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Refresh()
    {
        GetComponent<SpriteRenderer>().color = m_overrun ? Color.green : Color.red;
        for (int i = 0; i < m_connectionList.Count; i++)
        {
            m_connectionList[i].Refresh();
        }
        for (int i = 0; i < m_connectionList.Count; i++)
        {
            m_connectionList[i].Refresh();
        }
    }

    private void OnMouseUpAsButton()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            m_overrun = !m_overrun;
            m_bodyPartUIRef.Refresh();
        }
    }
}
