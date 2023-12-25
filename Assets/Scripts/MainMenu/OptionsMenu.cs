﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    public Slider m_xpRewardScaleSlider;
    public Text m_xpRewardTextRef;
    public Text m_resolutionTextRef;
    public Text m_safeAreaTextRef;
    [SerializeField] TextMeshProUGUI m_saveLocationTextRef;
    public UICheckBox m_muteCheckBox;
    public UICheckBox m_musicCheckBox;
    public UICheckBox m_soundEffectCheckBox;

    public GameObject m_confirmationBoxPrefab;

    [SerializeField] Slider m_volumeSlider;
    [SerializeField] Slider m_soundFXVolumeSlider;
    [SerializeField] Slider m_musicVolumeSlider;

    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        Refresh();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnVolumeSliderChanged()
    {
        m_gameHandlerRef.m_audioHandlerRef.m_masterVolume = m_volumeSlider.value;
        m_gameHandlerRef.m_audioHandlerRef.Refresh();
        GameHandler.AutoSaveCheck();
    }

    public void OnSoundEffectsSliderChanged()
    {
        m_gameHandlerRef.m_audioHandlerRef.m_soundEffectsVolume = m_soundFXVolumeSlider.value;
        m_gameHandlerRef.m_audioHandlerRef.Refresh();
        GameHandler.AutoSaveCheck();
    }

    public void OnMusicSliderChanged()
    {
        m_gameHandlerRef.m_audioHandlerRef.m_musicVolume = m_musicVolumeSlider.value;
        m_gameHandlerRef.m_audioHandlerRef.Refresh();
        GameHandler.AutoSaveCheck();
    }

    void LoadAudioSliderValues()
    {
        m_volumeSlider.value = m_gameHandlerRef.m_audioHandlerRef.m_masterVolume;
        m_soundFXVolumeSlider.value = m_gameHandlerRef.m_audioHandlerRef.m_soundEffectsVolume;
        m_musicVolumeSlider.value = m_gameHandlerRef.m_audioHandlerRef.m_musicVolume;
    }

    public void SaveButtonPressed()
    {
        m_gameHandlerRef.SaveGame();
        Refresh();
    }

    public void LoadButtonPressed()
    {
        if (!m_gameHandlerRef.LoadGame())
        {
            ConfirmationBox confirmationBox = Instantiate(m_confirmationBoxPrefab, transform).GetComponent<ConfirmationBox>();
            confirmationBox.SetMessageText("No save file found.");
            confirmationBox.SetToAcknowledgeOnlyMode();
            //confirmationBox.m_confirmationResponseDelegate = new ConfirmationBox.ConfirmationResponseDelegate(SellAllUnequippedEquipment);
        }
        Refresh();
    }

    public void ResetButtonPressed()
    {
        ConfirmationBox confirmationBox = Instantiate(m_confirmationBoxPrefab, transform).GetComponent<ConfirmationBox>();
        confirmationBox.SetMessageText("This will delete your save and reset the current game. Are you sure?");
        confirmationBox.SetConfirmationResponseDelegate(ResetGame);
    }

    public void ResetGame()
    {
        File.Delete(Application.persistentDataPath + "/Data.txt");
        File.Delete(Application.persistentDataPath + "/Data2.txt");
        m_gameHandlerRef.ResetGame();
        m_gameHandlerRef.TransitionScene(GameHandler.eScene.mainMenu);
    }

    internal void Refresh()
    {
        m_muteCheckBox.SetToggled(!m_gameHandlerRef.m_audioHandlerRef.m_muted);
        m_musicCheckBox.SetToggled(m_gameHandlerRef.m_audioHandlerRef.m_musicEnabled);
        m_soundEffectCheckBox.SetToggled(m_gameHandlerRef.m_audioHandlerRef.m_soundEffectsEnabled);

        LoadAudioSliderValues();
        m_gameHandlerRef.m_audioHandlerRef.Refresh();

        //m_resolutionTextRef.text = "Resolution: " + Screen.width + " x " + Screen.height;
        //m_safeAreaTextRef.text = "Safe Area: " + Screen.safeArea.width + " x " + Screen.safeArea.height;
        //m_saveLocationTextRef.text = "Save Location: " + Application.persistentDataPath;
    }

    public void ToggleMuted()
    {
        m_gameHandlerRef.m_audioHandlerRef.ToggleMuted();
        Refresh();
        GameHandler.AutoSaveCheck();
    }

    public void ToggleMusic()
    {
        m_gameHandlerRef.m_audioHandlerRef.ToggleMusic();
        Refresh();
        GameHandler.AutoSaveCheck();
    }

    public void ToggleSoundEffects()
    {
        m_gameHandlerRef.m_audioHandlerRef.ToggleSoundEffects();
        Refresh();
        GameHandler.AutoSaveCheck();
    }

    public void OnClose()
    {
        GameHandler.AutoSaveCheck();
    }

}
