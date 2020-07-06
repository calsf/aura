using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

// Stores initially saved data from SaveLoadManager and will then save data if it is updated

[Serializable]
public class SaveData : MonoBehaviour
{
    int[] extraDmg;
    bool[] auraUnlocked;
    bool[] lvlUnlocked;
    int[] equippedAuras;

    int gold;
    int health;

    bool visitedBoss2;

    public int[] ExtraDmg { get { return extraDmg; } }
    public bool[] AuraUnlocked { get { return auraUnlocked; } }
    public bool[] LvlUnlocked { get { return lvlUnlocked; } }
    public int[] EquippedAuras { get { return equippedAuras; } }
    public int Gold { get { return gold; } }
    public int Health { get { return health; } }
    public bool VisitedBoss2 { get { return visitedBoss2; } }

    void Awake()
    {
        // Initialize values from saved file, if no saved file, Load calls will init to default values
        health = SaveLoadManager.LoadHealth();
        gold = SaveLoadManager.LoadGold();

        extraDmg = SaveLoadManager.LoadExtraDmg();

        auraUnlocked = SaveLoadManager.LoadAuras();

        lvlUnlocked = SaveLoadManager.LoadLvls();

        equippedAuras = SaveLoadManager.LoadEquipped();

        visitedBoss2 = SaveLoadManager.LoadVisitedBoss2();
    }

    // Resets data to new data
    public SaveData LoadNew()
    {
        health = SaveLoadManager.LoadHealthNew();
        gold = SaveLoadManager.LoadGoldNew();

        extraDmg = SaveLoadManager.LoadExtraDmgNew();

        auraUnlocked = SaveLoadManager.LoadAurasNew();

        lvlUnlocked = SaveLoadManager.LoadLvlsNew();

        equippedAuras = SaveLoadManager.LoadEquippedNew();

        visitedBoss2 = SaveLoadManager.LoadVisitedBoss2New();

        return this;
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

    // Update if has visited boss 2
    public void UpdateVisitedBoss2(bool visitedBoss2)
    {
        this.visitedBoss2 = visitedBoss2;
        SaveLoadManager.SaveGame(this);
    }

    // Update extra dmg
    public void UpdateExtraDmg(int index)
    {
        extraDmg[index] += 1;
        SaveLoadManager.SaveGame(this);
    }

}
