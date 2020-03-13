using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionButton : MonoBehaviour
{
    [SerializeField]
    int width;
    [SerializeField]
    int height;
    [SerializeField]
    Image img;
    [SerializeField]
    Color selectedColor;
    [SerializeField]
    Color defaultColor;
    [SerializeField]
    Image[] allImage;

    void Awake()
    {
        //Set button equal to starting resolution
        if (PlayerPrefs.GetInt("Screenmanager Resolution Width") == width && PlayerPrefs.GetInt("Screenmanager Resolution Height") == height)
        {
            img.color = selectedColor;
        }
    }

    //Set resolution
    public void SetResolution()
    {
        Screen.SetResolution(width, height, Screen.fullScreen);
        foreach (Image i in allImage) //Reset all button images to unselected
        {
            i.color = defaultColor;
        }
        img.color = selectedColor; //Set this button to selected
    }
}
