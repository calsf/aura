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
        // Find the binded button to the key for keyboard and for gamepad
        string keybindName = ControlsManager.ControlInstance.Keybinds[navText.keyName].ToString();
        string padbindName = ControlsManager.ControlInstance.Padbinds[navText.padName].ToString();

        // Clean up the keybind/padbind string
        keybindName = ControlsManager.ControlInstance.CleanString(keybindName);
        padbindName = ControlsManager.ControlInstance.CleanString(padbindName);

        // Replace keybind and padbind in the navText's text with the binded keybind button and the binded padbind button
        text = GetComponent<Text>();
        text.text = navText.text.Replace(navText.keyName, keybindName).Replace(navText.padName, padbindName);
    }
}
