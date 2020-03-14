using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Script controls the currently equipped auras display

public class AuraSlots : MonoBehaviour
{
    [SerializeField]
    PlayerAuraControl auraControl;
    [SerializeField]
    AuraSelect auraSelect;

    SaveData saveData;  // Used to save equipped auras when equipped auras are updated

    [SerializeField]
    Image[] slots;

    void Start()
    {
        saveData = GameObject.FindGameObjectWithTag("SaveData").GetComponent<SaveData>();
        UpdateSlots();
    }

    void OnEnable()
    {
        auraSelect.OnAuraChange.AddListener(UpdateSlots);
    }

    void OnDisable()
    {
        auraSelect.OnAuraChange.RemoveListener(UpdateSlots);
    }

    // Update the equipped slots icon OnAuraChange event
    void UpdateSlots()
    {
        int[] selected = auraControl.SelectedAuras;
        AuraDefaults[] auras = auraControl.AuraDefaults;

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].sprite = auras[selected[i]].Aura.icon;
        }

        // Save equipped
        saveData.UpdateEquipped(selected);
    }
}
