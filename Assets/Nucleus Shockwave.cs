using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    float m_expandRate = 5f;
    [SerializeField] SpriteRenderer m_spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float factor = 1f + (Time.unscaledDeltaTime * m_expandRate);
        transform.localScale *= factor;
        m_spriteRenderer.color = new Color(m_spriteRenderer.color.r, m_spriteRenderer.color.g, m_spriteRenderer.color.b, m_spriteRenderer.color.a /factor);
        if (transform.localScale.z > 10f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D a_other)
    {
        Damageable oppDamageable = a_other.gameObject.GetComponent<Damageable>();
        if (oppDamageable != null)
        {
            oppDamageable.Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D a_other)
    {
        Damageable oppDamageable = a_other.gameObject.GetComponent<Damageable>();
        if (oppDamageable != null)
        {
            oppDamageable.Die();
        }
    }
}
