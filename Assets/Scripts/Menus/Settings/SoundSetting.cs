using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    [SerializeField]
    Text text;
    int maxVol = 10;

    public UnityEvent OnVolumeChange;

    // Start is called before the first frame update
    void Start()
    {
        text.text = ((PlayerPrefs.GetInt("SoundVolume", 10))).ToString();
    }

    // Increase volume on button click
    public void IncreaseVolume()
    {
        int newVol = PlayerPrefs.GetInt("SoundVolume") + 1;
        newVol = newVol > maxVol ? maxVol : newVol;
        PlayerPrefs.SetInt("SoundVolume", newVol);

        text.text = newVol.ToString();
        OnVolumeChange.Invoke();
    }

    // Decrease volume on button click
    public void DecreaseVolume()
    {
        int newVol = PlayerPrefs.GetInt("SoundVolume") - 1;
        newVol = newVol < 0 ? 0 : newVol; ;
        PlayerPrefs.SetInt("SoundVolume", newVol);

        text.text = newVol.ToString();
        OnVolumeChange.Invoke();
    }
}
