﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Display total saved gold (not the gained gold in current level but saved gold overall)

public class SavedGold : MonoBehaviour
{
    [SerializeField]
    Text gold;

    // Start is called before the first frame update
    void Start()
    {
        UpdateGold();
    }

    // Set text display to show gold from saved data
    public void UpdateGold()
    {
        gold.text = SaveLoadManager.LoadGold().ToString();
    }
}
