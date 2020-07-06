using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnhanceManager : MonoBehaviour
{
    // Parallel arrays for shop auras
    // NOTE: auraIndex is the corresponding index to total auras (to match the unlockedAuras from SaveLoadManager) E.g: first button auraSelection may have auraIndex of 5 because the button is for aura index 5 of unlockedAuras
    [SerializeField]
    Button[] auraSelection;     // Button for each aura

    [SerializeField]
    GameObject[] auraPriceDisplay; // Containers for price display for each aura price, includes an image and text component in children

    [SerializeField]
    Aura[] auras;               // Aura scriptable object

    [SerializeField]
    [TextArea(3, 10)]
    string enhanceBuy;        // Chat text when enhance is bought, specific to each aura
    [SerializeField]
    [TextArea(3, 10)]
    string buyFail;          // Chat text when enhance cannot be bought, generic for each aura

    SaveData saveData;
    SavedGold savedGold;
    ShopChat shopChat;

    // Start is called before the first frame update
    void Start()
    {
        shopChat = GetComponent<ShopChat>();
        saveData = GameObject.FindGameObjectWithTag("SaveData").GetComponent<SaveData>();
        savedGold = saveData.GetComponent<SavedGold>();
        UpdateShop();
    }

    void Update()
    {
        // On menu button, return back to level select scene
        if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["MenuButton"]) || Input.GetKeyDown(ControlsManager.ControlInstance.Padbinds["MenuPad"]))
        {
            LoadLevel.LoadInstance.LoadScene(1);
        }
    }

    // Button onClick to enhance aura
    public void BuyEnhance(int index)
    {
        // Return if selection has been disabled
        if (!auraSelection[index].interactable)
        {
            return;
        }

        int gold = SaveLoadManager.LoadGold();
        if (gold >= auras[index].enhancePrice[saveData.ExtraDmg[index]])
        {
            saveData.UpdateGold(gold - auras[index].enhancePrice[saveData.ExtraDmg[index]]);
            savedGold.UpdateGold();

            saveData.UpdateExtraDmg(index);             // Increase extra dmg
            UpdateShop();
            shopChat.SetChat(enhanceBuy);        // Show chat response to the purchase

            EnhanceAuraDisplay display = auraSelection[index].gameObject.GetComponent<EnhanceAuraDisplay>();
            display.UpdateDmg();
            display.UpdatePrice();

            SoundManager.SoundInstance.PlaySound("ShopBuySuccess");
        }
        else
        {
            shopChat.SetChat(buyFail);     // Show chat response if can't afford

            SoundManager.SoundInstance.PlaySound("ShopBuyFail");
        }
    }

    // Disable aura from shop if reached max bonus dmg
    public void UpdateShop()
    {
        int[] extraDmg = SaveLoadManager.LoadExtraDmg();

        for (int i = 0; i < auraSelection.Length; i++)
        {
            if (extraDmg[i] >= auras[i].maxExtraDmg)
            {
                // Set button interactable false, change color on price display's image
                auraSelection[i].interactable = false;

                // -1 maxExtraDmg means cannot be enhanced
                if (auras[i].maxExtraDmg != -1)
                {
                    auraPriceDisplay[i].GetComponentInChildren<Text>().text = "MAXED";
                    auraPriceDisplay[i].GetComponentInChildren<Image>().color = new Color(.5f, .5f, .5f, 1f);
                }

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
