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

    Color[] m_healthColours;

    // Start is called before the first frame update
    void Start()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_spriteRenderer.sprite = m_sprites[0];
        m_damageableRef = GetComponentInParent<Damageable>();
        m_healthColours = new Color[] { Color.red, Color.yellow, Color.green};

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
        float colorFactor = 1f - healthPerc;//1f - 0.4f * m_damageableRef.GetHealthPercentage();

        if (healthPerc >= 1f)
        {
            colorFactor = 0f;
        }

        float redFactor = 0f;

        if (healthPerc <= 0.25f)
        {
            redFactor = 1f;
        }
        else
        {
            redFactor = Mathf.Clamp(1f - (4f*(healthPerc -0.25f)),0f,1f);
        }

        float greenFactor = 0f;

        if (healthPerc > 0.5f)
        {
            greenFactor = Mathf.Clamp(1f - (4f * (healthPerc - 0.5f)), 0f, 1f);
        }
        else if (healthPerc <= 0.25f)
        {
            greenFactor = Mathf.Clamp(4f * (healthPerc), 0f, 1f);
        }
        else
        {
            greenFactor = 1f;// - ((healthPerc-0.25f)/0.5f);// healthPerc / 0.75f;
        }

        float blueFactor = 0f;

        if (healthPerc >= 0.5f)
        {
            blueFactor = Mathf.Clamp(1f - (4f * (healthPerc - 0.75f)), 0f, 1f);
        }
        else
        {
            blueFactor = Mathf.Clamp(1f + (4f * (healthPerc - 0.75f)), 0f, 1f);
        }


        m_spriteRenderer.color = new Color(redFactor, greenFactor, blueFactor, colorFactor);
    }
}
