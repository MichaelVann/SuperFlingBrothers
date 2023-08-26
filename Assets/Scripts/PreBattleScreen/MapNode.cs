using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    public bool m_occupied = false;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().color = m_occupied ? Color.red : Color.green;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
