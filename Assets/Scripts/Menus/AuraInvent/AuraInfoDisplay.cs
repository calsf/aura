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


    void Awake()
    {
        auraName.text = aura.auraName;
        auraDesc.text = aura.desc;
        auraIcon.sprite = aura.icon;

        if (auraPrice != null)
        {
            auraPrice.text = aura.price.ToString();
        }

        if (auraDmg != null)
        {
            auraDmg.text = aura.baseDmg.ToString();
        }
    }

}
