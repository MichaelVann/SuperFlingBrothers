using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public Slider m_xpRewardScaleSlider;
    public Text m_xpRewardTextRef;
    public Text m_resolutionTextRef;
    public Text m_safeAreaTextRef;

    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        m_resolutionTextRef.text = "Resolution: " + Screen.width + " x " + Screen.height;
        m_safeAreaTextRef.text = "Safe Area: " + Screen.safeArea.width + " x " + Screen.safeArea.height;
    }

    public void OnXPRewardSliderChange()
    {
        GameHandler.GAME_enemyXPRewardScale = m_xpRewardScaleSlider.value;
        m_xpRewardTextRef.text = "" + m_xpRewardScaleSlider.value;
    }

    void OnXPRequirementSliderChange()
    {

    }

    void OnLevelUPBaseSliderChange()
    {

    }
}
