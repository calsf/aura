using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles activation and use of auras

public class PlayerAuraControl : MonoBehaviour
{
    [SerializeField]
    PlayerMove move;
    [SerializeField]
    AuraSelect auraSelect;
    [SerializeField]
    CanvasGroup menu;

    GameObject[] auras;
    AuraDefaults[] auraDefaults;
    GameObject currAura;
    bool toggleAura;

    int lastSelected = -1; // Last slot used
    int[] selectedAuras = { 0, 1, 2, 3 }; // The numbers indicate index of auras array

    public int[] SelectedAuras { get { return selectedAuras; } }
    public AuraDefaults[] AuraDefaults { get { return auraDefaults; } }

    void OnEnable()
    {
        auraSelect.OnAuraChange.AddListener(UpdateAura);
    }

    void OnDisable()
    {
        auraSelect.OnAuraChange.RemoveListener(UpdateAura);
    }

    // Start is called before the first frame update
    void Awake()
    {
        auras = auraSelect.Auras;
        currAura = auras[0];
        
        // Get auraDefaults script from auras
        auraDefaults = new AuraDefaults[auras.Length];
        for (int i = 0; i < auras.Length; i++)
        {
            auraDefaults[i] = auras[i].GetComponent<AuraDefaults>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Do not allow control when paused
        if (menu.alpha == 0)
        {

            if (Input.GetKeyDown(ControlsManager.controlManager.Keybinds["Aura1Button"]) || Input.GetKeyDown(ControlsManager.controlManager.Padbinds["Aura1Pad"]))
            {
                ToggleAura(0);
            }
            else if (Input.GetKeyDown(ControlsManager.controlManager.Keybinds["Aura2Button"]) || Input.GetKeyDown(ControlsManager.controlManager.Padbinds["Aura2Pad"]))
            {
                ToggleAura(1);
            }
            else if (Input.GetKeyDown(ControlsManager.controlManager.Keybinds["Aura3Button"]) || Input.GetKeyDown(ControlsManager.controlManager.Padbinds["Aura3Pad"]))
            {
                ToggleAura(2);
            }
            else if (Input.GetKeyDown(ControlsManager.controlManager.Keybinds["Aura4Button"]) || Input.GetKeyDown(ControlsManager.controlManager.Padbinds["Aura4Pad"]))
            {
                ToggleAura(3);
            }

            // If toggle is not on, turn off aura on button release
            if (!toggleAura)
            {
                if ((lastSelected == 0 && (Input.GetKeyUp(ControlsManager.controlManager.Keybinds["Aura1Button"]) || Input.GetKeyUp(ControlsManager.controlManager.Padbinds["Aura1Pad"])))
                    || (lastSelected == 1 && (Input.GetKeyUp(ControlsManager.controlManager.Keybinds["Aura2Button"]) || Input.GetKeyUp(ControlsManager.controlManager.Padbinds["Aura2Pad"])))
                    || (lastSelected == 2 && (Input.GetKeyUp(ControlsManager.controlManager.Keybinds["Aura3Button"]) || Input.GetKeyUp(ControlsManager.controlManager.Padbinds["Aura3Pad"])))
                    || (lastSelected == 3 && (Input.GetKeyUp(ControlsManager.controlManager.Keybinds["Aura4Button"]) || Input.GetKeyUp(ControlsManager.controlManager.Padbinds["Aura4Pad"]))))
                {
                    currAura.SetActive(false);
                    lastSelected = -1;
                }
            }
        }
    }

    //For OnControlChange
    public void UpdateControls()
    {
        toggleAura = PlayerPrefs.GetString("ToggleAura") == "On" ? true : false;
    }

    void ToggleAura(int selected)
    {
        currAura.SetActive(false);

        // If aura is updated before an aura is activated
        if (selected != -1)
        {
            currAura = auras[selectedAuras[selected]];
        }

        // If same aura is selected, turn it off, else turn the selected aura on
        if (lastSelected != selected)
        {
            currAura.SetActive(true);
            lastSelected = selected;
        }
        else
        {
            lastSelected = -1;
        }
    }

    // Updates aura without toggling it off when an aura is changed
    void UpdateAura()
    {
        int temp = lastSelected;
        lastSelected = -1;
        ToggleAura(temp);
    }
}
