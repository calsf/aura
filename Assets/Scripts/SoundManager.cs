using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    Sound[] sounds;
    static SoundManager soundInstance;

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

        // Add an audio source for every sound object and initialize the audio source values accordingly
        foreach (Sound s in sounds)
        {
            s.AudioSrc = gameObject.AddComponent<AudioSource>();
            s.AudioSrc.clip = s.Clip;
            s.AudioSrc.loop = s.Loop;
            s.AudioSrc.spatialBlend = s.SpatialBlend;
        }
    }

    // Play sound
    public void PlaySound (string name)
    {
        Sound s = Array.Find(sounds, Sound => Sound.ClipName == name);
        if (s != null)
        {
            s.AudioSrc.volume = s.Volume;
            s.AudioSrc.pitch = s.Pitch;
            s.AudioSrc.Play();
        }
    }

    public void StopSound (string name)
    {
        Sound s = Array.Find(sounds, Sound => Sound.ClipName == name);
        if (s != null)
        {
            s.AudioSrc.Stop();
        }
    }
}
