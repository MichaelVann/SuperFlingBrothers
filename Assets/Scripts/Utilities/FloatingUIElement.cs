using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingUIElement : MonoBehaviour
{
    Vector3 m_startLocalPosition;
    [SerializeField] float m_riseHeight = 1f;
    [SerializeField] float m_floatSpeed = 1f;
    float m_elapsedTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        m_startLocalPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        m_elapsedTime += Time.deltaTime * m_floatSpeed * 2f;
        transform.localPosition = m_startLocalPosition + new Vector3(0f,Mathf.Sin(m_elapsedTime)*m_riseHeight * 10f,0f);
    }
}
