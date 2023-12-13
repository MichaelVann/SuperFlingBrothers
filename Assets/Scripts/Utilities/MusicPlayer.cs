using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayer : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    BattleManager m_battleManagerRef;

    public AudioMixer m_audioMixer;

    AudioSource m_characterAudioSource;
    AudioSource m_heartBeatAudioSource;
    AudioSource m_idlerAudioSource;
    AudioSource m_inertiaDasherAudioSource;
    AudioSource m_dodgerAudioSource;
    AudioSource m_healerAudioSource;
    AudioSource m_strikerAudioSource;
      

    bool m_timeWasFrozen = false;
    bool m_musicSwapped = false;

    public AudioClip m_characterMusic;
    public AudioClip m_heartBeatMusic;
    public AudioClip m_idlerMusic;
    public AudioClip m_inertiaDasherMusic;
    public AudioClip m_dodgerMusic;
    public AudioClip m_healerMusic;
    public AudioClip m_strikerMusic;

    // Start is called before the first frame update
    void Awake()
    {
        //m_audioMixer = GetComponent<AudioMixer>();

        m_characterAudioSource = GetComponent<AudioSource>();
        m_characterAudioSource.clip = m_characterMusic;
        
        m_heartBeatAudioSource = gameObject.AddComponent<AudioSource>();
        m_heartBeatAudioSource.clip = m_heartBeatMusic;

        AudioMixerGroup[] audioMixerGroups = m_audioMixer.FindMatchingGroups("");
        m_characterAudioSource.outputAudioMixerGroup = m_audioMixer.FindMatchingGroups("Character")[0];
        m_heartBeatAudioSource.outputAudioMixerGroup = m_audioMixer.FindMatchingGroups("Heartbeat")[0];

        SetupAudioSource(m_idlerAudioSource, m_idlerMusic, "Enemies");
        SetupAudioSource(m_inertiaDasherAudioSource, m_inertiaDasherMusic, "Enemies");

    }

    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_battleManagerRef = FindObjectOfType<BattleManager>();
        m_battleManagerRef.m_enemyCountChangeDelegate = new BattleManager.onEnemyCountChangeDelegate(OnEnemyCountChange);

        if (m_gameHandlerRef.m_audioManager.IsMusicEnabled())
        {
            m_characterAudioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float pitch = 1f;
        float pitchRetention = 0.2f;
        pitch *= pitchRetention + (Time.timeScale * (1f - pitchRetention));

        // check to see if time is frozen
        if (m_battleManagerRef.m_timeFrozen)
        {     // check whether we have dealt with the swap before; if we haven't:
            if (!m_timeWasFrozen)
            {
                // Freeze has just happened
                m_timeWasFrozen = true;
                m_heartBeatAudioSource.Play();
                m_characterAudioSource.loop = false;
                m_characterAudioSource.bypassEffects = false;
            }
            // if we have, must be coming through second time or later so
            // check to see if the music clip is still playing and if it's been swapped
            // if it isn't and hasn't we can swap it
            else if (!m_characterAudioSource.isPlaying && !m_musicSwapped)
            {
                m_characterAudioSource.clip = m_heartBeatMusic;
                m_characterAudioSource.loop = true;
                m_characterAudioSource.Play();
                m_musicSwapped = true;
            }
        }
        else
        // if time isn't frozen, check to see if we have swapped things back
        {
            if (m_timeWasFrozen)
            {
                // Check if heartbeat file still going or if we already stopped it
                if (m_heartBeatAudioSource.isPlaying)
                {
                    m_heartBeatAudioSource.Stop();
                    m_characterAudioSource.loop = false;
                    m_characterAudioSource.bypassEffects = true;
                }
            }
            // Don't change the toggle back until the music has stopped playing though
            if (!m_characterAudioSource.isPlaying)
            {
                m_characterAudioSource.clip = m_characterMusic;
                m_characterAudioSource.loop = true;
                m_characterAudioSource.Play();
                m_musicSwapped = false;
                m_timeWasFrozen = false;
            }
        }

        /*
        if (m_battleManagerRef.m_timeFrozen && !m_heartBeatAudioSource.isPlaying)
        {
            m_heartBeatAudioSource.Play();
            if (m_characterAudioSource.isPlaying && m_characterAudioSource.loop)
            else if (!m_characterAudioSource.isPlaying)
            {
                m_characterAudioSource.clip = m_pauseMusic;
                m_characterAudioSource.loop = true;
                m_characterAudioSource.Play();
            }
         }
        else if (!m_battleManagerRef.m_timeFrozen && m_heartBeatAudioSource.isPlaying)
        {
            m_heartBeatAudioSource.Stop();

        }
        */

        // m_characterAudioSource.volume = pitch;
    }

    void SetupAudioSource(AudioSource a_audioSource, AudioClip a_clip, string a_mixerGroupName)
    {
        a_audioSource = gameObject.AddComponent<AudioSource>();
        a_audioSource.clip = a_clip;
        a_audioSource.outputAudioMixerGroup = m_audioMixer.FindMatchingGroups(a_mixerGroupName)[0];
    }

    void OnEnemyCountChange()
    {
        if (m_battleManagerRef.m_enemyTypeCounts[(int) Enemy.eEnemyType.InertiaDasher]>0)
        {
            m_characterAudioSource.clip = m_heartBeatMusic;
            m_characterAudioSource.Play();
        }
    }
}

