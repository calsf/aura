using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Saves player position and health on activation and returns player back to original position and health on deactivation

public class AstralAura : MonoBehaviour
{
    [SerializeField]
    GameObject auraProjection;
    [SerializeField]
    SpriteRenderer playerProjection;

    PlayerHP playerHP;
    SpriteRenderer playerSprite;
    Vector2 savedPos;
    int savedHealth;

    CameraControl cam;

    void Awake()
    {
        playerHP = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHP>();
        playerSprite = GameObject.FindGameObjectWithTag("Player").GetComponent<SpriteRenderer>();

        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraControl>();
    }

    // Save position and health of player unless player is dead
    void OnEnable()
    {
        savedHealth = playerHP.CurrentHP;
        savedPos = playerHP.transform.position;

        // Activate the aura/player clones
        auraProjection.transform.position = playerHP.transform.position;
        playerProjection.transform.position = playerHP.transform.position;
        playerProjection.sprite = playerSprite.sprite;
        playerProjection.transform.localScale = playerSprite.transform.localScale;

        auraProjection.SetActive(true);
        playerProjection.gameObject.SetActive(true);
    }

    // Reset position and health unless player is dead
    void OnDisable()
    {
        if (playerHP.CurrentHP > 0 && savedHealth > 0)
        {
            playerHP.CurrentHP = savedHealth;
            playerHP.OnHealthChange.Invoke();

            playerHP.transform.position = savedPos;
            cam.MoveCamInstant();       // Move camera position directly to position
            cam.OnFarTeleport.Invoke(); // Invoke OnFarTeleport to move background instantly if necessary
        }

        // Always deactivate aura/player clones
        auraProjection.SetActive(false);
        playerProjection.gameObject.SetActive(false);
    }
}
