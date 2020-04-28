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
    GameObject player;
    [SerializeField]
    Animator menuAnim;

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

        player = GameObject.FindGameObjectWithTag("Player");
        menuNav = GetComponent<MenuNav>();
    }

    // Update is called once per frame
    void Update()
    {
        // Do not toggle menu if player is not active or if trying to rebind a key - if player is leaving and game is loading do not allow player to toggle menus
        if (!menuNav.IsLeaving && player.activeInHierarchy && !ControlsManager.ControlInstance.IsListen 
            && (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["MenuButton"]) || Input.GetKeyDown(ControlsManager.ControlInstance.Padbinds["MenuPad"])))
        {
            ToggleMenu();
            ToggleSubMenu(subMenus[0]);
            ControlsManager.ControlInstance.ResetSelectedKey();
            ControlsManager.ControlInstance.ResetSelectedPad();
        }

        // Prevent other coroutines from unpausing while in menu
        if (isMenu)
        {
            Time.timeScale = 0;
        }
    }

    //Toggle menu, pausing game
    public void ToggleMenu()
    {
        // Play animation for closing/opening menu and set properties - menu.alpha is set via animator
        if (menu.alpha > 0)
        {
            menuAnim.Play("MenuClose");

            //menu.alpha = 0;
            menu.blocksRaycasts = false;
            isMenu = false;
            Time.timeScale = 1;
        }
        else if (menu.alpha == 0)
        {
            menuAnim.Play("MenuOpen");
            SoundManager.SoundInstance.PlaySound("MenuPopUp");
            menu.blocksRaycasts = true;
            isMenu = true;
            Time.timeScale = 0;
        }

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
