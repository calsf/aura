using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Awards gold amount and deactivates object when player walks into the collider

public class GoldPickup : MonoBehaviour
{
    [SerializeField]
    int goldValue;

    [SerializeField]
    GameObject goldPopupPrefab;
    GameObject goldPopup;
    TextMeshPro goldPopupText;
    LevelManager levelManager;

    void Awake()
    {
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();

        goldPopup = Instantiate(goldPopupPrefab, Vector3.zero, Quaternion.identity);
        goldPopupText = goldPopup.GetComponent<TextMeshPro>();
    }

    void Start()
    {
        // Deactivate gold popup after instantiation
        goldPopup.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            SoundManager.SoundInstance.PlaySound("JewelPickup");

            goldPopupText.text = "+" + goldValue + " Gold";
            goldPopup.transform.position = transform.position + (Vector3.up * .1f);
            goldPopup.SetActive(true);

            levelManager.Gold += goldValue;
            gameObject.SetActive(false);
        }
    }
}
