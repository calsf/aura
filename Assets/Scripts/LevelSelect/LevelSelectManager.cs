using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    [SerializeField]
    GameObject selectedHighlight;
    Text selectedName;
    [SerializeField]
    Sprite lockedSprite;

    bool[] lvlUnlocked;
    [SerializeField]
    Button[] firstFloor;
    Button[] firstFloorUnlocked;
    [SerializeField]
    Button[] secondFloor;
    Button[] secondFloorUnlocked;
    [SerializeField]
    Button[] thirdFloor;
    Button[] thirdFloorUnlocked;
    [SerializeField]
    Button[] fourthFloor;
    Button[] fourthFloorUnlocked;
    int totalFloors = 4;

    int floor;      // Current floor
    int selected;   // Selected zone in a floor
    bool axisDown;

    Button[] lvlButtons;    // All level buttons

    // Sprites for the selected/unselected buttons on each floor
    [SerializeField]
    Sprite[] firstSelected;
    [SerializeField]
    Sprite[] firstUnselected;
    [SerializeField]
    Sprite[] secondSelected;
    [SerializeField]
    Sprite[] secondUnselected;
    [SerializeField]
    Sprite[] thirdSelected;
    [SerializeField]
    Sprite[] thirdUnselected;
    [SerializeField]
    Sprite[] fourthSelected;
    [SerializeField]
    Sprite[] fourthUnselected;

    // Names of levels
    [SerializeField]
    string[] firstNames;
    [SerializeField]
    string[] secondNames;
    [SerializeField]
    string[] thirdNames;
    [SerializeField]
    string[] fourthNames;

    bool hasSelected; // Set true once an option is selected to disable controls

    [SerializeField]
    CanvasGroup menu;
    [SerializeField]
    MenuNav menuNav;

    void Awake()
    {
        Time.timeScale = 1; // Set timescale back to 1 in the case player leaves a level

        // Get levels unlocked from save and disable and enable level buttons accordingly
        lvlButtons = new Button[firstFloor.Length + secondFloor.Length + thirdFloor.Length + fourthFloor.Length];
        firstFloor.CopyTo(lvlButtons, 0);
        secondFloor.CopyTo(lvlButtons, firstFloor.Length);
        thirdFloor.CopyTo(lvlButtons, firstFloor.Length + secondFloor.Length);
        fourthFloor.CopyTo(lvlButtons, firstFloor.Length + secondFloor.Length + thirdFloor.Length);

        lvlUnlocked = SaveLoadManager.LoadLvls();
        for (int i = 0; i < lvlButtons.Length; i++)
        {
            if (!lvlUnlocked[i])
            {
                lvlButtons[i].gameObject.GetComponent<Image>().sprite = lockedSprite;
            }
            /*lvlButtons[i].gameObject.GetComponent<Image>().enabled = lvlUnlocked[i];   */ // Set image inactive/active to maintain horizontal layout
        }

        // Init unlocked floors
        InitUnlockedFloors(ref firstFloor, ref firstFloorUnlocked);
        InitUnlockedFloors(ref secondFloor, ref secondFloorUnlocked);
        InitUnlockedFloors(ref thirdFloor, ref thirdFloorUnlocked);
        InitUnlockedFloors(ref fourthFloor, ref fourthFloorUnlocked);

        selectedName = selectedHighlight.GetComponentInChildren<Text>();

        NavFloorHorizontal(0, 0);
    }

    void Update()
    {
        if (hasSelected || menu.alpha > 0)
        {
            return;
        }

        // Move selectedHighlight object to selected position
        Button[] curr;
        string[] levelNames;

        switch (floor)
        {
            case 0:
                curr = firstFloorUnlocked;
                levelNames = firstNames;
                break;
            case 1:
                curr = secondFloorUnlocked;
                levelNames = secondNames;
                break;
            case 2:
                curr = thirdFloorUnlocked;
                levelNames = thirdNames;
                break;
            case 3:
                curr = fourthFloorUnlocked;
                levelNames = fourthNames;
                break;
            default:
                curr = firstFloorUnlocked;
                levelNames = firstNames;
                break;
        }

        if (selected < curr.Length)
        {
            selectedHighlight.transform.position = curr[selected].gameObject.transform.position;
            selectedName.text = levelNames[selected];
        }

        // Navigate up and down, left and right
        if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["DownButton"]) || Input.GetAxisRaw("Vertical") == -1)
        {
            if (!axisDown)
            {
                axisDown = true;

                NavFloorVertical(floor, floor + 1);
            }
        }
        else if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["UpButton"]) || Input.GetAxisRaw("Vertical") == 1)
        {
            if (!axisDown)
            {
                axisDown = true;

                NavFloorVertical(floor, floor - 1);
            }
        }
        else if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["LeftButton"]) || Input.GetAxisRaw("Horizontal") == -1)
        {
            if (!axisDown)
            {
                axisDown = true;

                NavFloorHorizontal(selected, selected - 1);
            }
        }
        else if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["RightButton"]) || Input.GetAxisRaw("Horizontal") == 1)
        {
            if (!axisDown)
            {
                axisDown = true;

                NavFloorHorizontal(selected, selected + 1);
            }
        }
        else
        {
            axisDown = false;
        }

        // To click button, press jump
        if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["JumpButton"]) || Input.GetKeyDown(ControlsManager.ControlInstance.Padbinds["JumpPad"]))
        {
            menuNav.IsLeaving = true;   // Set leaving to true to avoid menu input

            SoundManager.SoundInstance.PlaySound("ButtonEnter");    // Play button sound

            curr[selected].GetComponent<Animator>().Play("Button");  // Play button anim
            curr[selected].onClick.Invoke();     // Invoke on click of selected button

            hasSelected = true;
        }
    }

    // Move up or down floors
    void NavFloorVertical(int lastFloor, int floor)
    {
        Button[] curr;
        Sprite[] unselectedBtns;
        switch (lastFloor)
        {
            case 0:
                curr = firstFloorUnlocked;
                unselectedBtns = firstUnselected;
                break;
            case 1:
                curr = secondFloorUnlocked;
                unselectedBtns = secondUnselected;
                break;
            case 2:
                curr = thirdFloorUnlocked;
                unselectedBtns = thirdUnselected;
                break;
            case 3:
                curr = fourthFloorUnlocked;
                unselectedBtns = fourthUnselected;
                break;
            default:
                curr = firstFloorUnlocked;
                unselectedBtns = firstUnselected;
                break;
        }

        // If next floor goes past last floor, wrap back to beginning. If goes before first floor, wrap to last floor
        if (floor > totalFloors - 1)
        {
            floor = 0;
        }
        else if (floor < 0)
        {
            floor = totalFloors - 1;
        }
        this.floor = floor;     // Move floors

        // If floor did not change, do not do anything and return
        if (floor == lastFloor)
        {
            return;
        }

        // Deselect current zone
        curr[selected].GetComponent<Image>().sprite = unselectedBtns[selected];

        // Reset to first zone in new floor
        NavFloorHorizontal(0, 0);

        // Only play sound if floor changed
        if (this.floor != lastFloor)
        {
            SoundManager.SoundInstance.PlaySound("ButtonNav");
        }
    }

    // Move to next/previous zone in current floor
    void NavFloorHorizontal(int lastButton, int button)
    {
        Button[] curr;
        Sprite[] unselectedBtns;
        Sprite[] selectedBtns;
        switch (floor)
        {
            case 0:
                curr = firstFloorUnlocked;
                unselectedBtns = firstUnselected;
                selectedBtns = firstSelected;
                break;
            case 1:
                curr = secondFloorUnlocked;
                unselectedBtns = secondUnselected;
                selectedBtns = secondSelected;
                break;
            case 2:
                curr = thirdFloorUnlocked;
                unselectedBtns = thirdUnselected;
                selectedBtns = thirdSelected;
                break;
            case 3:
                curr = fourthFloorUnlocked;
                unselectedBtns = fourthUnselected;
                selectedBtns = fourthSelected;
                break;
            default:
                curr = firstFloorUnlocked;
                unselectedBtns = firstUnselected;
                selectedBtns = firstSelected;
                break;
        }

        // If next button goes past last button, wrap back to beginning. If goes before first button, wrap to last button
        if (button > curr.Length - 1)
        {
            button = 0;
        }
        else if (button < 0)
        {
            button = curr.Length - 1;
        }
        selected = button;

        curr[lastButton].GetComponent<Image>().sprite = unselectedBtns[lastButton];
        curr[button].GetComponent<Image>().sprite = selectedBtns[button];


        // Only play sound if selection changed
        if (selected != lastButton)
        {
            SoundManager.SoundInstance.PlaySound("ButtonNav");
        }
    }


    // Initialize unlocked floors
    void InitUnlockedFloors(ref Button[] allFloors, ref Button[] unlockedFloors)
    {
        int size = 0;
        foreach (Button b in allFloors)
        {
            if (b.gameObject.GetComponent<Image>().sprite != lockedSprite)
            {
                size++;
            }
        }
        unlockedFloors = new Button[size];

        size = 0;
        foreach (Button b in allFloors)
        {
            if (b.gameObject.GetComponent<Image>().sprite != lockedSprite)
            {
                unlockedFloors[size] = b;
                size++;
            }
        }

        // If size is 0, there is nothing unlocked on floor, remove from total floors
        if (size == 0)
        {
            totalFloors--;
        }

    }
}
