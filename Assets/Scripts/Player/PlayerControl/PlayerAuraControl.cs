using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Handles activation and use of auras

public class PlayerAuraControl : MonoBehaviour
{
    [SerializeField]
    PlayerMoveInput move;
    [SerializeField]
    AuraSelect auraSelect;
    [SerializeField]
    CanvasGroup menu;

    GameObject[] auras;
    AuraDefaults[] auraDefaults;
    GameObject currAura;
    bool toggleAura;

    int lastSelected = -1; // Last slot used
    int[] selectedAuras; // The numbers indicate index of auras array
    bool canAura = true;

    public int[] SelectedAuras { get { return selectedAuras; } }
    public AuraDefaults[] AuraDefaults { get { return auraDefaults; } }
    public bool CanAura { get { return canAura; } set { canAura = value; } }

    public UnityEvent OnActivateAura;

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
        // Load equipped auras from save
        selectedAuras = new int[4];
        selectedAuras = SaveLoadManager.LoadEquipped();

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
        //Do not allow player input while in menus or while dead
        if (MenuManager.MenuInstance.IsMenu || !canAura)
        {
            // If cannot use aura, set currAura inactive, keep aura on while in menu
            if (!canAura)
            {
                currAura.SetActive(false);
                lastSelected = -1;
            }
            return;
        }

        //Turn on aura on key down
        if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["Aura1Button"]) || Input.GetKeyDown(ControlsManager.ControlInstance.Padbinds["Aura1Pad"]))
        {
            ToggleAura(0);
        }
        else if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["Aura2Button"]) || Input.GetKeyDown(ControlsManager.ControlInstance.Padbinds["Aura2Pad"]))
        {
            ToggleAura(1);
        }
        else if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["Aura3Button"]) || Input.GetKeyDown(ControlsManager.ControlInstance.Padbinds["Aura3Pad"]))
        {
            ToggleAura(2);
        }
        else if (Input.GetKeyDown(ControlsManager.ControlInstance.Keybinds["Aura4Button"]) || Input.GetKeyDown(ControlsManager.ControlInstance.Padbinds["Aura4Pad"]))
        {
            ToggleAura(3);
        }

        // If ToggleAura option is not on, turn off aura on button release
        if (!toggleAura)
        {
            if ((lastSelected == 0 && (Input.GetKeyUp(ControlsManager.ControlInstance.Keybinds["Aura1Button"]) || Input.GetKeyUp(ControlsManager.ControlInstance.Padbinds["Aura1Pad"])))
                || (lastSelected == 1 && (Input.GetKeyUp(ControlsManager.ControlInstance.Keybinds["Aura2Button"]) || Input.GetKeyUp(ControlsManager.ControlInstance.Padbinds["Aura2Pad"])))
                || (lastSelected == 2 && (Input.GetKeyUp(ControlsManager.ControlInstance.Keybinds["Aura3Button"]) || Input.GetKeyUp(ControlsManager.ControlInstance.Padbinds["Aura3Pad"])))
                || (lastSelected == 3 && (Input.GetKeyUp(ControlsManager.ControlInstance.Keybinds["Aura4Button"]) || Input.GetKeyUp(ControlsManager.ControlInstance.Padbinds["Aura4Pad"]))))
            {
                AuraOff();
            }
        }
    }

    //For OnControlChange
    public void UpdateControls()
    {
        toggleAura = PlayerPrefs.GetString("ToggleAura") == "On" ? true : false;
    }

    //Toggle aura on and off
    void ToggleAura(int selected)
    {
        // Stop sound of current aura
        SoundManager.SoundInstance.StopSound($"{currAura.name}On");

        currAura.SetActive(false);

        // If aura is updated before an aura is activated
        if (selected != -1)
        {
            currAura = auras[selectedAuras[selected]];
        }

        // If same aura is selected, turn it off, else turn the selected aura on
        if (lastSelected != selected)
        {
            //Turn aura on
            currAura.SetActive(true);

            // Play AuraOn sound for the selected aura to activate
           SoundManager.SoundInstance.PlayStoppableSound($"{auras[selectedAuras[selected]].name}On");

            lastSelected = selected;
            OnActivateAura.Invoke();    // OnActivateAura event
        }
        else
        {
            lastSelected = -1;
        }
    }

    // Turn off aura
    public void AuraOff()
    {
        currAura.SetActive(false);
        lastSelected = -1;
    }

    // Updates aura without toggling it off when an aura is changed
    void UpdateAura()
    {
        StopCoroutine("WaitForResume");
        int temp = lastSelected;
        
        // Toggle the aura after player resumes/closes menu
        StartCoroutine(WaitForResume(temp));
    }

    // Toggle current aura after it has been updated and after player has resumed
    IEnumerator WaitForResume(int i)
    {
        yield return new WaitForSeconds(0.1f);
        lastSelected = -1;
        ToggleAura(i);
    }
}
