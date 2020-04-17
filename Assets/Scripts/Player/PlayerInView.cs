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

        // If the transform is in view space (1 and 0 would mean transform is at very edge/perfect in camera view but we will consider
        // in camera view to have an offset of .1f so InView behaviours are more easily noticeable upon entering camera view)
        if (point.z > 0 && point.x < .9f && point.x > 0.1 && point.y > 0.1f && point.y < .9f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
