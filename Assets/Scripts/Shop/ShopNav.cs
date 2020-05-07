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

    // Sprites for the item name/cost button
    [SerializeField]
    Sprite selectedSprite;
    [SerializeField]
    Sprite unselectedSprite;

    // Sprites for the item info
    [SerializeField]
    Sprite selectedSpriteInfo;
    [SerializeField]
    Sprite unselectedSpriteInfo;

    public Sprite UnselectedSprite { get { return unselectedSprite; } }
    public Sprite UnselectedSpriteInfo { get { return unselectedSpriteInfo; } }
    public int Selected { get { return selected; } }

    // Initialize item buttons, initialize nav with NavShop(0) call in ShopManager after it has checked for disabled items
    void Awake()
    {
        itemImg = new Image[itemBtn.Length];
        for (int i = 0; i < itemBtn.Length; i++)
        {
            itemImg[i] = itemBtn[i].GetComponent<Image>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Navigate to left and right items in shop
        if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["LeftButton"]) || Input.GetAxisRaw("Horizontal") == -1)
        {
            if (!axisDown)
            {
                int temp = selected;

                axisDown = true;
                NavShop(selected - 1);

                // Only play nav sound if selected item changed
                if (selected != temp)
                {
                    SoundManager.SoundInstance.PlaySound("ButtonNav");
                }
                
            }
        }
        else if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["RightButton"]) || Input.GetAxisRaw("Horizontal") == 1)
        {
            if (!axisDown)
            {
                int temp = selected;

                axisDown = true;
                NavShop(selected + 1);

                // Only play nav sound if selected item changed
                if (selected != temp)
                {
                    SoundManager.SoundInstance.PlaySound("ButtonNav");
                }
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
           itemImg[i].sprite = unselectedSprite;
           infoImg[i].sprite = unselectedSpriteInfo;
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

        itemImg[index].sprite = selectedSprite;
        infoImg[index].sprite = selectedSpriteInfo;
        selected = index;
    }

}
