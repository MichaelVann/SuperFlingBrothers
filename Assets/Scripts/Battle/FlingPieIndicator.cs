using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlingPieIndicator : MonoBehaviour
{
    SpriteRenderer m_spriteRenderer;
    public void Awake()
    {
    }

    public void Update()
    {
        //GetComponent<SpriteRenderer>().material.SetFloat("_Arc1", 90.0f);
    }

    public void SetPieFillAmount(float a_value)
    {
        if (!m_spriteRenderer)
        {
            m_spriteRenderer = GetComponent<SpriteRenderer>();
        }
        m_spriteRenderer.material.SetFloat("_Arc1", (1f-a_value)*360f);
        m_spriteRenderer.color = VLib.PercentageToColor(1f-a_value);
    }
}
