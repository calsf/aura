using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Show nav options/controls in text based on saved keybinds
// Example: text = JumpButton / JumpPad to Buy
// JumpButton will be replaced with the binded key to JumpButton (for example, Space is the keyboard binding to jump so it will show Space)
// JumpPad will be replaced with the binded gamepad button to JumpPad (for example, L2 is the JumpPad binding to jump so it will show L2)

public class NavTextDisplay : MonoBehaviour
{
    [SerializeField]
    NavText navText;

    Text text;

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
        text = GetComponent<Text>();
        text.text = newString;
    }
}
