using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartInterface : MonoBehaviour
{
    public BodyPartSelectionHandler m_bodyPartSelectionHandler;
    public int m_bodyPartID;
    public GameObject m_leftFrontLineRef;
    public GameObject m_rightFrontLineRef;
    public GameObject m_leftFrontLineColliderRef;
    public GameObject m_rightFrontLineColliderRef;
    float m_rightFrontLineOriginalXScale = 1f;
    float m_rightFrontLineColliderOriginalOffsetX = 0f;
    float m_leftFrontLineOriginalXScale = 1f;
    float m_leftFrontLineColliderOriginalOffsetX = 0f;
    bool m_rightFrontLineSetup = false;
    bool m_leftFrontLineSetup = false;
    Collider2D m_colliderRef;
    const int m_maximumFrontLineChecks = 1000;
    int m_frontLineChecks = 0;

    const float m_deltaFrontLineScale = 0.001f;
    // Start is called before the first frame update
    void Start()
    {
        m_colliderRef = GetComponent<Collider2D>();
        m_bodyPartSelectionHandler = FindObjectOfType<BodyPartSelectionHandler>();
        m_rightFrontLineOriginalXScale = m_rightFrontLineRef.transform.localScale.x;
        m_rightFrontLineColliderOriginalOffsetX = m_rightFrontLineColliderRef.transform.localPosition.x - m_rightFrontLineRef.transform.localPosition.x;
        m_leftFrontLineOriginalXScale = m_leftFrontLineRef.transform.localScale.x;
        m_leftFrontLineColliderOriginalOffsetX = m_leftFrontLineColliderRef.transform.localPosition.x - m_leftFrontLineRef.transform.localPosition.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_rightFrontLineSetup)
        {
            m_rightFrontLineRef.transform.localScale = new Vector3(m_rightFrontLineRef.transform.localScale.x + m_deltaFrontLineScale, m_rightFrontLineRef.transform.localScale.y, 1f);
            float colliderPosX = m_rightFrontLineRef.transform.localPosition.x + m_rightFrontLineColliderOriginalOffsetX * m_rightFrontLineRef.transform.localScale.x / m_rightFrontLineOriginalXScale;
            m_rightFrontLineColliderRef.transform.localPosition = new Vector3(colliderPosX, m_rightFrontLineColliderRef.transform.localPosition.y, m_rightFrontLineColliderRef.transform.localPosition.z);
            if (Physics2D.IsTouching(m_rightFrontLineColliderRef.GetComponent<Collider2D>(), m_colliderRef))
            {
                m_rightFrontLineRef.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                m_rightFrontLineRef.GetComponent<SpriteRenderer>().color = Color.green;
                m_rightFrontLineSetup = true;
            }

            //RaycastHit2D hitPoint = Physics2D.Raycast(m_rightFrontLineRef.transform.position, Vector3.right);
            //m_rightFrontLineRef.transform.position = hitPoint.point;
            //m_rightFrontLineSetup = true;
        }

        if (!m_leftFrontLineSetup)
        {
            m_leftFrontLineRef.transform.localScale = new Vector3(m_leftFrontLineRef.transform.localScale.x + m_deltaFrontLineScale, m_leftFrontLineRef.transform.localScale.y, 1f);
            float colliderPosX = m_leftFrontLineRef.transform.localPosition.x + m_leftFrontLineColliderOriginalOffsetX * m_leftFrontLineRef.transform.localScale.x / m_leftFrontLineOriginalXScale;
            m_leftFrontLineColliderRef.transform.localPosition = new Vector3(colliderPosX, m_leftFrontLineColliderRef.transform.localPosition.y, m_leftFrontLineColliderRef.transform.localPosition.z);
            if (Physics2D.IsTouching(m_leftFrontLineColliderRef.GetComponent<Collider2D>(), m_colliderRef))
            {
                m_leftFrontLineRef.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                m_leftFrontLineRef.GetComponent<SpriteRenderer>().color = Color.green;
                m_leftFrontLineSetup = true;
            }
        }

        if (Input.GetKey(KeyCode.V))
        {
            //m_rightFrontLineRef.transform.localScale = new Vector3(m_rightFrontLineRef.transform.localScale.x + m_deltaFrontLineScale, m_rightFrontLineRef.transform.localScale.y, 1f);
            //float colliderPosX = m_rightFrontLineRef.transform.localPosition.x + m_rightFrontLineColliderOriginalOffsetX * m_rightFrontLineRef.transform.localScale.x/m_rightFrontLineOriginalXScale;
            //m_rightFrontLineColliderRef.transform.localPosition = new Vector3(colliderPosX, m_rightFrontLineColliderRef.transform.localPosition.y, m_rightFrontLineColliderRef.transform.localPosition.z);

            //m_rightFrontLineRef.transform.localScale = new Vector3(m_rightFrontLineRef.transform.localScale.x + m_deltaFrontLineScale, m_rightFrontLineRef.transform.localScale.y, 1f);
            //float colliderPosX = m_rightFrontLineRef.transform.localPosition.x + m_rightFrontLineColliderOriginalOffsetX * m_rightFrontLineRef.transform.localScale.x / m_rightFrontLineOriginalXScale;
            //m_rightFrontLineColliderRef.transform.localPosition = new Vector3(colliderPosX, m_rightFrontLineColliderRef.transform.localPosition.y, m_rightFrontLineColliderRef.transform.localPosition.z);
            //if (Physics2D.IsTouching(m_rightFrontLineColliderRef.GetComponent<Collider2D>(), m_colliderRef))
            //{
            //    m_rightFrontLineRef.GetComponent<SpriteRenderer>().color = Color.red;
            //}
            //else
            //{
            //    m_rightFrontLineRef.GetComponent<SpriteRenderer>().color = Color.green;
            //    m_rightFrontLineSetup = true;
            //}

            m_rightFrontLineRef.transform.localScale = new Vector3(m_rightFrontLineRef.transform.localScale.x + m_deltaFrontLineScale, m_rightFrontLineRef.transform.localScale.y, 1f);
            float colliderPosX = m_rightFrontLineRef.transform.localPosition.x + m_rightFrontLineColliderOriginalOffsetX * m_rightFrontLineRef.transform.localScale.x / m_rightFrontLineOriginalXScale;
            m_rightFrontLineColliderRef.transform.localPosition = new Vector3(colliderPosX, m_rightFrontLineColliderRef.transform.localPosition.y, m_rightFrontLineColliderRef.transform.localPosition.z);
            if (Physics2D.IsTouching(m_rightFrontLineColliderRef.GetComponent<Collider2D>(), m_colliderRef))
            {
                m_rightFrontLineRef.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                m_rightFrontLineRef.GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }

    void OnMouseUpAsButton()
    {
        m_bodyPartSelectionHandler.SelectPart(m_bodyPartID);
    }
}
