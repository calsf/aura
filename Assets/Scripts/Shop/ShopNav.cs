using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopNav : MonoBehaviour
{
    [SerializeField]
    Button[] itemBtn;
    Image[] itemImg;
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
            // Only invoke onClick if the button is interactable
            if (itemBtn[selected].interactable)
            {
                itemBtn[selected].onClick.Invoke();
            }
        }
    }

    // Navigate the shop items
    public void NavShop(int index)
    {
        foreach (Image i in itemImg)
        {
            i.color = unselectedColor;
        }

        if (index > itemBtn.Length - 1)
        {
            index = 0;
        }
        else if (index < 0)
        {
            index = itemBtn.Length - 1;
        }

        itemImg[index].color = selectedColor;
        selected = index;
    }
}
