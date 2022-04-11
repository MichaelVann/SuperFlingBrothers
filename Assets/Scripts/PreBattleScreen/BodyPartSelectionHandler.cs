using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartSelectionHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    Camera m_cameraRef;
    public GameObject[] m_bodyPartUIObjectRefs;
    public GameObject m_bodyContainerRef;
    public GameObject m_partInfoPanel;
    public NodeInfoPanelHandler m_nodeInfoPanel;

    List<BodyPartUI> m_bodyPartUIList;
    public List<GameObject> m_nodeContainers;
    public List<GameObject> m_lineContainers;
    public GameObject m_lineContainer;
    public GameObject m_nodeContainer;

    int m_selectedBodyPartIndex = 0;
    public BodyPart m_selectedBodyPart;
    int m_selectedBattleNodeId = 0;
    public BattleNode m_selectedBattleNode;
    public UIBattleNode m_selectedUIBattleNode;

    //Zooming
    Vector3 m_humanBodyStartPos;
    Vector3 m_zoomPartVisibilityOffset;
    bool m_initialZoomed = false;
    bool m_zoomingIn = false;
    bool m_zooming = false;
    float m_startingZoom = 1f;
    float m_currentZoom = 1f;
    float m_targetZoom = 0f;
    float m_initialZoom = 3.5f;
    float m_maxZoom = 9.5f;
    Vector3 m_startingZoomLocation;
    Vector3 m_currentZoomLocation;
    Vector3 m_zoomTargetLocation;
    float m_zoomProgress = 0f;
    float m_zoomTime = 0.5f;

    bool m_wasPinchingLastFrame = false;
    float m_lastPinchDistance;

    bool m_wasPanning = false;
    Vector3 m_panLastRawLocation;


    public void SetNodeInfoPanelOpenState(bool a_open)
    {
        m_nodeInfoPanel.gameObject.SetActive(a_open);
        m_partInfoPanel.SetActive(!a_open);
    }

    public void Awake()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_cameraRef = FindObjectOfType<Camera>();
        m_humanBodyStartPos = m_bodyContainerRef.transform.localPosition;
        m_zoomPartVisibilityOffset = new Vector3(0f, 30f, 0f);

        if (!m_gameHandlerRef.m_humanBody.m_bodyInitialised)
        {
            SetUpPartNodes();
        }
        for (int i = 0; i < m_bodyPartUIList.Count; i++)
        {
            m_bodyPartUIList[i].SetUp(m_gameHandlerRef.m_humanBody.m_bodyPartList[i]);
        }
        ToggleNodeAndLineVisibility(false);
    }

    // Start is called before the first frame update
    void Start()
    {


    }

    void SetUpPartNodes()
    {
        m_bodyPartUIList = new List<BodyPartUI>();

        for (int i = 0; i < m_bodyPartUIObjectRefs.Length; i++)
        {
            m_bodyPartUIList.Add(m_bodyPartUIObjectRefs[i].GetComponent<BodyPartUI>());
        }

        m_gameHandlerRef.m_humanBody.SetUpBodyPartNodes(m_bodyPartUIList);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInitialZoom();
        if (m_initialZoomed)
        {
            PinchZoom();
            PanCamera();
        }
    }

    void ToggleNodeAndLineVisibility(bool a_visible)
    {
        m_lineContainer.SetActive(a_visible);
        m_nodeContainer.SetActive(a_visible);

    }

    void ApplyZoomAndPan()
    {
        m_bodyContainerRef.transform.localPosition = m_currentZoomLocation * m_currentZoom;
        m_bodyContainerRef.transform.localScale = new Vector3(m_currentZoom, m_currentZoom, 1f);
    }

    void UpdateInitialZoom()
    {
        if (m_zooming)
        {
            m_zoomProgress += Time.deltaTime / m_zoomTime;
            m_zoomProgress = Mathf.Clamp(m_zoomProgress, 0f, 1f);
            m_currentZoom = m_startingZoom + m_zoomProgress * (m_targetZoom - m_startingZoom);

            float lerp = m_zoomProgress;// * (1f + (m_targetZoom - m_currentZoom));

            m_currentZoomLocation = new Vector3(Mathf.Lerp(m_startingZoomLocation.x, m_zoomTargetLocation.x, lerp), Mathf.Lerp(m_startingZoomLocation.y, m_zoomTargetLocation.y, lerp), 0f);

            ApplyZoomAndPan();

            if (m_zoomProgress >= 1f)
            {
                m_zooming = false;
                m_zoomProgress = 0f;
                m_initialZoomed = m_zoomingIn ? true : false;
                if (m_initialZoomed)
                {
                    ToggleNodeAndLineVisibility(true);
                }
            }
        }
    }

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
                m_currentZoom = Mathf.Clamp(m_currentZoom * deltaPinchDistance, m_initialZoom, m_maxZoom);
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
            Camera camera = FindObjectOfType<Camera>();
            Vector3 localPanPosRaw = camera.ScreenToWorldPoint(Input.GetTouch(0).position);
            Vector3 localPanPos = m_bodyContainerRef.transform.InverseTransformPoint(camera.ScreenToWorldPoint(Input.GetTouch(0).position));
            localPanPos.z = 0f;
            localPanPosRaw.z = 0f;

            if (!m_wasPanning)
            {
                m_panLastRawLocation = localPanPosRaw;
            }
            else
            {
                m_currentZoomLocation += localPanPos - m_bodyContainerRef.transform.InverseTransformPoint(m_panLastRawLocation);
                m_currentZoomLocation.z = 0f;
                m_panLastRawLocation = localPanPosRaw;

            }
            m_wasPanning = true;
            ApplyZoomAndPan();
        }
        else
        {
            m_wasPanning = false;
        }
    }

    public void SelectPart(int a_index)
    {
        if (!m_initialZoomed)
        {
            m_selectedBodyPartIndex = a_index;
            m_selectedBodyPart = m_gameHandlerRef.m_humanBody.m_bodyPartList[a_index];
            InitialZoomToPart(a_index);
        }
    }

    public void SelectNode(int a_id, UIBattleNode a_selectedNode)
    {
        DeselectUINode();
        m_selectedBattleNodeId = a_id;
        m_selectedBattleNode = m_gameHandlerRef.m_humanBody.m_bodyPartList[m_selectedBodyPartIndex].m_nodes[m_selectedBattleNodeId];
        m_selectedUIBattleNode = a_selectedNode;
        SetNodeInfoPanelOpenState(true);
        m_nodeInfoPanel.SetUp(m_selectedBattleNode);
    }

    public void DeselectUINode()
    {
        if (m_selectedUIBattleNode != null)
        {
            m_selectedUIBattleNode.SetSelectionRingActive(false);
            m_selectedUIBattleNode = null;
        }
    }

    public void InitialZoomToPart(int a_index)
    {
        m_zooming = true;
        m_startingZoom = m_currentZoom;// m_bodyContainerRef.transform.localScale.x;
        m_targetZoom = m_initialZoom;
        m_zoomTargetLocation = m_humanBodyStartPos - m_bodyPartUIObjectRefs[a_index].transform.localPosition + m_zoomPartVisibilityOffset;
        m_startingZoomLocation = m_currentZoomLocation;
        m_partInfoPanel.SetActive(true);
        m_zoomProgress = 0f;
        m_zoomingIn = true;

    }

    public void UnInitialZoom()
    {
        m_zooming = true;
        m_startingZoom = m_currentZoom;// m_bodyContainerRef.transform.localScale.x;
        m_targetZoom = 1f;
        m_zoomTargetLocation = m_humanBodyStartPos;
        m_startingZoomLocation = m_currentZoomLocation;
        m_partInfoPanel.SetActive(false);
        m_zoomProgress = 0f;
        m_zoomingIn = false;
        ToggleNodeAndLineVisibility(false);
    }

}
