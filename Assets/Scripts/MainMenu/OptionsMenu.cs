using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public Slider m_xpRewardScaleSlider;
    public Text m_xpRewardText;
    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnXPRewardSliderChange()
    {
        m_gameHandlerRef.m_enemyXPScale = m_xpRewardScaleSlider.value;
        m_xpRewardText.text = "" + m_xpRewardScaleSlider.value;
    }

    void OnXPRequirementSliderChange()
    {

    }

    void OnLevelUPBaseSliderChange()
    {

    }
}
