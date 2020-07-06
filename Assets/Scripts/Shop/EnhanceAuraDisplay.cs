using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnhanceAuraDisplay : MonoBehaviour
{
    [SerializeField]
    Aura aura;

    [SerializeField]
    Text auraName;
    [SerializeField]
    Text auraDmg;
    [SerializeField]
    Text auraPrice;
    [SerializeField]
    Image auraIcon;

    [SerializeField]
    Text auraEnhancedDmg;
    [SerializeField]
    Image auraEnhancedIcon;

    SaveData saveData;


    void Awake()
    {
        saveData = GameObject.FindGameObjectWithTag("SaveData").GetComponent<SaveData>();

        auraName.text = "Enhance " + aura.auraName;
        auraIcon.sprite = aura.icon;
        auraEnhancedIcon.sprite = aura.icon;
    }

    void Start()
    {
        UpdateDmg();
        UpdatePrice();
    }

    // Update dmg including extra dmg
    public void UpdateDmg()
    {
        if (auraDmg != null)
        {
            int dmg = (aura.baseDmg + saveData.ExtraDmg[aura.auraNumber]);
            auraDmg.text = dmg.ToString();

            // Do not show another increase in enhanced damage if reached max extra dmg
            auraEnhancedDmg.text = (saveData.ExtraDmg[aura.auraNumber]) >= aura.maxExtraDmg ? (aura.baseDmg + aura.maxExtraDmg).ToString() : (dmg + 1).ToString();
        }
    }

    public void UpdatePrice()
    {
        // Do not update price here if reached max extra dmg
        if (saveData.ExtraDmg[aura.auraNumber] >= aura.maxExtraDmg)
        {
            return;
        }

        // Extra dmg amount determines the enhance price
        auraPrice.text = aura.enhancePrice[saveData.ExtraDmg[aura.auraNumber]].ToString();
    }

}