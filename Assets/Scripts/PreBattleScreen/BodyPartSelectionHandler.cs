﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartSelectionHandler : MonoBehaviour
{
    Camera m_cameraRef;
    public GameObject[] m_bodyPartUIRefs;
    public GameObject m_bodyContainerRef;
    public GameObject m_partInfoPanel;

    int m_selectedPartIndex = 0;

    //Zooming
    Vector3 m_humanBodyStartPos;
    Vector3 m_zoomPartVisibilityOffset;
    bool m_zoomed = false;
    bool m_zooming = false;
    float m_startingZoom = 1f;
    float m_currentZoom = 1f;
    float m_targetZoom = 0f;
    float m_maxZoom = 2.5f;
    Vector3 m_startingZoomLocation;
    Vector3 m_currentZoomLocation;
    Vector3 m_zoomTargetLocation;
    float m_zoomProgress = 0f;
    float m_zoomTime = 0.5f;
    float m_zoomSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        m_cameraRef = FindObjectOfType<Camera>();
        m_humanBodyStartPos = m_bodyContainerRef.transform.localPosition;
        m_zoomPartVisibilityOffset = new Vector3(0f, 120f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateZoom();
    }

    void UpdateZoom()
    {
        if (m_zooming)
        {
            //Vector3 deltaPos = m_zoomTargetLocation - m_currentZoomLocation;
            //float deltaZoom = m_targetZoom - m_currentZoom;
            //float zoomIncrement = Time.deltaTime * m_zoomSpeed * (deltaZoom >= 0 ? 1f : -1f);
            //zoomIncrement = deltaZoom >= 0 ? Mathf.Clamp(zoomIncrement, 0f, deltaZoom) : Mathf.Clamp(zoomIncrement, deltaZoom, 0f);
            //m_currentZoom += zoomIncrement;

            //float zoomDeltaPerc = zoomIncrement / deltaZoom;
            //float translationProgression = Time.deltaTime;


            //m_currentZoomLocation = new Vector3(Mathf.Lerp(m_currentZoomLocation.x, m_zoomTargetLocation.x, zoomDeltaPerc), Mathf.Lerp(m_currentZoomLocation.y, m_zoomTargetLocation.y, zoomDeltaPerc), Mathf.Lerp(m_currentZoomLocation.z, m_zoomTargetLocation.z, zoomDeltaPerc));

            //m_bodyContainerRef.transform.localPosition = m_currentZoomLocation * m_currentZoom;
            //m_bodyContainerRef.transform.localScale = new Vector3(m_currentZoom, m_currentZoom, 1f);
            //if (zoomDeltaPerc >= 1f)
            //{
            //    m_zooming = false;
            //}

            m_zoomProgress += Time.deltaTime / m_zoomTime;
            m_zoomProgress = Mathf.Clamp(m_zoomProgress, 0f, 1f);
            m_currentZoom = m_startingZoom + m_zoomProgress * (m_targetZoom - m_startingZoom);

            float lerp = m_zoomProgress;// * (1f + (m_targetZoom - m_currentZoom));

            m_currentZoomLocation = new Vector3(Mathf.Lerp(m_startingZoomLocation.x, m_zoomTargetLocation.x, lerp), Mathf.Lerp(m_startingZoomLocation.y, m_zoomTargetLocation.y, lerp), Mathf.Lerp(m_startingZoomLocation.z, m_zoomTargetLocation.z, lerp));

            m_bodyContainerRef.transform.localPosition = m_currentZoomLocation * m_currentZoom;
            m_bodyContainerRef.transform.localScale = new Vector3(m_currentZoom, m_currentZoom, 1f);
            if (m_zoomProgress >= 1f)
            {
                m_zooming = false;
                m_zoomProgress = 0f;
            }
        }
    }

    public void SelectPart(int a_index)
    {
        m_selectedPartIndex = a_index;
        ZoomToPart(a_index);
    }

    public void ZoomToPart(int a_index)
    {
        m_zooming = true;
        m_startingZoom = m_currentZoom;// m_bodyContainerRef.transform.localScale.x;
        m_targetZoom = m_maxZoom;
        m_zoomTargetLocation = m_humanBodyStartPos - m_bodyPartUIRefs[a_index].transform.localPosition + m_zoomPartVisibilityOffset;
        //m_zoomTargetLocation = m_humanBodyStartPos - m_bodyPartUIRefs[a_index].transform.localPosition * m_maxZoom + m_zoomPartVisibilityOffset;
        m_startingZoomLocation = m_currentZoomLocation;
        m_partInfoPanel.SetActive(true);
        m_zoomProgress = 0f;
    }

    public void UnZoom()
    {
        m_zooming = true;
        m_startingZoom = m_currentZoom;// m_bodyContainerRef.transform.localScale.x;
        m_targetZoom = 1f;
        m_zoomTargetLocation = m_humanBodyStartPos;
        m_startingZoomLocation = m_currentZoomLocation;
        m_partInfoPanel.SetActive(false);
        m_zoomProgress = 0f;
    }
}
