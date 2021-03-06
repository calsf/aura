﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Damage number pop up behavior for when player damages enemy

public class DamageNumber : MonoBehaviour {
    TextMeshPro text;
    Rigidbody2D rb;
    float ySpeed;

    // Use this for initialization
    void Awake()
    {
        ySpeed = 2f;
        text = GetComponent<TextMeshPro>();
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        //Move number up, then drop down
        rb.velocity = new Vector2(0, ySpeed -= .3f);
    }

    void OnEnable()
    {
        ySpeed = 2f;
        if (text != null)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
        }

        StartCoroutine(Fade());
    }

    //Fade out
    IEnumerator Fade()
    {
        yield return new WaitForSeconds(.4f);
        text.color = new Color(text.color.r, text.color.g, text.color.b, .8f);
        yield return new WaitForSeconds(.05f);
        text.color = new Color(text.color.r, text.color.g, text.color.b, .6f);
        yield return new WaitForSeconds(.05f);
        text.color = new Color(text.color.r, text.color.g, text.color.b, .4f);
        yield return new WaitForSeconds(.05f);
        text.color = new Color(text.color.r, text.color.g, text.color.b, .2f);
        yield return new WaitForSeconds(.05f);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
        gameObject.SetActive(false);
    }
}
