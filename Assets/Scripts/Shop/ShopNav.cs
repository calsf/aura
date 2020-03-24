using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopNav : MonoBehaviour
{
    [SerializeField]
    Button[] itemBtn;
    [SerializeField]
    Image[] infoImg;    // Item info image
    Image[] itemImg;    // Button image
    int selected;

    bool axisDown;  // To treat axis input as key down

    [SerializeField]
    Color selectedColor;
    [SerializeField]
    Color unselectedColor;

    // Start is called before the first frame update
    void Start()
    {
        itemImg = new Image[itemBtn.Length];
        for (int i = 0; i < itemBtn.Length; i++)
        {
            itemImg[i] = itemBtn[i].GetComponent<Image>();
        }

        NavShop(0);
    }

    // Update is called once per frame
    void Update()
    {
        // Navigate to left and right items in shop
        if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["LeftButton"]) || Input.GetAxisRaw("Horizontal") == -1)
        {
            if (!axisDown)
            {
                axisDown = true;
                NavShop(selected - 1);
            }
        }
        else if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["RightButton"]) || Input.GetAxisRaw("Horizontal") == 1)
        {
            if (!axisDown)
            {
                axisDown = true;
                NavShop(selected + 1);
            }
        }
        else
        {
            axisDown = false; // Reset axisDown
        }

        // To click button, press jump
        if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["JumpButton"]) || Input.GetKeyDown(ControlsManager.ControlInstance.Padbinds["JumpPad"]))
        {
            // Only invoke onClick if the button of selected index is interactable
            if (itemBtn[selected].interactable)
            {
                itemBtn[selected].onClick.Invoke();
            }
        }
    }

    // Navigate the shop items
    public void NavShop(int index)
    {
        // Reset image colors
        for (int i = 0; i < itemImg.Length; i++)
        {
            // Only reset colors of enabled items in shop
            if (itemBtn[i].interactable)
            {
                itemImg[i].color = unselectedColor;
                infoImg[i].color = unselectedColor;
            }
        }

        // If selected is greater than index, means we wanted to move to the left to the prev item
        bool isLeft = selected > index;

        // Check if everything has been disabled, if so, do not select anything and return
        bool allDisabled = true;
        foreach (Button b in itemBtn)
        {
            if (b.interactable)
            {
                allDisabled = false;
            }
        }
        if (allDisabled)
        {
            return;
        }

        // Wrap navigation around if at end/beginning
        if (index > itemBtn.Length - 1)
        {
            index = 0;
        }
        else if (index < 0)
        {
            index = itemBtn.Length - 1;
        }

        // Skip navigation to enabled items only
        while (!itemBtn[index].interactable)
        {
            if (isLeft)
            {
                index--;
                if (index > itemBtn.Length - 1)
                {
                    index = 0;
                }
                else if (index < 0)
                {
                    index = itemBtn.Length - 1;
                }
            }
            else
            {
                index++;
                if (index > itemBtn.Length - 1)
                {
                    index = 0;
                }
                else if (index < 0)
                {
                    index = itemBtn.Length - 1;
                }
            }
        }

        itemImg[index].color = selectedColor;
        infoImg[index].color = selectedColor;
        selected = index;
    }

}
