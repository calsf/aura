using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectManager : MonoBehaviour
{
    bool[] lvlUnlocked;
    Button[] lvlButtons;

    [SerializeField]
    GameObject buttonParent;

    void Awake()
    {
        Time.timeScale = 1; // Set timescale back to 1 in the case player leaves a level
        lvlButtons = buttonParent.GetComponentsInChildren<Button>();

        // Get levels unlocked from save and disable and enable level buttons accordingly
        lvlUnlocked = SaveLoadManager.LoadLvls();
        for (int i = 0; i < lvlButtons.Length; i++)
        {
            lvlButtons[i].gameObject.SetActive(lvlUnlocked[i]);
        }
    }
}
