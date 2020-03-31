using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Handles current level information
// Keeps track of the gold player has obtained from current playthrough of level

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    GameObject goldPopupPrefab;
    GameObject goldPopup;
    TextMeshPro goldPopupText;
    

    [SerializeField]
    GameObject player;
    [SerializeField]
    Transform spawnPoint;

    PlayerHP playerHP;

    int gold = 0;

    public int Gold { get { return gold; } set { gold = value; } }

    void Awake()
    {
        playerHP = player.GetComponent<PlayerHP>();
        player.transform.position = spawnPoint.position;

        // Player has a gold pop up for when they die and lose half gold
        goldPopup = Instantiate(goldPopupPrefab, Vector3.zero, Quaternion.identity);
        goldPopupText = goldPopup.GetComponent<TextMeshPro>();
        goldPopupText.color = new Color(1, 0, 0, 1);    // Change gold popup text color to red since will only pop up for losing gold
    }

    void Start()
    {
        goldPopup.SetActive(false);
    }

    void OnEnable()
    {
        playerHP.OnDeath.AddListener(GoldCut);
        playerHP.OnDeath.AddListener(SpawnPlayer);
    }

    void OnDisable()
    {
        playerHP.OnDeath.RemoveListener(GoldCut);
        playerHP.OnDeath.RemoveListener(SpawnPlayer);
    }

    // Lose half gold OnDeath
    void GoldCut()
    {
        // Show gold popup for gold lost with y offset above player
        goldPopup.transform.position = player.transform.position + (Vector3.up * .1f);
        goldPopupText.text = "-" + gold/2 + " Gold";
        goldPopup.SetActive(true);

        gold /= 2;
    }

    // Start coroutine to respawn player
    void SpawnPlayer()
    {
        StartCoroutine(Respawn());
    }

    // Move player to designated spawn location - PlayerHP handles resetting player values
    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(playerHP.RespawnDelay);
        player.transform.position = spawnPoint.position;
    }
}
