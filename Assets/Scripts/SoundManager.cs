using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static Dictionary<string, AudioClip> sounds;
    static AudioSource audioSrc;

    public void Start()
    {
        sounds = new Dictionary<string, AudioClip>();
        sounds["dash"] = Resources.Load<AudioClip>("dash");
        sounds["explo1"] = Resources.Load<AudioClip>("explo1");
        sounds["explo2"] = Resources.Load<AudioClip>("explo2");
        sounds["hit"] = Resources.Load<AudioClip>("hit");
        audioSrc = GetComponent<AudioSource>();
        
    }

    public static void PlaySound (string str)
    {
        audioSrc.PlayOneShot(sounds[str]);
    }
}
