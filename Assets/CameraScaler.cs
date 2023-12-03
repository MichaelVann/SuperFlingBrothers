using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScaler : MonoBehaviour
{
    float m_designedAspectRatio = 0.48f;
    Camera m_cameraRef;
    float m_defaultCameraSize;
    Vector2 m_screenSize;
    float m_aspectRatio;
    // Start is called before the first frame update
    void Start()
    {
        m_cameraRef = GetComponent<Camera>();
        m_defaultCameraSize = m_cameraRef.orthographicSize;
        m_screenSize = new Vector2(Screen.width, Screen.height);
        m_aspectRatio = m_screenSize.x / m_screenSize.y;
        print(m_aspectRatio);
        if (m_aspectRatio < m_designedAspectRatio)
        {
            m_cameraRef.orthographicSize = m_defaultCameraSize * m_designedAspectRatio/m_aspectRatio;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
