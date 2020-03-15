using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player hearts indicate how much health player has

public class PlayerHearts : MonoBehaviour
{
    [SerializeField]
    PlayerHP player;
    [SerializeField]
    GameObject fillParent;
    [SerializeField]
    GameObject backParent;

    Transform[] heartsFill;
    Transform[] heartsBack;

    // Start is called before the first frame update
    void Start()
    {
        heartsFill = fillParent.GetComponentsInChildren<Transform>();
        heartsBack = backParent.GetComponentsInChildren<Transform>();

        // Initialize hearts based on player's max health
        UpdateMaxHearts();
    }

    void OnEnable()
    {
        if (player != null)
        {
            player.OnHealthChange.AddListener(UpdateHearts);
        }
    }

    void Disable()
    {
        if (player != null)
        {
            player.OnHealthChange.RemoveListener(UpdateHearts);
        }
    }

    // Update hearts filled based on player's current health
    public void UpdateHearts()
    {
        int health = player.CurrentHP;

        for (int i = 0; i < heartsFill.Length; i++)
        {
            if (i <= player.CurrentHP)
            {
                heartsFill[i].gameObject.SetActive(true);
            }
            else
            {
                heartsFill[i].gameObject.SetActive(false);
            }
        }
    }

    // Update max hearts (used for shop display, not during gameplay)
    public void UpdateMaxHearts()
    {
        int playerMaxHP = SaveLoadManager.LoadHealth();
        for (int i = 0; i < heartsFill.Length; i++)
        {
            if (i <= playerMaxHP)
            {
                heartsBack[i].gameObject.SetActive(true);
                heartsFill[i].gameObject.SetActive(true);
            }
            else
            {
                heartsBack[i].gameObject.SetActive(false);
                heartsFill[i].gameObject.SetActive(false);
            }
        }
    }
}
