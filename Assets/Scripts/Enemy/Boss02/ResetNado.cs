using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Reset sprite renderer color and sprite upon disabling so it is re-enabled with same properties
// Reset collider as well

public class ResetNado : MonoBehaviour
{
    SpriteRenderer sr;
    Color origColor;
    Sprite origSprite;
    Collider2D coll;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        origColor = sr.color;
        origSprite = sr.sprite;
        coll = GetComponent<Collider2D>();
    }

    void OnDisable()
    {
        sr.color = origColor;
        sr.sprite = origSprite;

        if (coll != null)
        {
            coll.enabled = false;
        }
    }
}
