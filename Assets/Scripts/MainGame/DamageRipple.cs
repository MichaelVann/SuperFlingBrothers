using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageRipple : MonoBehaviour
{
    Damageable m_damageableRef;

    SpriteRenderer m_spriteRenderer;
    public Sprite[] m_sprites;

    float m_animationTimer = 0f;
    float m_frameLength = 0.07f;
    int m_spriteIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_spriteRenderer.sprite = m_sprites[0];
        m_damageableRef = GetComponentInParent<Damageable>();
    }

    // Update is called once per frame
    void Update()
    {
        m_animationTimer += Time.deltaTime;
        if (m_animationTimer >= m_frameLength)
        {
            m_animationTimer = 0f;
            m_spriteIndex = (m_spriteIndex + 1) % m_sprites.Length;
            m_spriteRenderer.sprite = m_sprites[m_spriteIndex];
        }
        float healthPerc = m_damageableRef.GetHealthPercentage();
        float colorFactor = 1f - 0.4f * m_damageableRef.GetHealthPercentage();

        if (healthPerc >= 1f)
        {
            colorFactor = 0f;
        }

        m_spriteRenderer.color = new Color(colorFactor, colorFactor, colorFactor, colorFactor);
    }
}
