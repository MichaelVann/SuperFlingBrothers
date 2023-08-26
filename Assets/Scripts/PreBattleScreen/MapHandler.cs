using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapHandler : MonoBehaviour
{
    //Zoom
    Camera m_cameraRef;

    Vector3 m_humanBodyStartPos;
    Vector3 m_zoomPartVisibilityOffset;
    static bool m_initialZoomed = false;
    static bool m_zoomingIn = false;
    static bool m_zooming = false;
    static float m_startingZoom = 5f;
    static float m_currentZoom = 1f;
    static float m_targetZoom = 0f;
    static float m_minZoom = 1f;
    static float m_maxZoom = 5f;
    float m_startingCameraSize;
    float m_startingCameraZPos;
    static Vector3 m_startingZoomLocation;
    static Vector3 m_currentZoomLocation;
    static Vector3 m_zoomTargetLocation;
    static float m_zoomProgress = 0f;
    static float m_zoomTime = 0.5f;

    bool m_wasPinchingLastFrame = false;
    float m_lastPinchDistance;

    bool m_wasPanning = false;
    Vector3 m_lastPanPos;

    // Start is called before the first frame update
    void Awake()
    {
        m_cameraRef = FindObjectOfType<Camera>();
        m_startingCameraSize = m_cameraRef.orthographicSize;
        m_startingCameraZPos = m_cameraRef.transform.position.z;
        m_currentZoomLocation = m_cameraRef.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateInitialZoom();
        //if (m_initialZoomed)
        //{
        //    PinchZoom();
        //    PanCamera();
        //}
        PinchZoom();
        PanCamera();
        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            m_currentZoom *= 1f + Time.deltaTime;
            ApplyZoomAndPan();
        }
        else if (Input.GetKey(KeyCode.KeypadMinus))
        {
            m_currentZoom *= 1f - Time.deltaTime;
            ApplyZoomAndPan();
        }
    }

    void ApplyZoomAndPan()
    {
        m_cameraRef.transform.position = m_currentZoomLocation;// * m_currentZoom;
        Debug.Log(m_currentZoomLocation);
        m_cameraRef.orthographicSize = m_startingCameraSize/m_currentZoom;// new Vector3(m_currentZoom, m_currentZoom, 1f);
    }

    //void UpdateInitialZoom()
    //{
    //    if (m_zooming)
    //    {
    //        m_zoomProgress += Time.deltaTime / m_zoomTime;
    //        m_zoomProgress = Mathf.Clamp(m_zoomProgress, 0f, 1f);
    //        m_currentZoom = m_startingZoom + m_zoomProgress * (m_targetZoom - m_startingZoom);

    //        float lerp = m_zoomProgress;// * (1f + (m_targetZoom - m_currentZoom));

    //        m_currentZoomLocation = new Vector3(Mathf.Lerp(m_startingZoomLocation.x, m_zoomTargetLocation.x, lerp), Mathf.Lerp(m_startingZoomLocation.y, m_zoomTargetLocation.y, lerp), 0f);

    //        ApplyZoomAndPan();

    //        if (m_zoomProgress >= 1f)
    //        {
    //            m_zooming = false;
    //            m_zoomProgress = 0f;
    //            m_initialZoomed = m_zoomingIn ? true : false;
    //            if (m_initialZoomed)
    //            {
    //                //ToggleNodeAndLineVisibility(true);
    //            }
    //        }
    //    }
    //}

    void PinchZoom()
    {
        if (Input.touchCount >= 2)
        {
            Vector2 touch0, touch1;
            float pinchDistance;
            touch0 = Input.GetTouch(0).position;
            touch1 = Input.GetTouch(1).position;
            pinchDistance = Vector2.Distance(touch0, touch1);

            if (m_wasPinchingLastFrame)
            {
                float deltaPinchDistance = pinchDistance / m_lastPinchDistance;
                m_currentZoom = Mathf.Clamp(m_currentZoom * deltaPinchDistance, m_minZoom, m_maxZoom);
                ApplyZoomAndPan();
            }

            m_lastPinchDistance = pinchDistance;
            m_wasPinchingLastFrame = true;
        }
        else
        {
            m_wasPinchingLastFrame = false;
        }
    }

    void PanCamera()
    {
        if (Input.touchCount == 1)
        {
            Vector3 panPos = m_cameraRef.ScreenToWorldPoint(Input.GetTouch(0).position);

            if (m_wasPanning)
            {
                Vector3 m_deltaPos = panPos - m_lastPanPos;
                Debug.Log(panPos);
                m_currentZoomLocation -= m_deltaPos;
                m_currentZoomLocation.z = m_startingCameraZPos;
            }
            m_wasPanning = true;
            ApplyZoomAndPan();
            m_lastPanPos = m_cameraRef.ScreenToWorldPoint(Input.GetTouch(0).position);
        }
        else
        {
            m_wasPanning = false;
        }
    }
}
