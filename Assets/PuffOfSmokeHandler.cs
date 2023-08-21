using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuffOfSmokeHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroySmoke()
    {
        GameObject.Destroy(this.gameObject);
    }
}
