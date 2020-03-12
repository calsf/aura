using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Damage number pop up behavior for when player damages enemy

public class DamageNumber : MonoBehaviour {
    TextMeshPro text;
    Rigidbody2D rb;
    float ySpeed;
    float xSpeed;
    float lastSign;

	// Use this for initialization
	void Start () {
        lastSign = Random.Range(0, 2) > 0 ? 1 : -1;
        ySpeed = 2f;
        xSpeed = Random.Range(2, 5) * lastSign;
        text = GetComponent<TextMeshPro>();
        rb = GetComponent<Rigidbody2D>();
	}
	
	void FixedUpdate () {
        //Move number up, eventually moves downward
        rb.velocity = new Vector2(xSpeed, ySpeed-=.3f);
	}

    void OnEnable()
    {
        if (text != null)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 1f);
        }

        ySpeed = 2f;
        lastSign = -lastSign;
        xSpeed = Random.Range(1, 2) * lastSign;
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
