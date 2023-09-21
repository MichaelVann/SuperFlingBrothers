using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapHandler : MonoBehaviour
{
    //Zoom
    GameHandler m_gameHandlerRef;
    Camera m_cameraRef;

    Vector3 m_humanBodyStartPos;
    Vector3 m_zoomPartVisibilityOffset;
    static bool m_initialZoomed = false;
    static bool m_zoomingIn = false;
    static bool m_zooming = false;
    static float m_startingZoom = 5f;
    static float m_currentZoom = 5f;
    static float m_targetZoom = 0f;
    static float m_minZoom = 1f;
    static float m_maxZoom = 5f;
    static float m_startingCameraSize = 5f;
    float m_startingCameraZPos;
    static Vector3 m_startingZoomLocation;
    static Vector3 m_currentPan = new Vector3(0f,0f,-110f);
    static Vector3 m_zoomTargetLocation;
    static float m_zoomProgress = 0f;
    static float m_zoomTime = 0.5f;

    public GameObject m_bodyMapPrefab;
    public HumanBodyUI m_viewedBodyPartUI;

    public TextMeshProUGUI m_humanBodyNameText;

    public GameObject m_partInfoPanel;
    public NodeInfoPanelHandler m_nodeInfoPanel;

    // UI Popup
    public GameObject m_lostNotificationRef;

    bool m_wasPinchingLastFrame = false;
    float m_lastPinchDistance;

    bool m_wasPanning = false;
    Vector3 m_lastPanPos;

    MapNode m_residingMapNode;

    //Selected BattleNode
    int m_selectedBattleNodeId = -1;
    public BattleNode m_selectedBattleNode;
    UIBattleNode m_selectedUIBattleNode;
    public GameObject m_backdropRef;
    Vector3 m_backdropRefStartingSize;

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

        if (m_gameHandlerRef.m_humanBody.m_lost)
        {
            // UI Popup: lost notification
            m_lostNotificationRef.SetActive(true);
        };
        m_humanBodyNameText.text = "Subject: " + m_gameHandlerRef.m_humanBody.GetHumansName(); 
    }

    public void Start()
    {
        m_residingMapNode = m_viewedBodyPartUI.GetResidingMapNode();
        m_currentPan = m_residingMapNode.transform.position;
        m_currentPan.z = m_startingCameraZPos;
        m_backdropRefStartingSize = m_backdropRef.transform.localScale;
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
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            m_currentPan += new Vector3(-1*Time.deltaTime,0f,0f);
            ApplyZoomAndPan();
        }
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

    void ApplyZoomAndPan()
    {
        m_cameraRef.transform.position = m_currentPan;// * m_currentZoom;
        m_backdropRef.transform.position = new Vector3(m_currentPan.x, m_currentPan.y, m_backdropRef.transform.position.z);
        //Debug.Log(m_currentZoomLocation);
        m_cameraRef.orthographicSize = m_startingCameraSize/m_currentZoom;// new Vector3(m_currentZoom, m_currentZoom, 1f);
        m_backdropRef.transform.localScale = m_backdropRefStartingSize / m_currentZoom;
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
    }

    public void DismissNotification()
    {
        // button closes notif - setactive(false);
        m_lostNotificationRef.SetActive(false);

    }
}
