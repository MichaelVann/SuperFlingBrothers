using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityWell : MonoBehaviour
{
    static float m_gravityForce = 0.1f;
    List<Rigidbody2D> m_affectedRigidBodies;

    // Start is called before the first frame update
    void Start()
    {
        m_affectedRigidBodies = new List<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < m_affectedRigidBodies.Count; i++)
        {
            Rigidbody2D otherRigidbody = m_affectedRigidBodies[i];

            Vector3 forceVector = (transform.position - otherRigidbody.transform.position).normalized;
            //forceVector = Quaternion.AngleAxis(90f, Vector3.forward) * forceVector;
            float strength = 1f / (transform.position - otherRigidbody.transform.position).magnitude;
            strength *= 100f;
            strength = Mathf.Clamp(strength, 0f, 300f);
            float velocityInfluence = 0.5f;
            strength *= velocityInfluence + ((1f- velocityInfluence) * otherRigidbody.velocity.magnitude);
            otherRigidbody.AddForce(forceVector * strength * Time.deltaTime);
        }

        transform.Rotate(new Vector3(0f,0f, -200f * Time.deltaTime));

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
