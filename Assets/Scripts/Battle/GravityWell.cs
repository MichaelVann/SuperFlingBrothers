using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class GravityWell : MonoBehaviour
{
    const float m_rotateSpeed = 200f;
    float m_strengthScale;
    float m_maxStrength;
    float m_forceAngle;

    const float m_megaScale = 4.5f;

    List<Rigidbody2D> m_affectedRigidBodies;

    internal enum eGravityWellType
    {
        Suck,
        Spin,
        MegaWhirlpool,
        Count
    }
    eGravityWellType m_type;

    void Awake()
    {
        m_affectedRigidBodies = new List<Rigidbody2D>();
        Init(eGravityWellType.Spin);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    internal void Init(eGravityWellType a_type)
    {
        m_type = a_type;
        switch (m_type)
        {
            case eGravityWellType.Suck:
                m_strengthScale = 150f;
                m_maxStrength = 300f;
                m_forceAngle = 0f;
                break;
            case eGravityWellType.Spin:
                m_forceAngle = 17f;
                m_strengthScale = 150f;
                m_maxStrength = 300f;
                break;
            case eGravityWellType.MegaWhirlpool:
                m_forceAngle = 45f;
                m_strengthScale = 500f;
                m_maxStrength = 600f;
                
                transform.localScale = new Vector3(m_megaScale, m_megaScale, 1f);
                break;
            case eGravityWellType.Count:
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < m_affectedRigidBodies.Count; i++)
        {
            Rigidbody2D otherRigidbody = m_affectedRigidBodies[i];

            Vector3 forceVector = (transform.position - otherRigidbody.transform.position).normalized;
            float strength = 1f / Mathf.Pow((transform.position - otherRigidbody.transform.position).magnitude, 2f);
            forceVector = forceVector.RotateVector3In2D(m_forceAngle);

            //forceVector = Quaternion.AngleAxis(90f, Vector3.forward) * forceVector;
            strength *= m_strengthScale;
            strength = Mathf.Clamp(strength, 0f, m_maxStrength);
            //float velocityInfluence = 0.5f;
            //strength *= velocityInfluence + ((1f- velocityInfluence) * otherRigidbody.velocity.magnitude);
            otherRigidbody.AddForce(forceVector * strength * Time.deltaTime);
        }

        transform.Rotate(new Vector3(0f,0f, -m_rotateSpeed * Time.deltaTime));

    }

    private void OnTriggerEnter2D(Collider2D a_otherCollider)
    {
        Rigidbody2D otherRigidbody = a_otherCollider.GetComponent<Rigidbody2D>();
        if (otherRigidbody != null)
        {
            m_affectedRigidBodies.Add(otherRigidbody);
        }
    }

    private void OnTriggerExit2D(Collider2D a_otherCollider)
    {
        Rigidbody2D otherRigidbody = a_otherCollider.GetComponent<Rigidbody2D>();
        if (otherRigidbody != null)
        {
            m_affectedRigidBodies.Remove(otherRigidbody);
        }
    }
}
