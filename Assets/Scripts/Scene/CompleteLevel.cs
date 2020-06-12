using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Enter gameobject to exit/complete level

public class CompleteLevel : MonoBehaviour
{
    /* A level may unlock 0 or more levels upon completion
    /* Set to the indexes of level(s) to unlock (BE AWARE INDEXES DO NOT INCLUDE SCENES SUCH AS LEVELSELECTION SO THEY DO NOT MATCH PERFECTLY WITH BUILD SETTINGS) */
    [SerializeField]
    int[] levelToUnlock;

    [SerializeField]
    GameObject lvlCompleteScreen;
    [SerializeField]
    Text goldEarnedTxt;
    int goldEarned;
    [SerializeField]
    Text totalGoldTxt;
    int totalGold;
    int maxGold = 999999999;    // Cannot have more total gold than maxGold

    SaveData saveData;
    LevelManager lvlManager;
    bool hasEntered = false;
    CameraControl cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraControl>();
        lvlCompleteScreen.SetActive(false);
        totalGold = SaveLoadManager.LoadGold();
        saveData = GameObject.FindGameObjectWithTag("SaveData").GetComponent<SaveData>();
        lvlManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !hasEntered)
        {
            cam.FollowPlayer = false;
            hasEntered = true;
            other.gameObject.GetComponent<PlayerAuraControl>().AuraOff();   // Turn player aura off
            other.gameObject.SetActive(false);  // Turn player off

            // UpdateGold, add gold gained from the level to the total saved gold and then save new value
            int newTotal = saveData.Gold + lvlManager.Gold;
            
            // Do not exceed max total gold cap
            if (newTotal > maxGold)
            {
                newTotal = maxGold;
            }

            // Save new total gold
            saveData.UpdateGold(newTotal);

            // Unlock next level(s) and save
            foreach (int i in levelToUnlock)
            {
                saveData.UnlockLevel(i);
            }

            StartCoroutine(Leave());
        }
    }

    // Return to level select
    IEnumerator Leave()
    {
        // Display level complete and gold earned/total gold
        lvlCompleteScreen.SetActive(true);
        goldEarned = lvlManager.Gold;
        totalGoldTxt.text = totalGold.ToString();
        goldEarnedTxt.text = goldEarned.ToString();
        yield return new WaitForSeconds(1f);

        int targetTotal = totalGold + goldEarned;   // New total gold
        // Start coin counting sound effect
        SoundManager.SoundInstance.PlayStoppableSound("CoinCount");
        // Decrease gold earned and add to total gold, updating text display each time
        while (goldEarned > 0)
        {
            // Decrease rate
            int multiplier = goldEarned.ToString().Length - 1;
            multiplier = multiplier <= 2 ? 1 : multiplier;

            goldEarned -= ((int)Mathf.Pow(10, multiplier));
            totalGold += ((int) Mathf.Pow(10, multiplier));

            // Do not exceed max total gold cap
            if (totalGold > maxGold)
            {
                totalGold = maxGold;
            }

            // Make sure to not go over target total or go below 0 gold earned
            totalGoldTxt.text = totalGold > targetTotal ? targetTotal.ToString() : totalGold.ToString();
            goldEarnedTxt.text = goldEarned < 0 ? "0" : goldEarned.ToString();

            yield return null;
        }

        totalGoldTxt.text = targetTotal.ToString();

        // Stop coin counting sound effect and play end sound
        SoundManager.SoundInstance.StopSound("CoinCount");
        SoundManager.SoundInstance.PlaySound("CoinCountEnd");


        // Once done, wait a bit and load back to level select
        yield return new WaitForSeconds(2.5f);

        LoadLevel.LoadInstance.LoadScene(1);
    }
}
