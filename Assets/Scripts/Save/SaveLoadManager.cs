using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

// Used to load saved data values into game and also to save new data values from game using SaveData

[Serializable]
public class Save
{
    bool[] auraUnlocked;    // Loaded by UnlockedAuras to get unlocked auras - disables and enables buttons for selecting auras based on unlock/lock
    bool[] lvlUnlocked;     // Loaded by LevelSelectManager to get unlocked levels - disables and enables buttons for selecting levels // Saved in CompleteLevel
    int[] equippedAuras;    // Loaded by PlayerAuraControl to get equipped auras // Saved in AuraSelect when equipped auras are updated
    int gold;               // Loaded by SaveGold to get saved gold // Saved in CompleteLevel
    int health;             // Loaded by PlayerHP to set player's max hp

    bool visitedBoss2;      // Loaded by LevelSelectManager, updated in Boss 2, if true, will replace shopkeeper 2 with an alternate shop

    public bool[] AuraUnlocked { get { return auraUnlocked; } }
    public bool[] LvlUnlocked { get { return lvlUnlocked; } }
    public int[] EquippedAuras { get { return equippedAuras; } }
    public int Gold { get { return gold; } }
    public int Health { get { return health; } }

    public bool VisitedBoss2 { get { return visitedBoss2; } }

    // Init Save with SaveData data
    public Save(SaveData data)
    {
        auraUnlocked = new bool[data.AuraUnlocked.Length];
        lvlUnlocked = new bool[data.LvlUnlocked.Length];
        equippedAuras = new int[data.EquippedAuras.Length];
        gold = data.Gold;
        health = data.Health;
        visitedBoss2 = data.VisitedBoss2;

        for (int i = 0; i < auraUnlocked.Length; i++)
        {
            auraUnlocked[i] = data.AuraUnlocked[i];
        }

        for (int i = 0; i < lvlUnlocked.Length; i++)
        {
            lvlUnlocked[i] = data.LvlUnlocked[i];
        }

        for (int i = 0; i < equippedAuras.Length; i++)
        {
            equippedAuras[i] = data.EquippedAuras[i];
        }
    }
}

public static class SaveLoadManager
{
    static int capHP = 10; // Max HP obtainable, maxHP may be lower but will never exceed cap ** Only set, will not be saved **
    public static int CapHP { get { return capHP; } }

    // Manually save/overwrite data
    public static void SaveGame(SaveData data)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = new FileStream(Application.persistentDataPath + "/aura.sav", FileMode.Create);

        // Init Save saveData with new data that is passed in as argument
        Save saveData = new Save(data);

        // Write new data to file
        bf.Serialize(file, saveData);
        file.Close();
    }

    /********* LOAD FROM FILE, LOADS NEW IF DOESN'T EXIST ****************/
    // Load auras
    public static bool[] LoadAuras()
    {
        // Load from file if it exists
        if (File.Exists(Application.persistentDataPath + "/aura.sav"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = new FileStream(Application.persistentDataPath + "/aura.sav", FileMode.Open);

            Save saveData = (Save)bf.Deserialize(file);
            file.Close();

            return saveData.AuraUnlocked;
        }
        else
        {
            // If no save file exists, start fresh with only 1 aura unlocked
            return LoadAurasNew();
        }
    }

    // Load unlocked levels
    public static bool[] LoadLvls()
    {
        // Load from file if it exists
        if (File.Exists(Application.persistentDataPath + "/aura.sav"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = new FileStream(Application.persistentDataPath + "/aura.sav", FileMode.Open);

            Save saveData = (Save)bf.Deserialize(file);
            file.Close();

            return saveData.LvlUnlocked;
        }
        else
        {
            // If no save file exists, start fresh (levels do not include level select)
            return LoadLvlsNew();
        }
    }

    // Load equipped auras
    public static int[] LoadEquipped()
    {
        // Load from file if it exists
        if (File.Exists(Application.persistentDataPath + "/aura.sav"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = new FileStream(Application.persistentDataPath + "/aura.sav", FileMode.Open);

            Save saveData = (Save)bf.Deserialize(file);
            file.Close();

            return saveData.EquippedAuras;
        }
        else
        {
            // If no save file exists, start fresh
            return LoadEquippedNew();
        }
    }

    // Load health
    public static int LoadHealth()
    {
        // Load from file if it exists
        if (File.Exists(Application.persistentDataPath + "/aura.sav"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = new FileStream(Application.persistentDataPath + "/aura.sav", FileMode.Open);

            Save saveData = (Save)bf.Deserialize(file);
            file.Close();

            return saveData.Health;
        }
        else
        {
            // Return default health if no file exists
            return LoadHealthNew();
        }
    }

    // Load gold
    public static int LoadGold()
    {
        // Load from file if it exists
        if (File.Exists(Application.persistentDataPath + "/aura.sav"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = new FileStream(Application.persistentDataPath + "/aura.sav", FileMode.Open);

            Save saveData = (Save)bf.Deserialize(file);
            file.Close();

            return saveData.Gold;
        }
        else
        {
            // Return default of 0 gold if no file exists
            return LoadGoldNew();
        }
    }

    // Load if has visited boss 2
    public static bool LoadVisitedBoss2()
    {
        // Load from file if it exists
        if (File.Exists(Application.persistentDataPath + "/aura.sav"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = new FileStream(Application.persistentDataPath + "/aura.sav", FileMode.Open);

            Save saveData = (Save)bf.Deserialize(file);
            file.Close();

            return saveData.VisitedBoss2;
        }
        else
        {
            // If no save file exists, start fresh has not visited boss 2
            return LoadVisitedBoss2New();
        }
    }

    /******* Load NEW data ****************/
    // Load auras
    public static bool[] LoadAurasNew()
    {
        // Start fresh with only 1 aura unlocked
        bool[] defaultAuras = new bool[]
        {
            true, true, true, true, true, true, true, true, true, true, true, true, true, true
        };

        return defaultAuras;
    }

    // Load unlocked levels
    public static bool[] LoadLvlsNew()
    {
        // Load new levels (levels do not include start screen or level select, minus 2 from the scene index)
        bool[] defaultLvls = new bool[]
        {
            true, true, true, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false
        };

        return defaultLvls; 
    }

    // Load equipped auras
    public static int[] LoadEquippedNew()
    {
        // If no save file exists, start fresh
        int[] defaultEquip = new int[]
        {
            0, 0, 0, 0
        };

        return defaultEquip;
        
    }

    // Load health
    public static int LoadHealthNew()
    {
        // Return default health
        return 3;
    }

    // Load gold
    public static int LoadGoldNew()
    {
        // Return default of 0 gold
        return 0;
    }

    // Load if has visited boss 2
    public static bool LoadVisitedBoss2New()
    {
        return false;
    }


}

