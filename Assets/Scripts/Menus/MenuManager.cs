using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    CanvasGroup menu;   //Parent menu

    [SerializeField]
    CanvasGroup[] subMenus;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            toggleMenu();
            toggleSubMenu(subMenus[0]);
            ControlsManager.controlManager.ResetSelectedKey();
            ControlsManager.controlManager.ResetSelectedPad();
        }

        // Prevent other coroutines from unpausing while in menu
        if (menu.alpha > 0 && Time.timeScale > 0)
        {
            Time.timeScale = 0;
        }
    }

    //Toggle menu, pausing game
    public void toggleMenu()
    {
        menu.alpha = menu.alpha > 0 ? 0 : .7f;
        menu.blocksRaycasts = menu.blocksRaycasts ? false : true;
        Time.timeScale = menu.alpha > 0 ? 0 : 1;
    }

    //For submenu nav, toggle corresponding submenu's CanvasGroup with button
    public void toggleSubMenu(CanvasGroup sub)
    {
        foreach (CanvasGroup sb in subMenus)
        {
            sb.alpha = 0;
            sb.blocksRaycasts = false;
        }
        sub.alpha = 1;
        sub.blocksRaycasts = true;
    }
}
