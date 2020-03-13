using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    CanvasGroup menu;   //Parent menu
    [SerializeField]
    CanvasGroup[] subMenus;

    MenuNav menuNav;

    bool isMenu = false;
    static MenuManager menuManager;

    public static MenuManager MenuInstance { get { return menuManager; } }
    public bool IsMenu { get { return isMenu; } }

    void Awake()
    {
        //Singleton
        if (menuManager == null)
        {
            menuManager = this;
        }
        else
        {
            Destroy(menuManager.gameObject);
            menuManager = this;
        }

        menuNav = GetComponent<MenuNav>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ControlsManager.ControlInstance.IsListen && (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["MenuButton"]) || Input.GetKeyDown(ControlsManager.ControlInstance.Padbinds["MenuPad"])))
        {
            ToggleMenu();
            ToggleSubMenu(subMenus[0]);
            ControlsManager.ControlInstance.ResetSelectedKey();
            ControlsManager.ControlInstance.ResetSelectedPad();
        }

        // Prevent other coroutines from unpausing while in menu
        if (menu.alpha > 0 && Time.timeScale > 0)
        {
            Time.timeScale = 0;
        }
    }

    //Toggle menu, pausing game
    public void ToggleMenu()
    {
        menu.alpha = menu.alpha > 0 ? 0 : .7f;
        menu.blocksRaycasts = menu.blocksRaycasts ? false : true;
        Time.timeScale = menu.alpha > 0 ? 0 : 1;
        isMenu = menu.alpha > 0 ? true : false;
        
        //Set menu defaults on menu open
        if (isMenu)
        {
            menuNav.NavMenu(0);
        }
    }

    //For submenu nav, toggle corresponding submenu's CanvasGroup with button
    public void ToggleSubMenu(CanvasGroup sub)
    {
        // Turn off all submenus
        foreach (CanvasGroup sb in subMenus)
        {
            sb.alpha = 0;
            sb.blocksRaycasts = false;
        }

        // Turn on selected submenu
        sub.alpha = 1;
        sub.blocksRaycasts = true;
    }
}
