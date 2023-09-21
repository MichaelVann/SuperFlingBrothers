using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionRing : MonoBehaviour
{
    float m_rotationSpeed = 15f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //m_rotation += Time.deltaTime;
        transform.Rotate(Vector3.forward, -Time.deltaTime * m_rotationSpeed);
    }
}
