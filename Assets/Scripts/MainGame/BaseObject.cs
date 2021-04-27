using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour
{
    public Rigidbody2D m_rigidBody;
    protected SpriteRenderer m_spriteRenderer;
    public virtual void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void Update()
    {
        
    }
}
