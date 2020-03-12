using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Display current level gold count in text

public class GoldCounter : MonoBehaviour
{
    LevelManager levelManager;
    Text count;

    void Awake()
    {
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        count = GetComponent<Text>();
    }

    void Update()
    {
        count.text = levelManager.Gold.ToString();
    }
}
