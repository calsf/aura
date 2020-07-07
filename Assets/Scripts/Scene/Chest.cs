using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Opens chest and awards gold amount when player walks into the collider

public class Chest : MonoBehaviour
{
    [SerializeField]
    int goldValue;

    [SerializeField]
    GameObject goldPopupPrefab;
    GameObject goldPopup;
    TextMeshPro goldPopupText;
    LevelManager levelManager;

    bool hasOpened;
    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        goldPopup = Instantiate(goldPopupPrefab, Vector3.zero, Quaternion.identity);
        goldPopupText = goldPopup.GetComponent<TextMeshPro>();
    }

    void Start()
    {
        // Deactive gold popup after
        goldPopup.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !hasOpened)
        {
            hasOpened = true;
            anim.Play("ChestOpen");

            SoundManager.SoundInstance.PlaySound("OpenChest");

            goldPopupText.text = "+" + goldValue + " Gold";
            goldPopup.transform.position = transform.position + (Vector3.up * .1f);
            goldPopup.SetActive(true);

            levelManager.Gold += goldValue;
        }
    }
}
