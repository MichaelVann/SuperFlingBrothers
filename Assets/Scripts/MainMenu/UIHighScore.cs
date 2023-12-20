using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIHighScore : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_rankTextRef;
    [SerializeField] TextMeshProUGUI m_squadNameTextRef;
    [SerializeField] TextMeshProUGUI m_daysCountTextRef;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void Init(int a_rank, string a_squadName, int a_score)
    {
        m_rankTextRef.text = "#" + a_rank;
        m_squadNameTextRef.text = "" + a_squadName;
        m_daysCountTextRef.text = "<color=red>" + a_score + "</color>days";
    }
}
