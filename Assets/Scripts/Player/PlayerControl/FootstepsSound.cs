using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsSound : MonoBehaviour
{
    string nextStep = "Footstep2";
    string currStep = "Footstep1";

    public void Footstep()
    {
        string temp = currStep;
        SoundManager.SoundInstance.PlaySound(currStep);
        currStep = nextStep;
        nextStep = temp;
        
    }
}
