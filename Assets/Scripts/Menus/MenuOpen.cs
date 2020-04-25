using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach to the menu object that plays menu open animation

public class MenuOpen : MonoBehaviour
{
    [SerializeField]
    MenuNav menuNav;

    // Initialize submenu once menu open animation has finished using animation event
    public void OnMenuOpened()
    {
        menuNav.NavMenu(0);
    }
}
