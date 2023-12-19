using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;

    //Camera
    Camera m_cameraRef;
    bool m_wasPinchingLastFrame = false;
    float m_lastPinchDistance;
    bool m_wasPanning = false;
    Vector3 m_lastPanPos;
    static float m_currentZoom = 5f;
    static float m_minZoom = 1f;
    static float m_maxZoom = 5f;
    static float m_startingCameraSize = 5f;
    float m_startingCameraZPos;
    static Vector3 m_currentPan = new Vector3(0f,0f,-110f);
    static Vector3 m_cameraMinBounds;
    static Vector3 m_cameraMaxBounds;
    static Vector3 m_cameraCornerBounds;

    [SerializeField] RectTransform m_viewFrameRect;

    [SerializeField] GameObject m_bodyMapPrefab;

    public HumanBodyUI m_viewedBodyPartUI;

    public GameObject m_partInfoPanel;
    public NodeInfoPanelHandler m_nodeInfoPanel;

    // UI Popup
    public GameObject m_lostNotificationRef;
    [SerializeField] GameObject m_cameraMaxBoundsRef;
    [SerializeField] GameObject m_cameraMinBoundsRef;
    [SerializeField] GameObject m_cameraCornerBoundsRef;

    MapNode m_residingMapNode;

    //Selected BattleNode
    int m_selectedBattleNodeId = -1;
    public BattleNode m_selectedBattleNode;
    UIBattleNode m_selectedUIBattleNode;
    [SerializeField] GameObject m_testMarkerPrefab;

    //Camera


    // Start is called before the first frame update
    void Awake()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_cameraRef = FindObjectOfType<Camera>();
        //m_startingCameraSize = m_cameraRef.orthographicSize;
        m_startingCameraZPos = m_cameraRef.transform.position.z;
        //m_mapNodeList = FindObjectsOfType<MapNode>();
        //m_currentZoomLocation = m_cameraRef.transform.position;
        m_viewedBodyPartUI = Instantiate(m_bodyMapPrefab, transform).GetComponent<HumanBodyUI>();
        m_viewedBodyPartUI.Initialise();
        //ApplyZoomAndPan();
    }

    public void Start()
    {
        m_residingMapNode = m_viewedBodyPartUI.GetResidingMapNode();
        m_currentPan = m_residingMapNode.transform.position;
        m_currentPan.z = m_startingCameraZPos;
        m_cameraMinBounds = m_cameraMinBoundsRef.transform.position;
        m_cameraMaxBounds = m_cameraMaxBoundsRef.transform.position;
        m_cameraCornerBounds = m_cameraCornerBoundsRef.transform.position;
        ApplyZoomAndPan();
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
        Vector3 debugPan = new Vector3();
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            debugPan += new Vector3(-1*Time.deltaTime,0f,0f);
            ApplyZoomAndPan();
        }
        if(Input.GetKey(KeyCode.RightArrow))
        {
            debugPan += new Vector3(Time.deltaTime, 0f, 0f);
            ApplyZoomAndPan();
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            debugPan += new Vector3(0f, Time.deltaTime, 0f);
            ApplyZoomAndPan();
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            debugPan += new Vector3(0f,-Time.deltaTime, 0f);
            ApplyZoomAndPan();
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            Vector3 screenToWorldPos = m_cameraRef.ScreenToWorldPoint(new Vector3(Screen.width,0f,0f));
            //Vector3 screenToWorldPos = m_cameraRef.ScreenToWorldPoint(new Vector3(m_cameraRef.scaledPixelWidth / 2f, m_cameraRef.scaledPixelHeight / 2f, 0f));
            //screenToWorldPos += m_cameraRef.transform.position;
            Instantiate(m_testMarkerPrefab, screenToWorldPos, Quaternion.identity);
        }

        debugPan *= (5f / m_currentZoom);
        m_currentPan += debugPan;
    }

    public void SelectNode(UIBattleNode a_selectedNode)
    {
        //if (!m_initialZoomed)
        //{
        //    return;
        //}
        DeselectUINode();
        //m_selectedBattleNodeId = a_id;
        //m_selectedBodyPartIndex = a_bodypartID;
        //Debug.Log(m_selectedBodyPartIndex + "," + m_selectedBattleNodeId);
        m_selectedBattleNode = a_selectedNode.m_battleNodeRef;
        m_selectedUIBattleNode = a_selectedNode;
        m_gameHandlerRef.SetSelectedBattle(a_selectedNode.m_battleNodeRef);
        m_selectedUIBattleNode.SetSelectionRingActive(true);
        SetNodeInfoPanelOpenState(true, m_selectedBattleNode);
    }


    public void DeselectUINode()
    {
        if (m_selectedUIBattleNode != null)
        {
            m_selectedUIBattleNode.SetSelectionRingActive(false);
            m_selectedUIBattleNode = null;
            SetNodeInfoPanelOpenState(false, m_selectedBattleNode);
        }
    }

    public void SetNodeInfoPanelOpenState(bool a_open, BattleNode a_battleNode)
    {
        m_nodeInfoPanel.gameObject.SetActive(a_open);
        m_nodeInfoPanel.SetUp(a_battleNode);
        m_partInfoPanel.SetActive(!a_open);

    }

    void ClampCameraPan()
    {
        float boundsZoomOffsetX = (m_cameraRef.orthographicSize*m_cameraRef.aspect);
        float boundsZoomOffsetY = (m_cameraRef.orthographicSize * (5f-1.62f)/5f);

        Vector2 minBounds = m_cameraMinBounds;
        Vector2 maxBounds = m_cameraMaxBounds;
        Vector3 bottomLeftWorldPoint = m_cameraRef.ScreenToWorldPoint(new Vector3(0f, m_viewFrameRect.offsetMin.y, 0f));
        float yCornerOffset = bottomLeftWorldPoint.y - m_cameraCornerBounds.y;
        float xCornerOffset = bottomLeftWorldPoint.x - m_cameraCornerBounds.x;
        if (bottomLeftWorldPoint.y < m_cameraCornerBounds.y && bottomLeftWorldPoint.x < m_cameraCornerBounds.x)
        {
            if (xCornerOffset < yCornerOffset)
            {
                minBounds.y = m_cameraCornerBounds.y;
            }
            else
            {
                minBounds.x = m_cameraCornerBounds.x;
            }
        }
        //if (bottomLeftWorldPoint.x < m_cameraCornerBounds.x)
        //{
        //    minBounds.y = m_cameraCornerBounds.y;
        //}


        m_currentPan.x = Mathf.Clamp(m_currentPan.x, minBounds.x + boundsZoomOffsetX, maxBounds.x - boundsZoomOffsetX);
        m_currentPan.y = Mathf.Clamp(m_currentPan.y, minBounds.y + boundsZoomOffsetY, maxBounds.y - boundsZoomOffsetY);
    }

    void ApplyZoomAndPan()
    {
        m_cameraRef.transform.position = m_currentPan;// * m_currentZoom;

        m_currentZoom = Mathf.Clamp(m_currentZoom, m_minZoom, m_maxZoom);
        m_cameraRef.orthographicSize = m_startingCameraSize/m_currentZoom;// new Vector3(m_currentZoom, m_currentZoom, 1f);
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
                m_currentZoom = m_currentZoom * deltaPinchDistance;
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
                m_currentPan -= m_deltaPos;
                m_currentPan.z = m_startingCameraZPos;
            }
            m_wasPanning = true;
            ApplyZoomAndPan();
            m_lastPanPos = m_cameraRef.ScreenToWorldPoint(Input.GetTouch(0).position);
        }
        else
        {
            m_wasPanning = false;
        }
        ClampCameraPan();
    }

    public void DismissNotification()
    {
        // button closes notif - setactive(false);
        m_lostNotificationRef.SetActive(false);

    }
}
