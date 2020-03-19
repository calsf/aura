using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuNav : MonoBehaviour
{
    // Submenu navigation buttons
    [SerializeField]
    Button[] nav;
    Image[] navImg;

    [SerializeField]
    Scrollbar[] scroll;     // Each submenu's scrollbar
    [SerializeField]
    Button[] auraInvent;            // First submenu buttons as set by editor, includes all auras
    Button[] unlockedAuraInvent;    // Filtered aura select buttons so only can navigate through unlocked auras
    [SerializeField]
    Button[] controls;      // Second submenu buttons
    [SerializeField]
    Button[] settings;      // Third submenu buttons
    [SerializeField]
    Button[] leave;         // Fourth submenu buttons
    Button[][] subButtons;  // Store each submenu of buttons into an array

    [SerializeField]
    Button equipBtn;        // Equip button to give control to slotButtons
    [SerializeField]
    Button[] slotButtons;   // Slots to determine where to equip aura
    int selectedSlot;
    bool isEquipping;

    [SerializeField]
    Color selectedColor;
    [SerializeField]
    Color unselectedColor;
    int selectedSub;        // Index of selected submenu
    int selectedSubBtn;     // Index of selected submenu's button

    bool axisDown;  // To treat axis input as key down
    bool isLeaving; // Disable actions if trying to leave and loading to return back

    // Start is called before the first frame update
    void Start()
    {
        // Get size of unlocked
        int count = 0;
        foreach (Button b in auraInvent)
        {
            if (b.gameObject.activeInHierarchy)
            {
                count++;
            }
        }

        // Add unlocked auras to unlockedAuraInvent
        unlockedAuraInvent = new Button[count];
        int index = 0;
        foreach (Button b in auraInvent)
        {
            if (b.gameObject.activeInHierarchy)
            {
                unlockedAuraInvent[index] = b;
                index++;
            }
        }

        subButtons = new Button[4][];
        subButtons[0] = unlockedAuraInvent;
        subButtons[1] = controls;
        subButtons[2] = settings;
        subButtons[3] = leave;

        navImg = new Image[nav.Length];
        for (int i = 0; i < nav.Length; i++)
        {
            navImg[i] = nav[i].GetComponent<Image>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Do not check for menu input if listening for new keybind, if menu is not open, or if player is leaving
        if (isLeaving || !MenuManager.MenuInstance.IsMenu || ControlsManager.ControlInstance.IsListen)
        {
            return;
        }

        // isEquipping checks if player is currently trying to equip an aura, if so move control from menu nav/submenu to the slot select buttons
        if (!isEquipping)
        {
            if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["DownButton"]) || Input.GetAxisRaw("Vertical") == -1)
            {
                if (!axisDown)
                {
                    axisDown = true;
                    NavSub(selectedSubBtn, selectedSubBtn + 1);
                }
            }
            else if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["UpButton"]) || Input.GetAxisRaw("Vertical") == 1)
            {
                if (!axisDown)
                {
                    axisDown = true;
                    NavSub(selectedSubBtn, selectedSubBtn - 1);
                }
            }
            else if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["LeftButton"]) || Input.GetAxisRaw("Horizontal") == -1)
            {
                if (!axisDown)
                {
                    axisDown = true;
                    NavMenu(selectedSub - 1);
                }
            }
            else if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["RightButton"]) || Input.GetAxisRaw("Horizontal") == 1)
            {
                if (!axisDown)
                {
                    axisDown = true;
                    NavMenu(selectedSub + 1);
                }
            }
            else
            {
                axisDown = false; // Reset axisDown
            }
        }
        else
        {
            if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["LeftButton"]) || Input.GetAxisRaw("Horizontal") == -1)
            {
                if (!axisDown)
                {
                    axisDown = true;
                    SelectSlot(selectedSlot, selectedSlot - 1);
                }
            }
            else if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["RightButton"]) || Input.GetAxisRaw("Horizontal") == 1)
            {
                if (!axisDown)
                {
                    axisDown = true;
                    SelectSlot(selectedSlot, selectedSlot + 1);
                }
            }
            else
            {
                axisDown = false; // Reset axisDown
            }
        }

        // To click button, press jump
        if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["JumpButton"]) || Input.GetKeyDown(ControlsManager.ControlInstance.Padbinds["JumpPad"]))
        {
            // Act based on current submenu
            switch(selectedSub)
            {
                case 0:     // Aura inventory submenu will equip selected aura
                    if (isEquipping)
                    {
                        slotButtons[selectedSlot].onClick.Invoke(); // Equip aura into currently selected slot and return control to submenus/nav
                        isEquipping = false;
                    }
                    else
                    {
                        equipBtn.onClick.Invoke();  // Give control over to slotButtons
                        isEquipping = true;
                        SelectSlot(0, 0);
                    }
                    break;
                case 1:     // Controls submenu will change controls
                    subButtons[selectedSub][selectedSubBtn].onClick.Invoke();
                    ControlsManager.ControlInstance.IsListen = false; // Wait for next input if trying to rebind key
                    break;
                case 2:     // Settings submenu
                    subButtons[selectedSub][selectedSubBtn].onClick.Invoke();
                    break;
                case 3:     // Leave submenu set isLeaving true to avoid more actions while loading back IMPORTANT: LOADING INTO A NEW SCENE WHILE IN MENUS WILL STILL HAVE TIMESCALE = 0 SO NEED TO RESET IN NEW SCENE
                    isLeaving = true;
                    subButtons[selectedSub][selectedSubBtn].onClick.Invoke();
                    break;
            }
            
        }
        else if (Input.GetKeyUp(ControlsManager.ControlInstance.Keybinds["JumpButton"]) || Input.GetKeyUp(ControlsManager.ControlInstance.Padbinds["JumpPad"]))
        {
            // Wait for next input if trying to rebind key
            // Key rebindings should be all buttons before Other options in controls
            // subButtons[selectedSub].Length - (# of Option buttons)
            if (selectedSub == 1 && selectedSubBtn < subButtons[selectedSub].Length - 3)
            {
                ControlsManager.ControlInstance.IsListen = true;
            }
        }
    }

    // Navigate through the slots to equip an aura
    public void SelectSlot(int lastSlot, int slot)
    {
        if (slot > slotButtons.Length - 1)
        {
            slot = 0;
        }
        else if (slot < 0)
        {
            slot = slotButtons.Length - 1;
        }
        selectedSlot = slot;
        slotButtons[lastSlot].GetComponent<Image>().color = unselectedColor;
        slotButtons[slot].GetComponent<Image>().color = selectedColor;
    }

    // Navigate submenu buttons
    public void NavSub(int lastButton, int button)
    {
        // If next button goes past last button in submenu, wrap back to beginning. If goes before first button, wrap to last button in submenu
        if (button > subButtons[selectedSub].Length - 1)
        {
            button = 0;
        }
        else if (button < 0)
        {
            button = subButtons[selectedSub].Length - 1;
        }
        selectedSubBtn = button;
        // If submenu has a scrollbar, move scrollbar while navigating buttons
        if (scroll.Length - 1 >= selectedSub)
        {
            scroll[selectedSub].value = (1f - ((float)button / (subButtons[selectedSub].Length)));
        }

        // Highlight new current button, reset last one
        subButtons[selectedSub][lastButton].GetComponent<Image>().color = unselectedColor;
        subButtons[selectedSub][button].GetComponent<Image>().color = selectedColor;

        // If in aura invent, invoke onClick to display aura info (Pressing jump will activate EquipButton's onClick)
        if (selectedSub == 0)
        {
            subButtons[selectedSub][button].onClick.Invoke();
        }
    }

    // Navigate the submenus
    public void NavMenu(int sub)
    {
        NavSub(selectedSubBtn, 0);   // Reset current submenu button to first button

        foreach (Image i in navImg)
        {
            i.color = unselectedColor;
        }

        if (sub > nav.Length - 1)
        {
            sub = 0;
        }
        else if (sub < 0)
        {
            sub = nav.Length - 1;
        }

        navImg[sub].color = selectedColor;
        nav[sub].onClick.Invoke();
        selectedSub = sub;           // Set new submenu index
        NavSub(selectedSubBtn, 0);   // Sets new submenu button to first button
    }
}
