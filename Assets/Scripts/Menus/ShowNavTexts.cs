using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Show nav texts for controller and gamepad controls depending on player setting
// Will only affect nav texts tagged with NavText

public class ShowNavTexts : MonoBehaviour
{
    NavTextDisplay[] navTexts;

    bool hideKeyboard;
    bool hideGamepad;

    void Awake()
    {
        GameObject[] navObjects = GameObject.FindGameObjectsWithTag("NavText");

        navTexts = new NavTextDisplay[navObjects.Length];
        for (int i = 0; i < navObjects.Length; i++)
        {
            navTexts[i] = navObjects[i].GetComponent<NavTextDisplay>();
        } 
    }

    public void UpdateControlsDisplay()
    {
        if (navTexts == null)
        {
            return;
        }

        // Toggle control displays depending on player's preferences
        hideKeyboard = PlayerPrefs.GetString("HideKeyboardControls") == "On" ? true : false;
        hideGamepad = PlayerPrefs.GetString("HideGamepadControls") == "On" ? true : false;

        foreach (NavTextDisplay n in navTexts)
        {
            // Set nav text display only if the nav text has been updated
            if (n.HasUpdated)
            {
                if ((n.IsKeyboard && hideKeyboard) || (!n.IsKeyboard && hideGamepad))
                {
                    n.gameObject.SetActive(false);
                }
                else
                {
                    n.gameObject.SetActive(true);
                }
            }
        }
    }
}
