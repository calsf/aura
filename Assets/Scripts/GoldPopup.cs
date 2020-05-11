using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Gold pop up pops up vertically

public class GoldPopup : MonoBehaviour
{
    TextMeshPro text;
    Rigidbody2D rb;
    Vector3 dist;

    // Use this for initialization
    void Awake()
    {
        text = GetComponent<TextMeshPro>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Don't move if paused
        if (Time.timeScale > 0)
        {
            dist += Time.deltaTime * (Vector3.up * .02f);
            transform.position += dist;
        }
    }

    void OnEnable()
    {
        dist = Vector3.zero;
        if (text != null)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
        }

        StartCoroutine(Fade());
    }

    //Fade out
    IEnumerator Fade()
    {
        yield return new WaitForSeconds(.5f);
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
