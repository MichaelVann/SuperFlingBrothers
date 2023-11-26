using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationBar : MonoBehaviour
{
    public GameObject[] m_occluderRefs;
    public Button[] m_buttonRefs;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetSelected(int a_index)
    {
        for (int i = 0; i < m_occluderRefs.Length; i++)
        {
            bool selectedButton = (a_index == i ? true : false);
            m_occluderRefs[i].gameObject.SetActive(selectedButton);
            m_buttonRefs[i].interactable = !selectedButton;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
