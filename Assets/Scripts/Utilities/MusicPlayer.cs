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
    List<AudioSource> m_enemyAudioSources;

    bool m_timeWasFrozen = false;
  
    public AudioClip m_characterMusic;
    public AudioClip m_heartBeatMusic;
    public AudioClip m_idlerMusic;
    public AudioClip m_inertiaDasherMusic;
    public AudioClip m_dodgerMusic;
    public AudioClip m_healerMusic;
    public AudioClip m_strikerMusic;

    public AudioClip m_damageSound;
    public AudioClip m_bounceSound;
    public AudioClip m_flingSound;
    public AudioClip m_pocketSound;

    public AudioSource m_defaultAudioSource; 

    void Awake()
    {
        m_defaultAudioSource = gameObject.AddComponent<AudioSource>();
        m_enemyAudioSources = new List<AudioSource>();
        // create AudioSources for various streams
        m_characterAudioSource = gameObject.AddComponent<AudioSource>();
        m_heartBeatAudioSource = gameObject.AddComponent<AudioSource>();
        m_idlerAudioSource = gameObject.AddComponent<AudioSource>();
        m_inertiaDasherAudioSource = gameObject.AddComponent<AudioSource>();
        m_dodgerAudioSource = gameObject.AddComponent<AudioSource>();
        m_healerAudioSource = gameObject.AddComponent<AudioSource>();
        m_strikerAudioSource = gameObject.AddComponent<AudioSource>();


        AudioMixerGroup[] audioMixerGroups = m_audioMixer.FindMatchingGroups("");
      
        SetupAudioSource(m_characterAudioSource, m_characterMusic, "Character");
        SetupAudioSource(m_heartBeatAudioSource, m_heartBeatMusic, "Heartbeat");
        SetupAudioSource(m_idlerAudioSource, m_idlerMusic, "Enemies");
        SetupAudioSource(m_inertiaDasherAudioSource, m_inertiaDasherMusic, "Enemies");
        SetupAudioSource(m_dodgerAudioSource, m_dodgerMusic, "Enemies");
        SetupAudioSource(m_healerAudioSource, m_healerMusic, "Enemies");
        SetupAudioSource(m_strikerAudioSource, m_strikerMusic, "Enemies");

    }

    // Start is called before the first frame update
    void Start()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_battleManagerRef = FindObjectOfType<BattleManager>();
        m_battleManagerRef.m_enemyCountChangeDelegate = new BattleManager.onEnemyCountChangeDelegate(OnEnemyCountChange);

        if (m_gameHandlerRef.m_audioManager.IsMusicEnabled())
        {
            m_characterAudioSource.Play();
            m_heartBeatAudioSource.Play();
            Refresh();
            
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
                m_audioMixer.SetFloat("heartBeatVol", 4f);
                m_audioMixer.SetFloat("characterVol", -6f);
                m_audioMixer.SetFloat("enemiesVol", -6f);
               
            }
        }
        else
        // if time isn't frozen, check to see if we have swapped things back
        {
            if (m_timeWasFrozen)
            {
                m_timeWasFrozen = false;

                m_audioMixer.SetFloat("heartBeatVol", -80f);
                m_audioMixer.SetFloat("characterVol", 0f);
                m_audioMixer.SetFloat("enemiesVol", 0f);
            }
         }

         }

    void SetupAudioSource(AudioSource a_audioSource, AudioClip a_clip, string a_mixerGroupName)
    {
        a_audioSource.clip = a_clip;
        a_audioSource.loop = true;
        a_audioSource.outputAudioMixerGroup = m_audioMixer.FindMatchingGroups(a_mixerGroupName)[0];
    }

    void Refresh()
    {
        
        if (m_idlerAudioSource.isPlaying &&
            m_battleManagerRef.m_enemyTypeCounts[0] == 0)
        {
            m_idlerAudioSource.Stop();
        }
        else if (!m_idlerAudioSource.isPlaying &&
            m_battleManagerRef.m_enemyTypeCounts[0] > 0)
        {
            m_idlerAudioSource.Play();
        }

        if (m_inertiaDasherAudioSource.isPlaying &&
            m_battleManagerRef.m_enemyTypeCounts[1] == 0)
        {
            m_inertiaDasherAudioSource.Stop();
        }
        else if (!m_inertiaDasherAudioSource.isPlaying &&
            m_battleManagerRef.m_enemyTypeCounts[1] > 0)
        {
            m_inertiaDasherAudioSource.Play();
        }

        if (m_dodgerAudioSource.isPlaying &&
            m_battleManagerRef.m_enemyTypeCounts[2] == 0)
        {
            m_dodgerAudioSource.Stop();
        }
        else if (!m_dodgerAudioSource.isPlaying &&
            m_battleManagerRef.m_enemyTypeCounts[2] > 0)
        {
            m_dodgerAudioSource.Play();
        }

        if (m_healerAudioSource.isPlaying &&
            m_battleManagerRef.m_enemyTypeCounts[3] == 0)
        {
            m_healerAudioSource.Stop();
        }
        else if (!m_healerAudioSource.isPlaying &&
            m_battleManagerRef.m_enemyTypeCounts[3] > 0)
        {
            m_healerAudioSource.Play();
        }

        if (m_strikerAudioSource.isPlaying &&
            m_battleManagerRef.m_enemyTypeCounts[4] == 0)
        {
            m_strikerAudioSource.Stop();
        }
        else if (!m_strikerAudioSource.isPlaying &&
            m_battleManagerRef.m_enemyTypeCounts[4] > 0)
        {
            m_strikerAudioSource.Play();
        }
    }

    void OnEnemyCountChange()
    {

        Refresh();      
        
    }

    public void PlayDamageSound()
    {
        m_defaultAudioSource.PlayOneShot(m_damageSound, 1);
    }

    public void PlayBounceSound()
    {
        m_defaultAudioSource.PlayOneShot(m_bounceSound, 1);
    }

    public void PlayFlingSound()
    {
        m_defaultAudioSource.PlayOneShot(m_flingSound, 0.5f);
    }

    public void PlayPocketSound()
    {
        m_defaultAudioSource.PlayOneShot(m_pocketSound, 0.5f);
    }
}

