using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    CanvasGroup menu;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            toggleMenu();
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
}
