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
    public UICheckBox m_muteCheckBox;
    public UICheckBox m_musicCheckBox;
    public UICheckBox m_soundEffectCheckBox;

    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        Refresh();
    }

    // Update is called once per frame
    void Update()
    {

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

    public void SaveButtonPressed()
    {
        m_gameHandlerRef.SaveGame();
        Refresh();
    }

    public void LoadButtonPressed()
    {
        m_gameHandlerRef.LoadGame();
        Refresh();
    }

    internal void Refresh()
    {
        m_muteCheckBox.SetToggled(m_gameHandlerRef.m_audioManager.m_muted);
        m_musicCheckBox.SetToggled(m_gameHandlerRef.m_audioManager.m_musicEnabled);
        m_soundEffectCheckBox.SetToggled(m_gameHandlerRef.m_audioManager.m_soundEffectsEnabled);
        m_resolutionTextRef.text = "Resolution: " + Screen.width + " x " + Screen.height;
        m_safeAreaTextRef.text = "Safe Area: " + Screen.safeArea.width + " x " + Screen.safeArea.height;
    }

    public void ToggleMuted()
    {
        m_gameHandlerRef.m_audioManager.ToggleMuted();
        Refresh();
    }

    public void ToggleMusic()
    {
        m_gameHandlerRef.m_audioManager.ToggleMusic();
        Refresh();
    }

    public void ToggleSoundEffects()
    {
        m_gameHandlerRef.m_audioManager.ToggleMuted();
        Refresh();
    }

}
