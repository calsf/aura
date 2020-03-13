using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

// Manages controls, including keyboard and gamepad bindings and other control related settings

public class ControlsManager : MonoBehaviour
{
    GameObject selectedPad; // Selected gamepad keybinding
    GameObject selectedKey; // Selected keyboard keybinding

    [SerializeField]
    GameObject[] keyButtons;
    [SerializeField]
    GameObject[] padButtons;
    [SerializeField]
    GameObject[] otherButtons;
    [SerializeField]
    Color unselectedColor; //Unselected button color
    Color selectedColor = Color.green; //Selected color

    public UnityEvent OnControlChange;
    public static ControlsManager controlManager;
    Dictionary<string, KeyCode> keybinds;
    Dictionary<string, KeyCode> padbinds;

    public Dictionary<string, KeyCode> Keybinds { get { return keybinds; } }
    public Dictionary<string, KeyCode> Padbinds { get { return padbinds; } }

    void Awake()
    {
        //Singleton
        if (controlManager == null)
        {
            controlManager = this;
        }
        else
        {
            Destroy(controlManager.gameObject);
            controlManager = this;
        }

        //Load keybinds, set to default if no player pref
        keybinds = new Dictionary<string, KeyCode>();
        padbinds = new Dictionary<string, KeyCode>();
        SetControls();
    }

