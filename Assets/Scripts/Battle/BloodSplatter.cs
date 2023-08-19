using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplatter : MonoBehaviour
{
    public Sprite[] m_decals;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = m_decals[VLib.vRandom(0, m_decals.Length-1)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
