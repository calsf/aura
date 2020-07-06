using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Sets texts to aura scriptable object

public class AuraInfoDisplay : MonoBehaviour
{
    [SerializeField]
    Aura aura;

    [SerializeField]
    Text auraName;
    [SerializeField]
    Text auraDesc;
    [SerializeField]
    Text auraDmg;
    [SerializeField]
    Text auraPrice;
    [SerializeField]
    Image auraIcon;

    [SerializeField]
    bool showBaseDmgOnly;
    SaveData saveData;


    void Awake()
    {
        saveData = GameObject.FindGameObjectWithTag("SaveData").GetComponent<SaveData>();

        auraName.text = aura.auraName;
        auraIcon.sprite = aura.icon;

        if (auraDesc != null)
        {
            auraDesc.text = aura.desc;
        }

        if (auraPrice != null)
        {
            auraPrice.text = aura.price.ToString();
        } 
    }

    void Start()
    {
        if (auraDmg != null)
        {
            string txt = showBaseDmgOnly ? aura.baseDmg.ToString() : (aura.baseDmg + saveData.ExtraDmg[aura.auraNumber]).ToString();
            auraDmg.text = txt;
        }
    }

}
