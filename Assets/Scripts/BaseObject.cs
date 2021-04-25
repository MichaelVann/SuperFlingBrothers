using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObject : MonoBehaviour
{
    Rigidbody2D m_rigidBodyRef;

    public virtual void Awake()
    {
        m_rigidBodyRef = GetComponent<Rigidbody2D>();
    }

    public virtual void Update()
    {
        
    }
}
