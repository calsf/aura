using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullScreenBtn : MonoBehaviour
{
    [SerializeField]
    Image img;
    [SerializeField]
    Color selectedColor;
    [SerializeField]
    Color defaultColor;

    void Awake()
    {
        //Show if currently in fullscreen
        if (Screen.fullScreen)
        {
            img.color = selectedColor;
        }
        else
        {
            img.color = defaultColor;
        }
    }

    public void ToggleFullScreen()
    {
        //Toggle full screen off or on
        if (Screen.fullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            img.color = defaultColor;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            img.color = selectedColor;
        }
    }
}
