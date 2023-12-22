using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlingDirectionIndicator : MonoBehaviour
{
    [SerializeField] GameObject m_arrowRef;
    Vector3 m_originalArrowPos;
    float m_lerpTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        m_originalArrowPos = m_arrowRef.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        m_lerpTimer += Time.unscaledDeltaTime*4f;
        float sin = Mathf.Sin(m_lerpTimer) +1;
        m_arrowRef.transform.localPosition = m_originalArrowPos + m_originalArrowPos * (0.2f*sin);
    }
}
