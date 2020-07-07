using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnhanceNav : MonoBehaviour
{
    [SerializeField]
    Scrollbar scroll;
    [SerializeField]
    Button[] auraInvent;            // Includes all auras
    Button[] unlockedAuraInvent;    // Filtered aura select buttons so only can navigate through unlocked auras
    [SerializeField]
    RectTransform auraList;

    int selectedBtn;                // Selected btn index to navigate the filtered aura select buttons
    int selectedAuraNumber;         // Same as aura.auraNumber, used to determine which aura to enhance

    [SerializeField]
    Sprite selectedSprite;
    [SerializeField]
    Sprite unselectedSprite;

    bool axisDown;  // To treat axis input as key down

    [SerializeField]
    EnhanceManager enhanceManager;

    public int SelectedBtn { get { return selectedBtn; } }


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

        // Adjust the size of aura list so it scrolls when there are enough unlocked auras
        // 5 auras can be displayed without scrolling so with more than 5 auras, increase aura list size so that navigating through auras scrolls
        // Otherwise, will not increase aura list size and leave the aura list with no scrolling
        int extraAuras = unlockedAuraInvent.Length - 5;
        if (extraAuras > 0)
        {
            float extraSpace = extraAuras == 1 ? 40 : 45;
            auraList.anchoredPosition = new Vector2(auraList.anchoredPosition.x, auraList.anchoredPosition.y - extraSpace);
            auraList.sizeDelta = new Vector2(auraList.sizeDelta.x, auraList.sizeDelta.y + extraSpace);
            extraAuras--;
        }

        while (extraAuras > 0)
        {
            auraList.anchoredPosition = new Vector2(auraList.anchoredPosition.x, auraList.anchoredPosition.y - 36f);
            auraList.sizeDelta = new Vector2(auraList.sizeDelta.x, auraList.sizeDelta.y + 36);
            extraAuras--;
        }

        NavAuras(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // Do not allow input while leaving scene
        if (enhanceManager.IsLeaving)
        {
            return;
        }

        if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["DownButton"]) || Input.GetAxisRaw("Vertical") == -1)
        {
            if (!axisDown)
            {
                axisDown = true;
                NavAuras(selectedBtn, selectedBtn + 1);

                SoundManager.SoundInstance.PlaySound("ButtonNav");
            }
        }
        else if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["UpButton"]) || Input.GetAxisRaw("Vertical") == 1)
        {
            if (!axisDown)
            {
                axisDown = true;
                NavAuras(selectedBtn, selectedBtn - 1);

                SoundManager.SoundInstance.PlaySound("ButtonNav");
            }
        }
        else
        {
            axisDown = false; // Reset axisDown
        }

        // Pressing jump will attempt to enhance the aura of selectedAuraNumber
        if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["JumpButton"]) || Input.GetKeyDown(ControlsManager.ControlInstance.Padbinds["JumpPad"]))
        {
            // Uses selectedAuraNumber which is index of all auras (aura.auraNumber), selectedBtn is only the index of each button which is not accurate since
            // it only includes auras unlocked
            enhanceManager.BuyEnhance(selectedAuraNumber);
        }
    }

    // Navigate aura buttons
    public void NavAuras(int lastButton, int button)
    {
        // If next button goes past last button, wrap back to beginning. If goes before first button, wrap to last button
        if (button > unlockedAuraInvent.Length - 1)
        {
            button = 0;
        }
        else if (button < 0)
        {
            button = unlockedAuraInvent.Length - 1;
        }
        selectedBtn = button;


        scroll.value = (1f - ((float)button / (unlockedAuraInvent.Length)));

        // Change sprite of button, reset last one
        unlockedAuraInvent[lastButton].GetComponent<Image>().sprite = unselectedSprite;
        unlockedAuraInvent[button].GetComponent<Image>().sprite = selectedSprite;

        // Invoke onClick to display enhance aura info and set selected aura
        unlockedAuraInvent[selectedBtn].onClick.Invoke();
    }

    // OnClick button, set the selected aura number equal to the corresponding button's aura
    public void SetSelectedAura(int index)
    {
        selectedAuraNumber = index;
    }

}
