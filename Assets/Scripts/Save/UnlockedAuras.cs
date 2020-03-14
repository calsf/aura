using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Show unlocked auras based on save data - Disable buttons to equip auras in AuraList/aura inventory if not unlocked

public class UnlockedAuras : MonoBehaviour
{
    [SerializeField]
    GameObject aurasParent;
    Button[] auras;

    // Start is called before the first frame update
    void Start()
    {
        auras = aurasParent.GetComponentsInChildren<Button>();
        bool[] auraUnlocked = SaveLoadManager.LoadAuras();

        for (int i = 0; i < auras.Length; i++)
        {
            if (auraUnlocked[i])
            {
                auras[i].gameObject.SetActive(true);
            }
            else
            {
                auras[i].gameObject.SetActive(false);
            }
        }
    }
}
