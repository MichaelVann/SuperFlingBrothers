using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public GameObject m_wallCrackTemplate;
    public enum WallDirection
    {
        Left,
        Right,
        Up,
        Down
    }
    public WallDirection m_direction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D a_collision)
    {
        if (a_collision.rigidbody != null && !a_collision.gameObject.GetComponent<Wall>())
        {
            float angle = 0f;
            switch (m_direction)
            {
                case WallDirection.Left:
                    angle = 0f;
                    break;
                case WallDirection.Right:
                    angle = 180f;
                    break;
                case WallDirection.Up:
                    angle = 270f;
                    break;
                case WallDirection.Down:
                    angle = 90f;
                    break;
                default:
                    break;
            }

            GameObject crack = Instantiate<GameObject>(m_wallCrackTemplate, a_collision.contacts[0].point, Quaternion.AngleAxis(angle, Vector3.forward));
            float scale = a_collision.relativeVelocity.magnitude;
            crack.transform.localScale *= scale/3f;
        }
    }
}
