using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadChecker : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public GameObject m_gameHandlerTemplate;
    // Start is called before the first frame update
    void Awake()
    {
        if (!FindObjectOfType<GameHandler>())
        {
            m_gameHandlerRef = Instantiate(m_gameHandlerTemplate).GetComponent<GameHandler>();
            //m_gameHandlerRef.SetGameMode(GameHandler.eGameMode.Health);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
