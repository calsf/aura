using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Enter gameobject to exit/complete level

public class CompleteLevel : MonoBehaviour
{
    /* A level may unlock 0 or more levels upon completion
    /* Set to the indexes of level(s) to unlock (BE AWARE INDEXES DO NOT INCLUDE SCENES SUCH AS LEVELSELECTION SO THEY DO NOT MATCH PERFECTLY WITH BUILD SETTINGS) */
    [SerializeField]
    int[] levelToUnlock;

    SaveData saveData;
    LevelManager lvlManager;

    // Start is called before the first frame update
    void Start()
    {
        saveData = GameObject.FindGameObjectWithTag("SaveData").GetComponent<SaveData>();
        lvlManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            // UpdateGold, add gold gained from the level to the total saved gold and then save new value
            saveData.UpdateGold(saveData.Gold + lvlManager.Gold);

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
        yield return new WaitForSeconds(3f);

        AsyncOperation op = SceneManager.LoadSceneAsync(0);
        while (!op.isDone)
        {
            yield return null;
        }
    }
}
