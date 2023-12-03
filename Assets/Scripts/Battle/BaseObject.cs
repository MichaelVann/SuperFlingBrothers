using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour
{
    internal Rigidbody2D m_rigidBody;
    public SpriteRenderer m_spriteRenderer;

    public virtual void Awake()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        //m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void Update()
    {
        
    }
}