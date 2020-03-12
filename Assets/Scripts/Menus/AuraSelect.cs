using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AuraSelect : MonoBehaviour
{
    [SerializeField]
    GameObject[] auras;
    [SerializeField]
    PlayerAuraControl auraControl;

    int selectedAura;

    public UnityEvent OnAuraChange;
    public GameObject[] Auras { get { return auras; } }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (selectedAura != -1)
            {
                ResetSelectedAura();
            }
        }
    }

    // Selected aura will be set by EquipAura/an equip button
    public void SelectAura(int index)
    {
        selectedAura = index;
    }

    // Equip selected aura into selected slot
    public void SelectSlot(int slot)
    {
        if (selectedAura != -1)
        {
            auraControl.SelectedAuras[slot] = selectedAura;
            ResetSelectedAura();
            OnAuraChange.Invoke(); // OnAuraChange event
        }
    }

    public void ResetSelectedAura()
    {
        selectedAura = -1;
    }
}
