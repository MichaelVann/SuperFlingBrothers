using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter2D(Collision2D a_collision)
    {
        if (a_collision.gameObject.GetComponent<Damageable>())
        {
            a_collision.gameObject.GetComponent<Damageable>().Damage(100000f);
        }
    }
}
