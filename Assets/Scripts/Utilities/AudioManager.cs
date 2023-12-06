using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager
{
    internal bool m_muted;
    internal bool m_musicEnabled;
    internal bool m_soundEffectsEnabled;


    internal void ToggleMuted() { m_muted = !m_muted; }

    internal void ToggleMusic() { m_musicEnabled = !m_musicEnabled; }

    internal void ToggleSoundEffects() { m_soundEffectsEnabled = !m_soundEffectsEnabled; }

    internal bool IsMusicEnabled()
    {
        bool retVal = m_musicEnabled;
        retVal &= !m_muted;
        return retVal;
    }

    internal bool IsSoundEffectsOn()
    {
        bool retVal = m_soundEffectsEnabled;
        retVal &= !m_muted;
        return retVal;
    }

    public AudioManager()
    {
        m_muted = false;
        m_musicEnabled = false;
        m_soundEffectsEnabled = true;
    }
}
