using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach to any object that should undergo a color change/flash but is not part of the main enemy object getting hit
// Call Flash() on OnDamaged event on the main enemy with EnemyDefaults

public class ColorChange : MonoBehaviour
{
    SpriteRenderer spriteRender;

    bool isColorChange;
    float startTime;
    float delay = .08f;

    void Awake()
    {
        spriteRender = GetComponent<SpriteRenderer>();
    }

    // For flashing alpha of sprite renderer when enemy is hit, called on LateUpdate to override any animations that change spriterender's color
    void LateUpdate()
    {
        if (isColorChange)
        {
            spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b, .4f);
            if (Time.time > startTime + delay)
            {
                spriteRender.color = new Color(spriteRender.color.r, spriteRender.color.g, spriteRender.color.b, 1f);
                isColorChange = false;
            }
        }
    }

    public void Flash()
    {
        // If not currently changing color, start changing color and set start time to current time
        if (!isColorChange)
        {
            isColorChange = true;
            startTime = Time.time;
        }
    }
}
