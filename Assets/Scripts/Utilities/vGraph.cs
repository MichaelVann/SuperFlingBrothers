using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class vGraph : MonoBehaviour
{
    public GameObject m_verticalMarkRef;
    public GameObject m_horizontalMarkRef;
    public GameObject m_dotRef;
    public Text m_currentValueTextRef;
    public Text m_titleTextRef;

    List<GameObject> m_verticalAxisTextList;
    List<GameObject> m_horizontalAxisTextList;

    public int m_verticalAxisDelineations;
    public int m_horizontalAxisDelineations;

    public GameObject m_graphBackgroundRef;

    float[] m_trackedValues;

    public GameObject[] m_dots;

    float yScale = 5f;

    public LineRenderer m_lineRenderer;

    float m_bgWidth;
    float m_bgHeight;

    float m_graphVerticalPadding = 20f;

    private void Awake()
    {
        m_bgWidth = m_graphBackgroundRef.GetComponent<RectTransform>().rect.width;
        m_bgHeight = m_graphBackgroundRef.GetComponent<RectTransform>().rect.height;

        m_trackedValues = new float[10];
        m_dots = new GameObject[m_trackedValues.Length];

        m_dots[0] = m_dotRef;
        for (int i = 1; i < m_dots.Length; i++)
        {
            m_dots[i] = Instantiate<GameObject>(m_dotRef, m_graphBackgroundRef.transform);
        }

        m_verticalAxisDelineations = 6;

        m_verticalAxisTextList = new List<GameObject>();
        for (int i = 0; i < m_verticalAxisDelineations; i++)
        {
            m_verticalAxisTextList.Add(Instantiate<GameObject>(m_verticalMarkRef, m_graphBackgroundRef.transform));
        }
        m_horizontalAxisTextList = new List<GameObject>();

        SetupVerticalDelineations();

        Destroy(m_verticalMarkRef);

        m_lineRenderer.startWidth = 0.02f;
        m_lineRenderer.endWidth = 0.02f;
        m_lineRenderer.positionCount = m_dots.Length;


        UpdateYScale();
        UpdateDotsAndLine();
    }

    // Start is called before the first frame update
    void Start()
    {


    }

    public void Init(float[] a_trackedNumbers, string a_name)
    {
        m_trackedValues = a_trackedNumbers;
        m_titleTextRef.text = a_name;
        Refresh();
    }

    private void UpdateYScale()
    {
        float largestValue = 1f;
        for (int i = m_trackedValues.Length - 1; i >= 0; i--)
        {
            if (m_trackedValues[i] > largestValue)
            {
                largestValue = m_trackedValues[i];
            }
        }

        while (largestValue > yScale && largestValue != Mathf.Infinity)
        {
            yScale *= 2f;
        }

        while (largestValue <= yScale / 2.5f && largestValue != 0f)
        {
            yScale /= 2f;
        }

        SetupVerticalDelineations();
    }

    public void Refresh()
    {
        UpdateYScale();
        UpdateDotsAndLine();
        m_currentValueTextRef.text = "Current Value: " + m_trackedValues[0];
    }

    private void SetupVerticalDelineations()
    {
        float markGap = (m_bgHeight - 2 * m_graphVerticalPadding) / (m_verticalAxisTextList.Count-1);

        for (int i = 0; i < m_verticalAxisTextList.Count; i++)
        {
            float yPos = m_graphVerticalPadding  + i * markGap;
            yPos -= m_bgHeight / 2f;
            m_verticalAxisTextList[i].transform.localPosition = new Vector3(-m_bgWidth / 2f - 20f, yPos);
            m_verticalAxisTextList[i].GetComponent<Text>().text = "" + VLib.TruncateFloatsDecimalPlaces((yScale / (float)(m_verticalAxisTextList.Count - 1)) * i, 2);
            m_verticalAxisTextList[i].transform.SetParent(this.transform);
        }
    }

    private void SetupDotsAndLine()
    {
        for (int i = 1; i < m_dots.Length; i++)
        {
            if (m_dots[i] != null)
            {
                Destroy(m_dots[i]);
            }
        }
        m_dots = new GameObject[m_trackedValues.Length];
        m_dots[0] = m_dotRef;
        for (int i = 1; i < m_dots.Length; i++)
        {
            m_dots[i] = Instantiate<GameObject>(m_dotRef, m_graphBackgroundRef.transform);
        }
        m_lineRenderer.positionCount = m_dots.Length;

    }

    private void UpdateDotsAndLine()
    {
        float xGap = m_bgWidth / (m_trackedValues.Length);

        Vector3[] linePositions = new Vector3[m_trackedValues.Length];

        if (m_dots.Length != m_trackedValues.Length)
        {
            SetupDotsAndLine();
        }

        for (int i = 0; i < m_trackedValues.Length; i++)
        {
            float x = m_bgWidth - ((i + 1) * xGap);
            float y = m_graphVerticalPadding + m_trackedValues[i] / yScale * (m_bgHeight - m_graphVerticalPadding * 2f);
            m_dots[i].transform.localPosition = new Vector3(x, y, 0f);
            m_dots[i].transform.localPosition -= new Vector3(m_bgWidth / 2f, m_bgHeight / 2f, 0f);
            m_dots[i].transform.position = m_dots[i].transform.parent.TransformPoint(m_dots[i].transform.localPosition);
            linePositions[i] = m_dots[i].transform.position;
            linePositions[i].z = 0f;
        }

        m_lineRenderer.SetPositions(linePositions);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
