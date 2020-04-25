using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class for a sound object

[System.Serializable]
public class Sound
{
    // Sound clip properties
    [SerializeField]
    string clipName;
    [SerializeField]
    AudioClip clip;
    [SerializeField] [Range(0, 1)]
    float volume;
    [SerializeField] [Range(0, 3)]
    float pitch;
    [SerializeField]
    bool loop;

    AudioSource audioSrc;

    // Getters and setters
    public string ClipName { get { return clipName; } set { clipName = value; } }
    public AudioClip Clip { get { return clip; } set { clip = value; } }
    public float Volume { get { return volume; } set { volume = value; } }
    public float Pitch { get { return pitch; } set { pitch = value; } }
    public bool Loop { get { return loop; } set { loop = value; } }
    public AudioSource AudioSrc { get { return audioSrc; } set { audioSrc = value; } }

}
