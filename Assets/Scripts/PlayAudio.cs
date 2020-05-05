using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used to play sound in animations

public class PlayAudio : MonoBehaviour
{
    AudioSource clip;
    float defaultVolume;

    void Awake()
    {
        clip = GetComponent<AudioSource>();
        defaultVolume = clip.volume;
    }

    public void Play()
    {
        // Adjust volume according to player's sound volume setting
        clip.volume = defaultVolume * (PlayerPrefs.GetInt("SoundVolume", 10) / 10f);

        clip.Play();
    }
}
