using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class StartOptions : MonoBehaviour
{
    [SerializeField]
    LoadLevel loadLevel;

    [SerializeField]
    CanvasGroup buttonCanvas;
    [SerializeField]
    Button[] optionsDefault;    // All options
    Button[] options;           // Options that can be navigated and selected
    Text[] optionsText;

    [SerializeField]
    GameObject warning;
    [SerializeField]
    Button[] warningOptions;
    Text[] warningOptionsText;

    int selected;
    int selectedWarning;

    bool axisDown;  // To treat axis input as key down

    // Sprites for the selected/unselected button
    [SerializeField]
    Sprite selectedSprite;
    [SerializeField]
    Sprite unselectedSprite;

    // Color for the selected/unselected options text
    [SerializeField]
    Color unselectedColor;
    [SerializeField]
    Color selectedColor;

    void Awake()
    {
        // First option should be continue, if no save file, set interactable to false
        if (!File.Exists(Application.persistentDataPath + "/aura.sav"))
        {
            optionsDefault[0].interactable = false;
        }

        // Initialize options that can navigated
        int index = 0;

        // Init size of options and text
        foreach (Button b in optionsDefault)
        {
            if (b.interactable)
            {
                index++;
            }
        }
        options = new Button[index];
        optionsText = new Text[index];

        // Init option buttons and text
        index = 0;
        foreach (Button b in optionsDefault)
        {
            if (b.interactable)
            {
                options[index] = b;
                optionsText[index] = b.GetComponentInChildren<Text>();
                index++;
            }
        }

        warningOptionsText = new Text[warningOptions.Length];
        for (int i = 0; i < warningOptions.Length; i++)
        {
            warningOptionsText[i] = warningOptions[i].GetComponentInChildren<Text>();
        }

        NavOptions(0, 0);   // Init selected option to first option
    }

    void Update()
    {
        // Wait until fade in complete before accepting input
        if (buttonCanvas.alpha != 1)
        {
            return;
        }

        // Navigate options up and down
        if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["DownButton"]) || Input.GetAxisRaw("Vertical") == -1)
        {
            if (!axisDown)
            {
                axisDown = true;

                // Navigate start options or new game warnings
                if (warning.activeInHierarchy)
                {
                    NavWarnings(selectedWarning, selectedWarning + 1);
                }
                else
                {
                    NavOptions(selected, selected + 1);
                }
            }
        }
        else if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["UpButton"]) || Input.GetAxisRaw("Vertical") == 1)
        {
            if (!axisDown)
            {
                axisDown = true;

                // Navigate start options or new game warnings
                if (warning.activeInHierarchy)
                {
                    NavWarnings(selectedWarning, selectedWarning - 1);
                }
                else
                {
                    NavOptions(selected, selected - 1);
                }
            }
        }
        else
        {
            axisDown = false;
        }

        // To click button, press jump
        if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["JumpButton"]) || Input.GetKeyDown(ControlsManager.ControlInstance.Padbinds["JumpPad"]))
        {
            SoundManager.SoundInstance.PlaySound("ButtonEnter");    // Play button sound

            if (warning.activeInHierarchy)
            {
                warningOptions[selectedWarning].GetComponent<Animator>().Play("Button");  // Play button anim
                warningOptions[selectedWarning].onClick.Invoke();     // Invoke on click of selected button
            }
            else
            {
                options[selected].GetComponent<Animator>().Play("Button");  // Play button anim
                options[selected].onClick.Invoke();     // Invoke on click of selected button
            }
            
        }
    }

    // Navigate start options
    void NavOptions(int lastButton, int button)
    {
        // If next button goes past last button options, wrap back to beginning. If goes before first button, wrap to last button
        if (button > options.Length - 1)
        {
            button = 0;
        }
        else if (button < 0)
        {
            button = options.Length - 1;
        }
        selected = button;

        options[lastButton].GetComponent<Image>().sprite = unselectedSprite;
        optionsText[lastButton].color = unselectedColor;
        options[button].GetComponent<Image>().sprite = selectedSprite;
        optionsText[button].color = selectedColor;

        // Only play sound if selection changed
        if (selected != lastButton)
        {
            SoundManager.SoundInstance.PlaySound("ButtonNav");
        }
    }

    // Navigate warning options
    void NavWarnings(int lastButton, int button)
    {
        // If next button goes past last button options, wrap back to beginning. If goes before first button, wrap to last button
        if (button > warningOptions.Length - 1)
        {
            button = 0;
        }
        else if (button < 0)
        {
            button = warningOptions.Length - 1;
        }
        selectedWarning = button;

        warningOptions[lastButton].GetComponent<Image>().sprite = unselectedSprite;
        warningOptionsText[lastButton].color = unselectedColor;
        warningOptions[button].GetComponent<Image>().sprite = selectedSprite;
        warningOptionsText[button].color = selectedColor;

        // Only play sound if selection changed
        if (selectedWarning != lastButton)
        {
            SoundManager.SoundInstance.PlaySound("ButtonNav");
        }
    }

    // Prompt to overwrite/save data to new default data
    public void PromptNewGame()
    {
        // Warn player if there exists a save file already and return
        if (File.Exists(Application.persistentDataPath + "/aura.sav"))
        {
            warning.SetActive(true);    // Set warning screen active, navigation turns over to warnings while this is active
            NavWarnings(0, 0);          // Init selected warning option to first option
            return;
        }

        // If no file, skips prompt and confirms new game
        ConfirmNewGame();
    }

    // Overwrite/save data to new default data
    public void ConfirmNewGame()
    {
        SaveData saveData = gameObject.AddComponent<SaveData>();
        SaveLoadManager.SaveGame(saveData.LoadNew());

        // Load into zone select
        loadLevel.LoadScene(1);
    }

    // Cancel new game and exit new game warnings screen
    public void CancelNewGame()
    {
        warning.SetActive(false);
    }

    // Quit application
    public void Quit()
    {
        Application.Quit();
    }
}
