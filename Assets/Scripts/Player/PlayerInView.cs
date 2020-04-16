using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used to check if "player is in view"
// The argument that is passed into InView checks if the transform position is in camera (in player view)

public class PlayerInView : MonoBehaviour
{
    Camera cam;
    
    void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    //Check if player is in view of the transform that is passed in as argument
    public bool InView(Transform other)
    {
        Vector3 point = cam.WorldToViewportPoint(other.position);

        // If the transform is in view space
        if (point.z > 0 && point.x < 1 && point.x > 0 && point.y > 0 && point.y < 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
