using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlingPieIndicator : MonoBehaviour
{
    public void Start()
    {
        
    }
    public void Update()
    {
        //GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", 90.0f);
    }

    public void SetPieFillAmount(float a_value)
    {
        GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", (1-a_value)*360f);
    }
}
