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
        for (int i = 0; i < heartsFill.Length; i++)
        {
            if (i <= player.MaxHP)
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

    void OnEnable()
    {
        player.OnHealthChange.AddListener(UpdateHearts);
    }

    void Disable()
    {
        player.OnHealthChange.RemoveListener(UpdateHearts);
    }

    // Update hearts filled based on player's current health
    void UpdateHearts()
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
}
