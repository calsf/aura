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
    Aura[] auras;               // Aura scriptable object

    int[] auraIndex;            // Index of each aura in the whole array of auras (Reminder: index 0 is always base aura and will never be in shop)
    int[] auraPrice;            // Price of each aura

    [SerializeField]
    [TextArea(3, 10)]
    string[] auraBought;        // Chat text when aura is bought, specific to each aura
    [SerializeField]
    [TextArea(3, 10)]
    string auraFail;          // Chat text when aura cannot be bought, generic for each aura

    [SerializeField]
    Button health;              // Button to increase max health
    [SerializeField]
    GameObject healthPriceDisplay; // Container for price display for health price, includes an image and text component in children

    int baseHealthPrice = 1000;
    int healthPrice;

    [SerializeField]
    PlayerHearts playerHearts;

    [SerializeField]
    [TextArea(3, 10)]
    string healthMaxed;         // Chat text when health increase is bought and maxed
    [SerializeField]
    [TextArea(3, 10)]
    string healthBought;        // Chat text when a health increase is bought
    [SerializeField]
    [TextArea(3, 10)]
    string healthFail;          // Chat text when health increase cannot be bought
    
    SaveData saveData;
    SavedGold savedGold;
    ShopChat shopChat;

    [SerializeField]
    ShopNav shopNav;

    bool isLeaving = false;

    public bool IsLeaving { get { return isLeaving; } }

    void Awake()
    {
        // Init aura prices and aura index number
        auraPrice = new int[auras.Length];
        auraIndex = new int[auras.Length];
        for (int i = 0; i < auras.Length; i++)
        {
            auraPrice[i] = auras[i].price;
            auraIndex[i] = auras[i].auraNumber;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        int currMaxHP = SaveLoadManager.LoadHealth();
        healthPrice = baseHealthPrice * currMaxHP;     // Increase price of max health increase with the current max HP

        shopChat = GetComponent<ShopChat>();
        saveData = GameObject.FindGameObjectWithTag("SaveData").GetComponent<SaveData>();
        savedGold = saveData.GetComponent<SavedGold>();
        UpdateShop();       // Set shop aura's on or off based on saved aura unlocks
        UpdateShopHealth(currMaxHP);     // Set max health increase off or no based on saved max health

        shopNav.NavShop(0); // Set starting nav on item after updated for disabled items
    }

    void Update()
    {
        // Do not allow input while leaving scene
        if (isLeaving)
        {
            return;
        }

        // On menu button, return back to level select scene
        if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["MenuButton"]) || Input.GetKeyDown(ControlsManager.ControlInstance.Padbinds["MenuPad"]))
        {
            isLeaving = true;
            LoadLevel.LoadInstance.LoadScene(1);
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
            shopChat.SetChat(auraBought[index]);        // Show chat response to the aura bought

            // Move to next item
            shopNav.NavShop(shopNav.Selected + 1);

            SoundManager.SoundInstance.PlaySound("ShopBuySuccess");
        }
        else
        {
            shopChat.SetChat(auraFail);     // Show chat response if can't afford aura

            SoundManager.SoundInstance.PlaySound("ShopBuyFail");
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

            // Show chat response to buy
            if (newHealth >= SaveLoadManager.CapHP)
            {
                shopChat.SetChat(healthMaxed);

                // Move to next item
                shopNav.NavShop(shopNav.Selected + 1);
            }
            else
            {
                shopChat.SetChat(healthBought);
            }

            SoundManager.SoundInstance.PlaySound("ShopBuySuccess");
        }
        else
        {
            shopChat.SetChat(healthFail);     //Show chat response if not enough gold to buy

            SoundManager.SoundInstance.PlaySound("ShopBuyFail");
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
            healthPriceDisplay.GetComponentInChildren<Image>().color = new Color(.5f, .5f, .5f, 1f);

            // Change colors of all images and texts belonging to the item to be disabled
            Image[] auraImgs = health.GetComponentsInChildren<Image>();
            Text[] texts = health.GetComponentsInChildren<Text>();
            foreach (Image a in auraImgs)
            {
                a.color = new Color(.5f, .5f, .5f, 1);
            }

            foreach (Text t in texts)
            {
                t.color = new Color(.5f, .5f, .5f, 1);
            }
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
                // Set button interactable false, change color on price display's image
                auraSelection[i].interactable = false;
                auraPriceDisplay[i].GetComponentInChildren<Text>().text = "Owned";
                Color color = auraPriceDisplay[i].GetComponentInChildren<Image>().color;
                auraPriceDisplay[i].GetComponentInChildren<Image>().color = new Color(.5f, .5f, .5f, 1f);

                // Change colors of all images and texts belonging to the item to be disabled
                Image[] auraImgs = auraSelection[i].GetComponentsInChildren<Image>();
                Text[] texts = auraSelection[i].GetComponentsInChildren<Text>();
                foreach (Image a in auraImgs)
                {
                    a.color = new Color(.5f, .5f, .5f, 1);
                }

                foreach (Text t in texts)
                {
                    t.color = new Color(.5f, .5f, .5f, 1);
                }
            }
        }
    }
}
