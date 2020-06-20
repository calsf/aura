using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enable pausing / toggling menu and also enable the player AFTER fade in has finished
// Fade in must be using normal update mode so if game gets paused, it would be paused mid fade in
// Attach to the fade in object and with an animation event, call EnableMenuToggle at end of animation

public class EnablePause : MonoBehaviour
{
    [SerializeField]
    MenuManager menu;

    GameObject player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (menu != null)
        {
            menu.enabled = false;
        }
    }
    
    // Disable player at the start
    void Start()
    {
        player.SetActive(false);
    }

    // Re-enable menu toggle and player after fade
    public void EnableMenuToggle()
    {
        if (menu != null)
        {
            menu.enabled = true;
        }

        EnablePlayer();
    }

    // Re-enable player
    public void EnablePlayer()
    {
        player.SetActive(true);
    }
}
