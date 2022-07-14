using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public List<AudioClip> soundList = new List<AudioClip>();
    public List<AudioClip> songList = new List<AudioClip>();

    public AudioSource songSrc;

    public enum MusicState
    {
        Idle,
        Detected,
        Chasing
    }

    public MusicState _state;


    private void Start()
    {
        playMusic("Idle");
    }

    public void playSound(string soundName, AudioSource src)
    {
        switch (soundName)
        {
            case "Walk":
                src.PlayOneShot(soundList[0]);
                break;
        }
    }

    public void playMusic(string songName)
    {
        songSrc.loop = true;
        switch (songName)
        {
            case "Idle":

                _state = MusicState.Idle;
                songSrc.clip = songList[0];
                songSrc.Play();

                break;

            case "Detected":

                _state = MusicState.Detected;
                songSrc.clip = songList[1];
                songSrc.Play();

                break;

            case "Chasing":

                _state = MusicState.Chasing;
                songSrc.clip = songList[2];
                songSrc.Play();

                break;

        }
    }

    public int returnState()
    {
        switch (_state)
        {
            case MusicState.Idle:
                return 1;

            case MusicState.Detected:
                return 2;

            case MusicState.Chasing:
                return 3;
        }

        return 0;
    }
}
