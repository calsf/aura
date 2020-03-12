using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is so only one Equip button is needed to equip an aura into a slot

public class EquipAura : MonoBehaviour
{
    [SerializeField]
    AuraSelect aura;
    int setAura;

    // Set the aura to the Equip button will use based on aura selected
    public void SetAura(int index)
    {
        setAura = index;
    }

    // When Equip button is clicked, set the selectedAura to setAura index so it can be equipped into a slot
    public void Equip()
    {
        aura.SelectAura(setAura);
    }
}