    // Update is called once per frame
    void Update()
    {
        // Gamepad binding
        if (selectedPad != null)
        {
            if (Input.GetKeyDown(KeyCode.Joystick1Button0))
            {
                BindKey(selectedPad.name, KeyCode.Joystick1Button0, padbinds);
                ResetSelectedPad();
                SaveKeys();
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button1))
            {
                BindKey(selectedPad.name, KeyCode.Joystick1Button1, padbinds);
                ResetSelectedPad();
                SaveKeys();
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button2))
            {
                BindKey(selectedPad.name, KeyCode.Joystick1Button2, padbinds);
                ResetSelectedPad();
                SaveKeys();
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button3))
            {
                BindKey(selectedPad.name, KeyCode.Joystick1Button3, padbinds);
                ResetSelectedPad();
                SaveKeys();
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button4))
            {
                BindKey(selectedPad.name, KeyCode.Joystick1Button4, padbinds);
                ResetSelectedPad();
                SaveKeys();
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button5))
            {
                BindKey(selectedPad.name, KeyCode.Joystick1Button5, padbinds);
                ResetSelectedPad();
                SaveKeys();
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button6))
            {
                BindKey(selectedPad.name, KeyCode.Joystick1Button6, padbinds);
                ResetSelectedPad();
                SaveKeys();
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button7))
            {
                BindKey(selectedPad.name, KeyCode.Joystick1Button7, padbinds);
                ResetSelectedPad();
                SaveKeys();
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button8))
            {
                BindKey(selectedPad.name, KeyCode.Joystick1Button8, padbinds);
                ResetSelectedPad();
                SaveKeys();
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button9))
            {
                BindKey(selectedPad.name, KeyCode.Joystick1Button9, padbinds);
                ResetSelectedPad();
                SaveKeys();
            }
            else if (Input.GetKeyDown(KeyCode.Joystick1Button10))
            {
                BindKey(selectedPad.name, KeyCode.Joystick1Button10, padbinds);
                ResetSelectedPad();
                SaveKeys();
            }
        }
    }

    //Bind key name with selected keycode
    void BindKey(string name, KeyCode keycode, Dictionary<string, KeyCode> binds)
    {
        // Set buttons to corresponding keys/pad buttons to update display
        GameObject[] btns = null;
        if (binds == keybinds)
        {
            btns = keyButtons;
        }
        else if (binds == padbinds)
        {
            btns = padButtons;
        }
        else
        {
            return;
        }

        //If key is already bound, remove old key
        if (binds.ContainsValue(keycode))
        {
            string old = binds.FirstOrDefault(x => x.Value == keycode).Key;
            binds[old] = KeyCode.None;
            UpdateDisplay(old, KeyCode.None, btns);
        }
        binds[name] = keycode;
        UpdateDisplay(name, keycode, btns);
    }

    //Update the displayed keybinds in menu, looks for the corresponding button name with the key name
    void UpdateDisplay(string keyName, KeyCode keycode, GameObject[] buttons)
    {
        Text temp = Array.Find(buttons, x => x.name == keyName).GetComponentInChildren<Text>();
        
        //Add space before uppercase letters and before a number, clean up alpha and joystick keycode names
        string str = CleanString(keycode.ToString());
        StringBuilder sb = new StringBuilder();
        foreach (char c in str)
        {
            if (char.IsUpper(c) || char.IsDigit(c))
            {
                sb.Append(' ');
            }
            sb.Append(c);
        }

        temp.text = sb.ToString();
    }

    //While a key is selected, listen for the next key press and save
    void OnGUI()
    {
        // Keyboard keybinding
        if (selectedKey != null)
        {
            Event e = Event.current;
            if ((e.isKey && e.keyCode != KeyCode.Escape))
            {
                BindKey(selectedKey.name, e.keyCode, keybinds);
                ResetSelectedKey();
                SaveKeys();
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    BindKey(selectedKey.name, KeyCode.LeftShift, keybinds);
                    ResetSelectedKey();
                    SaveKeys();
                }
                else if (Input.GetKey(KeyCode.RightShift))
                {
                    BindKey(selectedKey.name, KeyCode.RightShift, keybinds);
                    ResetSelectedKey();
                    SaveKeys();
                }
            }
        }
    }

    //OnClick function for each button that selects and highlights the button (For keyboard)
    public void ChangeKey(GameObject clicked)
    {
        if (selectedKey != null)
        {
            //Reset selected key if same key is clicked, else switch selected keys
            if (selectedKey == clicked)
            {
                ResetSelectedKey();
            }
            else
            {
                selectedKey.GetComponent<Image>().color = unselectedColor;
                selectedKey = clicked;
                clicked.GetComponent<Image>().color = selectedColor;
            }
            return;
        }
        selectedKey = clicked;
        clicked.GetComponent<Image>().color = Color.green;
    }

    //OnClick function for each button that selects and highlights the button (For gamepad)
    public void ChangePad(GameObject clicked)
    {
        if (selectedPad != null)
        {
            //Reset selected key if same key is clicked, else switch selected keys
            if (selectedPad == clicked)
            {
                ResetSelectedPad();
            }
            else
            {
                selectedPad.GetComponent<Image>().color = unselectedColor;
                selectedPad = clicked;
                clicked.GetComponent<Image>().color = selectedColor;
            }
            return;
        }
        selectedPad = clicked;
        clicked.GetComponent<Image>().color = Color.green;
    }

    //Save keybind settings
    public void SaveKeys()
    {
        foreach (var key in keybinds)
        {
            PlayerPrefs.SetString(key.Key, key.Value.ToString());
        }
        PlayerPrefs.Save();
    }

    //Reset the selected key
    public void ResetSelectedKey()
    {
        if (selectedKey != null)
        {
            selectedKey.GetComponent<Image>().color = unselectedColor;
            selectedKey = null;
        }
    }

    //Reset the selected gamepad
    public void ResetSelectedPad()
    {
        if (selectedPad != null)
        {
            selectedPad.GetComponent<Image>().color = unselectedColor;
            selectedPad = null;
        }
    }


    //Reset controls display
    public void SetControls()
    {
        // Button objects in editor must match names here (ButtonName, KeyCode)
        BindKey("UpButton", (KeyCode)(Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("UpButton", "W"))), keybinds);
        BindKey("LeftButton", (KeyCode)(Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("LeftButton", "A"))), keybinds);
        BindKey("DownButton", (KeyCode)(Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("DownButton", "S"))), keybinds);
        BindKey("RightButton", (KeyCode)(Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("RightButton", "D"))), keybinds);
        BindKey("DashButton", (KeyCode)(Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("DashButton", "LeftShift"))), keybinds);
        BindKey("JumpButton", (KeyCode)(Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("JumpButton", "Space"))), keybinds);
        BindKey("Aura1Button", (KeyCode)(Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Aura1Button", "U"))), keybinds);
        BindKey("Aura2Button", (KeyCode)(Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Aura2Button", "I"))), keybinds);
        BindKey("Aura3Button", (KeyCode)(Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Aura3Button", "O"))), keybinds);
        BindKey("Aura4Button", (KeyCode)(Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Aura4Button", "P"))), keybinds);

        //Gamepad
        BindKey("DashPad", (KeyCode)(Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("DashPad", "Joystick1Button7"))), padbinds);
        BindKey("JumpPad", (KeyCode)(Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("JumpPad", "Joystick1Button6"))), padbinds);
        BindKey("Aura1Pad", (KeyCode)(Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Aura1Pad", "Joystick1Button1"))), padbinds);
        BindKey("Aura2Pad", (KeyCode)(Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Aura2Pad", "Joystick1Button0"))), padbinds);
        BindKey("Aura3Pad", (KeyCode)(Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Aura3Pad", "Joystick1Button3"))), padbinds);
        BindKey("Aura4Pad", (KeyCode)(Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Aura4Pad", "Joystick1Button2"))), padbinds);

        //Other options
        UpdateToggle("ToggleAura");
        UpdateToggle("UpJump");
    }

    // Toggle control option on and off
    public void ToggleControl(string option)
    {
        string currSetting = PlayerPrefs.GetString(option);
        PlayerPrefs.SetString(option, currSetting == "On" ? "Off" : "On");
        UpdateToggle(option);
    }

    //Update toggled display - button name must match PlayerPref string/option
    void UpdateToggle(string option)
    {
        Text temp = Array.Find(otherButtons, x => x.name == option).GetComponentInChildren<Text>();
        temp.text = PlayerPrefs.GetString(option) == "On" ? "On" : "Off";
        OnControlChange.Invoke();
    }

    //Reset player pref, updates the displayed settings to default after
    public void ResetPref()
    {
        PlayerPrefs.DeleteAll();
        SetControls();
    }

    // Clean up KeyCode string
    string CleanString(string str)
    {
        // Remove "Alpha" from keycodes
        string cleanStr = str.Replace("Alpha", "");

        // Look for joystick keycodes and replace
        for(int i = 0; i < 20; i++)
        {
            if (cleanStr.Contains($"Joystick1Button{i}"))
            {
                switch (i)
                {
                    case 0:
                        return "Square";
                    case 1:
                        return "X";
                    case 2:
                        return "Circle";
                    case 3:
                        return "Triangle";
                    case 4:
                        return "L1";
                    case 5:
                        return "R1";
                    case 6:
                        return "L2";
                    case 7:
                        return "R2";
                    default:
                        return $"Gamepad {i}";
                }
            }
        }

        return cleanStr;
    }
}
