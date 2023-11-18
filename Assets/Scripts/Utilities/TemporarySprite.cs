using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporarySprite : MonoBehaviour
{
    vTimer m_lifeTimer;
    public SpriteRenderer m_spriteRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    public void Init(float a_lifeTime, Sprite a_sprite)
    {
        m_lifeTimer = new vTimer(a_lifeTime);
        m_spriteRenderer.sprite = a_sprite;
        if (transform.parent != null)
        {
            Vector3 parentScale = transform.parent.localScale;
            Vector3 scale = transform.localScale;
            transform.localScale = new Vector3(scale.x/parentScale.x, scale.y/parentScale.y, scale.z/parentScale.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_lifeTimer != null)
        {
            if (m_lifeTimer.Update())
            {
                Destroy(gameObject);
            }
        }
    }
}
