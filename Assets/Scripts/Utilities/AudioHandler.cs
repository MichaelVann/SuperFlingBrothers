using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioHandler : MonoBehaviour
{
    GameHandler m_gameHandlerRef;
    BattleManager m_battleManagerRef;

    internal bool m_muted;
    internal bool m_musicEnabled;
    internal bool m_soundEffectsEnabled;
    internal float m_sceneFadeAmount = 0f;
    internal float m_masterVolume = 1f;
    internal float m_musicVolume = 1f;
    internal float m_soundEffectsVolume = 1f;
   
    public AudioMixer m_audioMixer;

    AudioSource m_characterAudioSource;
    AudioSource m_heartBeatAudioSource;
    AudioSource m_idlerAudioSource;
    AudioSource m_inertiaDasherAudioSource;
    AudioSource m_dodgerAudioSource;
    AudioSource m_healerAudioSource;
    AudioSource m_strikerAudioSource;
    AudioSource[] m_enemyAudioSources;

    AudioSource m_soundEffectsAudioSource;
    AudioSource m_menuMusicAudioSource;

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
    public AudioClip m_explosionSound;

    public AudioClip m_menuMusic;

    internal bool m_inBattle = false;

    internal void ToggleMuted() { m_muted = !m_muted; Refresh(); }

    internal void ToggleMusic() { m_musicEnabled = !m_musicEnabled; Refresh(); }

    internal void ToggleSoundEffects() { m_soundEffectsEnabled = !m_soundEffectsEnabled; Refresh(); }

    void Awake()
    {
        m_gameHandlerRef = FindObjectOfType<GameHandler>();
        m_menuMusicAudioSource = gameObject.AddComponent<AudioSource>();
        m_enemyAudioSources = new AudioSource[(int)Enemy.eEnemyType.Count];
        // create AudioSources for various streams
        m_characterAudioSource = gameObject.AddComponent<AudioSource>();
        m_heartBeatAudioSource = gameObject.AddComponent<AudioSource>();
        m_idlerAudioSource = gameObject.AddComponent<AudioSource>();
        m_inertiaDasherAudioSource = gameObject.AddComponent<AudioSource>();
        m_dodgerAudioSource = gameObject.AddComponent<AudioSource>();
        m_healerAudioSource = gameObject.AddComponent<AudioSource>();
        m_strikerAudioSource = gameObject.AddComponent<AudioSource>();
        m_soundEffectsAudioSource = gameObject.AddComponent<AudioSource>();


        AudioMixerGroup[] audioMixerGroups = m_audioMixer.FindMatchingGroups("");
      
        SetupAudioSource(m_characterAudioSource, m_characterMusic, "Character");
        SetupAudioSource(m_heartBeatAudioSource, m_heartBeatMusic, "Heartbeat");

        SetupEnemyMusicSource(m_idlerAudioSource, m_idlerMusic, Enemy.eEnemyType.Idler);
        SetupEnemyMusicSource(m_inertiaDasherAudioSource, m_inertiaDasherMusic, Enemy.eEnemyType.InertiaDasher);
        SetupEnemyMusicSource(m_dodgerAudioSource, m_dodgerMusic, Enemy.eEnemyType.Dodger);
        SetupEnemyMusicSource(m_healerAudioSource, m_healerMusic, Enemy.eEnemyType.Healer);
        SetupEnemyMusicSource(m_strikerAudioSource, m_strikerMusic, Enemy.eEnemyType.Striker);


        SetupAudioSource(m_soundEffectsAudioSource, null, "Sound Effects");
        SetupAudioSource(m_menuMusicAudioSource, m_menuMusic, "Music");

        m_muted = false;
        m_musicEnabled = false;
        m_soundEffectsEnabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    internal void PlayBattleMusic(BattleManager a_battleManagerRef)
    { 
        m_inBattle = true;
        m_battleManagerRef = a_battleManagerRef;
        m_characterAudioSource.Play();
        m_heartBeatAudioSource.Play();
        m_menuMusicAudioSource.Stop();
        RefreshBattleMusic();
        Refresh();
        m_battleManagerRef.m_enemyCountChangeDelegate = new BattleManager.onEnemyCountChangeDelegate(OnEnemyCountChange);
    }

    internal void PlayMenuMusic()
    {
        m_inBattle = false;
        if (!m_menuMusicAudioSource.isPlaying)
        {
            m_menuMusicAudioSource.clip = m_menuMusic;
            m_menuMusicAudioSource.Play();

            m_characterAudioSource.Stop();
            m_heartBeatAudioSource.Stop();
            m_idlerAudioSource.Stop();
            m_inertiaDasherAudioSource.Stop();
            m_dodgerAudioSource.Stop();
            m_healerAudioSource.Stop();
            m_strikerAudioSource.Stop();
        }
        Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_inBattle)
        {
            BattleUpdate();
        }
        else
        {
            RegularUpdate();
        }
    }

    float VolumeScaleToDB(float a_volume)
    {
        float volInDB = -80f;

        if (a_volume > 0f)
        {
            const float mult = 33.2f; // 10/Log(2)
            volInDB = mult * Mathf.Log10(a_volume);
        }

        return volInDB;
    }

    void UpdateVolumes()
    {
        //Sound
        float soundVol = 1f;
        soundVol *= m_soundEffectsVolume;
        soundVol *= m_soundEffectsEnabled ? 1f : 0f;
        m_audioMixer.SetFloat("soundEffectsVol", VolumeScaleToDB(soundVol));

        //Music
        float musicVol = 1f;
        musicVol *= m_musicVolume;
        musicVol *= m_musicEnabled ? 1f : 0f;
        m_audioMixer.SetFloat("musicVol", VolumeScaleToDB(musicVol));

        //Master
        float masterVol = 1f;
        masterVol *= (1f - m_sceneFadeAmount);
        masterVol *= m_masterVolume;
        masterVol *= m_muted ? 0f : 1f;

        m_audioMixer.SetFloat("masterVol", VolumeScaleToDB(masterVol));
    }

    internal void BattleUpdate()
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
                m_audioMixer.SetFloat("battleMusicVol", -6f);                
            }
        }
        else
        // if time isn't frozen, check to see if we have swapped things back
        {
            if (m_timeWasFrozen)
            {
                m_timeWasFrozen = false;

                m_audioMixer.SetFloat("heartBeatVol", -80f);
                m_audioMixer.SetFloat("battleMusicVol", 0f);
            }
        }
    }

    internal void RegularUpdate()
    {
            
    }

    void SetupAudioSource(AudioSource a_audioSource, AudioClip a_clip, string a_mixerGroupName)
    {
        a_audioSource.clip = a_clip;
        a_audioSource.loop = true;
        a_audioSource.outputAudioMixerGroup = m_audioMixer.FindMatchingGroups(a_mixerGroupName)[0];
    }

    void SetupEnemyMusicSource(AudioSource a_audioSource, AudioClip a_clip, Enemy.eEnemyType a_type)
    {
        SetupAudioSource(a_audioSource, a_clip, "Enemies");
        m_enemyAudioSources[(int)a_type] = a_audioSource;
    }

    internal void RefreshBattleMusic()
    {
        if (m_inBattle)
        {
            for (int i = 0; i < m_enemyAudioSources.Length; i++)
            {
                AudioSource source = m_enemyAudioSources[i];
                if (source.isPlaying && m_battleManagerRef.m_enemyTypeCounts[i] == 0)
                {
                    source.Stop();
                }
                else if (!source.isPlaying && m_battleManagerRef.m_enemyTypeCounts[i] > 0)
                {
                    source.Play();
                }
            }
        }
    }

    internal void Refresh()
    {
        UpdateVolumes();
    }

    void StopMusic()
    {
        for (int i = 0; i < m_enemyAudioSources.Length; i++)
        {
            m_enemyAudioSources[i].Stop();
        }
        m_menuMusicAudioSource.Stop();
    }

    void OnEnemyCountChange()
    {
        RefreshBattleMusic();      
    }

    public void PlaySoundEffect(AudioClip a_clip, float a_volume)
    {
        if (m_soundEffectsEnabled)
        {
            m_soundEffectsAudioSource.PlayOneShot(a_clip, a_volume);
        }
    }

    public void PlayDamageSound()
    {
        PlaySoundEffect(m_damageSound, 1);     
    }

    public void PlayBounceSound()
    {
        PlaySoundEffect(m_bounceSound, 1);       
    }

    public void PlayFlingSound()
    {
        PlaySoundEffect(m_flingSound, 0.5f);       
    }

    public void PlayPocketSound()
    {
        PlaySoundEffect(m_pocketSound, 0.8f);        
    }

    public void PlayExplosionSound()
    {
        PlaySoundEffect(m_explosionSound, 1f);        
    }
}

