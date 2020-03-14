using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

// Stores initially saved data from SaveLoadManager and will then save data if it is updated

[Serializable]
public class SaveData : MonoBehaviour
{
    bool[] auraUnlocked;
    bool[] lvlUnlocked;
    int[] equippedAuras;

    int gold;
    int health;

    public bool[] AuraUnlocked { get { return auraUnlocked; } }
    public bool[] LvlUnlocked { get { return lvlUnlocked; } }
    public int[] EquippedAuras { get { return equippedAuras; } }
    public int Gold { get { return gold; } }
    public int Health { get { return health; } }


    void Awake()
    {
        // Initialize values from saved file, if no saved file, Load calls will init to default values
        health = SaveLoadManager.LoadHealth();
        gold = SaveLoadManager.LoadGold();

        auraUnlocked = new bool[5];
        auraUnlocked = SaveLoadManager.LoadAuras();

        lvlUnlocked = new bool[2];
        lvlUnlocked = SaveLoadManager.LoadLvls();

        equippedAuras = new int[4];
        equippedAuras = SaveLoadManager.LoadEquipped();
    }

    // Unlock aura
    public void UnlockAura(int index)
    {
        auraUnlocked[index] = true;
        SaveLoadManager.SaveGame(this);
    }

    // Unlock lvl
    public void UnlockLevel(int index)
    {
        lvlUnlocked[index] = true;
        SaveLoadManager.SaveGame(this);
    }

    // Update equipped
    public void UpdateEquipped(int[] equipped)
    {
        equippedAuras = equipped;
        SaveLoadManager.SaveGame(this);
    }

    // Update saved gold
    public void UpdateGold(int gold)
    {
        this.gold = gold;
        SaveLoadManager.SaveGame(this);
    }

    // Update saved max health
    public void UpdateHealth(int health)
    {
        this.health = health;
        SaveLoadManager.SaveGame(this);
    }

}
