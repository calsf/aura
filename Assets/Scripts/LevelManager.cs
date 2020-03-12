using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles current level information
// Keeps track of the gold player has obtained from current playthrough of level

public class LevelManager : MonoBehaviour
{
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
