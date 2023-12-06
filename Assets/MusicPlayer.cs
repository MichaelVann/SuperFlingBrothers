using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    BattleManager m_battleManagerRef;
    AudioSource m_mainAudioSource;
    AudioSource m_heartBeatAudioSource;

    public AudioClip m_mainMusic;
    public AudioClip m_heartBeatAudio;
    // Start is called before the first frame update
    void Awake()
    {
        m_mainAudioSource = GetComponent<AudioSource>();
        m_mainAudioSource.clip = m_mainMusic;

        m_heartBeatAudioSource = gameObject.AddComponent<AudioSource>();
        m_heartBeatAudioSource.clip = m_heartBeatAudio;
    }
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_battleManagerRef = FindObjectOfType<BattleManager>();

        if (m_gameHandlerRef.m_audioManager.IsMusicEnabled())
        {
            m_mainAudioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float pitch = 1f;
        float pitchRetention = 0.2f;
        pitch *= pitchRetention + (Time.timeScale * (1f - pitchRetention));

        if (m_battleManagerRef.m_timeFrozen && !m_heartBeatAudioSource.isPlaying)
        {
            m_heartBeatAudioSource.Play();
        }
        else if (!m_battleManagerRef.m_timeFrozen && m_heartBeatAudioSource.isPlaying)
        {
            m_heartBeatAudioSource.Stop();
        }

        m_mainAudioSource.volume = pitch;
    }
}
