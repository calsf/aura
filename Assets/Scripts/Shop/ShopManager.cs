using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    // Parallel arrays for shop auras
    // NOTE: auraIndex is the corresponding index to total auras (to match the unlockedAuras from SaveLoadManager) E.g: first button auraSelection may have auraIndex of 5 because the button is for aura index 5 of unlockedAuras
    [SerializeField]
    Button[] auraSelection;     // Button for each aura
    [SerializeField]
    GameObject[] auraPriceDisplay; // Containers for price display for each aura price, includes an image and text component in children
    [SerializeField]
    int[] auraIndex;            // Index of each aura in the whole array of auras (Reminder: index 0 is always base aura and will never be in shop)
    [SerializeField]
    int[] auraPrice;            // Price of each aura

    [SerializeField]
    Button health;              // Button to increase max health
    [SerializeField]
    GameObject healthPriceDisplay; // Container for price display for health price, includes an image and text component in children
    int baseHealthPrice = 100;
    int healthPrice;
    [SerializeField]
    PlayerHearts playerHearts;

    [SerializeField]
    LoadLevel load;     // To return back to level selection
    
    SaveData saveData;
    SavedGold savedGold;

    // Start is called before the first frame update
    void Start()
    {
        int currMaxHP = SaveLoadManager.LoadHealth();
        healthPrice = baseHealthPrice * currMaxHP;     // Increase price of max health increase with the current max HP

        saveData = GameObject.FindGameObjectWithTag("SaveData").GetComponent<SaveData>();
        savedGold = saveData.GetComponent<SavedGold>();
        UpdateShop();       // Set shop aura's on or off based on saved aura unlocks
        UpdateShopHealth(currMaxHP);     // Set max health increase off or no based on saved max health
    }

    void Update()
    {
        // On menu button, return back to level select scene
        if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["MenuButton"]) || Input.GetKeyDown(ControlsManager.ControlInstance.Padbinds["MenuPad"]))
        {
            load.LoadScene(0);
        }
    }

    // Button onClick to buy aura, if player has enough gold, unlock aura and update gold
    public void BuyAura(int index)
    {
        int gold = SaveLoadManager.LoadGold();
        if (gold >= auraPrice[index])
        {
            int indexOfSelection = auraIndex[index];

            saveData.UpdateGold(gold - auraPrice[index]);
            savedGold.UpdateGold();

            saveData.UnlockAura(indexOfSelection);      // Unlock the aura index from overall auras list, not from this shop's aura list
            UpdateShop();
        }
    }

    // Button onClick to buy max health increase, if player has enough gold, increase max health by 1
    public void BuyHealth()
    {
        int gold = SaveLoadManager.LoadGold();
        int newHealth = SaveLoadManager.LoadHealth() + 1;
        if (gold >= healthPrice)
        {
            saveData.UpdateGold(gold - healthPrice);        // Update gold first
            savedGold.UpdateGold();
            healthPrice = baseHealthPrice * newHealth;     // Update new health price, increase price of max health increase with the current max HP

            saveData.UpdateHealth(newHealth);   // Save new max health
            UpdateShopHealth(newHealth);
            playerHearts.UpdateMaxHearts();     // Update max hearts display to match updated new max health
        }
    }

    // Disable health from shop if at max hp
    public void UpdateShopHealth(int newHealth)
    {
        healthPriceDisplay.GetComponentInChildren<Text>().text = healthPrice.ToString();     // Update health price text display

        if (newHealth >= SaveLoadManager.CapHP)
        {
            // Set button interactable false, change alpha on price display's image
            health.interactable = false;
            healthPriceDisplay.GetComponentInChildren<Text>().text = "Maxed";
            Color color = healthPriceDisplay.GetComponentInChildren<Image>().color;
            healthPriceDisplay.GetComponentInChildren<Image>().color = new Color(color.r, color.g, color.b, .5f);
        }
    }

    // Disable aura from shop if already unlocked
    public void UpdateShop()
    {
        bool[] auraUnlocked = SaveLoadManager.LoadAuras();

        for (int i = 0; i < auraSelection.Length; i++)
        {
            auraPriceDisplay[i].GetComponentInChildren<Text>().text = auraPrice[i].ToString();   // Update each aura's price text display

            int indexOfSelection = auraIndex[i];    // Index from total auras to match auraUnlocked
            if (auraUnlocked[indexOfSelection])
            {
                // Set button interactable false, change alpha on price display's image
                auraSelection[i].interactable = false;
                auraPriceDisplay[i].GetComponentInChildren<Text>().text = "Owned";
                Color color = auraPriceDisplay[i].GetComponentInChildren<Image>().color;
                auraPriceDisplay[i].GetComponentInChildren<Image>().color = new Color(color.r, color.g, color.b, .5f);
            }
        }
    }
}
