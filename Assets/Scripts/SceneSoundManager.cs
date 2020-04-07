using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sound manager for scene specific sounds

public class SceneSoundManager : MonoBehaviour
{
    [SerializeField]
    AudioSource[] audioSources;
    float[] defaultValues;

    float soundVolume;

    void Awake()
    {
        defaultValues = new float[audioSources.Length];
        for (int i = 0; i < audioSources.Length; i++)
        {
            defaultValues[i] = audioSources[i].volume;
        }
        UpdateVolume();
    }

    // Update sound volume when OnVolumeChange occurs from sound settings
    public void UpdateVolume()
    {
        soundVolume = PlayerPrefs.GetInt("SoundVolume", 10) / 10f;
        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i].volume = defaultValues[i] * soundVolume;
        }
    }
}
