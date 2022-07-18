using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioClip))]

public class SoundEffects
    : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> sounds = new List<AudioClip>();    
    [SerializeField]
    private List<string> soundNames = new List<string>();
    
    private AudioSource _effectSource;
    
    int arrayNum;
    
    private void Awake()
    {
        _effectSource = GetComponent<AudioSource>();
    }
    
    
    public void PlaySound(string soundName)
    {
        int soundIndex = soundNames.IndexOf(soundName);
        if (soundIndex > -1)
        {
            PlaySound(soundIndex);
        }
    }

    public bool currentlyPlayingSound(string soundName)
    {
        int soundIndex = soundNames.IndexOf(soundName);
        if (soundIndex > -1)
        {
            return currentlyPlayingSound(soundIndex);
        }

        return false;
    }

    public void PlaySound(int soundIndex)
    {
        _effectSource.PlayOneShot(sounds[soundIndex]);
    }

    public bool currentlyPlayingSound(int soundIndex)
    {
        return _effectSource.isPlaying && _effectSource.clip.name == sounds[soundIndex].name;
    }

    public bool playingAnySound()
    {
        return _effectSource.isPlaying;
    }
}
