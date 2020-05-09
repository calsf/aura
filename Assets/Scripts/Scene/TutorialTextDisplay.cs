using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTextDisplay : MonoBehaviour
{
    [SerializeField]
    NavText navText;

    TextMeshPro text;

    // Start is called before the first frame update
    void Start()
    {
        UpdateNavText();
        ControlsManager.ControlInstance.OnBindChange.AddListener(UpdateNavText);
    }

    void OnDisable()
    {
        ControlsManager.ControlInstance.OnBindChange.RemoveListener(UpdateNavText);
    }

    // Update the nav text at start and also update when OnControlChange event in ControlsManager occurs
    public void UpdateNavText()
    {
        // Find the binded button to the key for keyboard and for gamepad
        string keybindName = "";
        string padbindName = "";
        string newString = navText.text;

        // Only replace if there is a keyName/padName
        if (navText.keyName != "")
        {
            keybindName = ControlsManager.ControlInstance.Keybinds[navText.keyName].ToString();

            // Clean up the keybind string
            keybindName = ControlsManager.ControlInstance.CleanString(keybindName);

            // Replace keybind string in the navText's text with the binded keybind button
            newString = newString.Replace(navText.keyName, keybindName);
        }

        if (navText.padName != "")
        {
            padbindName = ControlsManager.ControlInstance.Padbinds[navText.padName].ToString();

            // Clean up the padbind string
            padbindName = ControlsManager.ControlInstance.CleanString(padbindName);

            // Replace padbind string in the navText's text with the binded padbind button
            newString = newString.Replace(navText.padName, padbindName);
        }

        // Update the text display with modified string
        text = GetComponent<TextMeshPro>();
        text.text = newString;
    }
}
