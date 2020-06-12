using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sound manager for non spatial/global audio sources 
// (enemy hit and death sounds are spatial which should not be included here, each enemy should have their own audio sources attached to enemy object)

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    Sound[] sounds;
    static SoundManager soundInstance;
    float soundVolume;

    AudioSource[] audioSources;

    public static SoundManager SoundInstance { get { return soundInstance; } }

    void Awake()
    {
        //Singleton
        if (soundInstance == null)
        {
            soundInstance = this;
        }
        else
        {
            Destroy(soundInstance.gameObject);
            soundInstance = this;
        }

        audioSources = new AudioSource[sounds.Length];
        soundVolume = PlayerPrefs.GetInt("SoundVolume", 10) / 10f;

        // Add an audio source for every sound object and initialize the audio source values accordingly
        for (int i = 0; i < sounds.Length; i++)
        {
            sounds[i].AudioSrc = gameObject.AddComponent<AudioSource>();
            sounds[i].AudioSrc.clip = sounds[i].Clip;
            sounds[i].AudioSrc.loop = sounds[i].Loop;
            sounds[i].AudioSrc.volume = sounds[i].Volume * soundVolume;
            sounds[i].AudioSrc.pitch = sounds[i].Pitch;

            audioSources[i] = sounds[i].AudioSrc;
        }
    }

    // Play sound one shot, cannot be stopped
    public void PlaySound (string name)
    {
        Sound s = Array.Find(sounds, Sound => Sound.ClipName == name);
        if (s != null)
        {
            s.AudioSrc.PlayOneShot(s.Clip);
        }
    }

    // Play sound that can be stopped but will cut off if replayed
    public void PlayStoppableSound (string name)
    {
        Sound s = Array.Find(sounds, Sound => Sound.ClipName == name);
        if (s != null)
        {
            s.AudioSrc.Play();
        }
    }

    // Stop playing sound
    public void StopSound (string name)
    {
        Sound s = Array.Find(sounds, Sound => Sound.ClipName == name);
        if (s != null)
        {
            s.AudioSrc.Stop();
        }
    }

    // Update sound volume when OnVolumeChange occurs from sound settings
    public void UpdateVolume()
    {
        soundVolume = PlayerPrefs.GetInt("SoundVolume", 10) / 10f;
        for (int i = 0; i < sounds.Length; i++)
        {
            audioSources[i].volume = sounds[i].Volume * soundVolume;
        } 
    }
}
