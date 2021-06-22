using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlingReadynessIndicator : MonoBehaviour
{
    Enemy m_enemyRef;

    SpriteRenderer m_spriteRenderer;
    public Sprite[] m_sprites;

    float m_animationTimer = 0f;
    float m_frameLength = 0.07f;
    int m_spriteIndex = 0;

    Color[] m_healthColours;

    // Start is called before the first frame update
    void Start()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_spriteRenderer.sprite = m_sprites[0];
        m_enemyRef = GetComponentInParent<Enemy>();
        m_healthColours = new Color[] { Color.red, Color.yellow, Color.green };

    }

    // Update is called once per frame
    void Update()
    {
        float flingPercentage = m_enemyRef.m_flingTimer / m_enemyRef.m_flingTimerMax;

        m_animationTimer += Time.deltaTime * 2f*(flingPercentage);
        if (m_animationTimer >= m_frameLength)
        {
            m_animationTimer = 0f;
            m_spriteIndex = (m_spriteIndex + 1) % m_sprites.Length;
            m_spriteRenderer.sprite = m_sprites[m_spriteIndex];
        }

        float colorFactor = Mathf.Pow(flingPercentage, 5f);

        m_spriteRenderer.color = new Color(colorFactor, colorFactor / 1.7f, 0f, 1f);


    }
}
