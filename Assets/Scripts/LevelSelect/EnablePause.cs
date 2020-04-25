using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enable pausing / toggling menu AFTER fade in has finished
// Fade in must be using normal update mode so if game gets paused, it would be paused mid fade in
// Attach to the fade in object and with an animation event, call EnableMenuToggle at end of animation

public class EnablePause : MonoBehaviour
{
    [SerializeField]
    MenuManager menu;

    void Awake()
    {
        if (menu != null)
        {
            menu.enabled = false;
        }
    }

    public void EnableMenuToggle()
    {
        if (menu != null)
        {
            menu.enabled = true;
        }
    }
}
