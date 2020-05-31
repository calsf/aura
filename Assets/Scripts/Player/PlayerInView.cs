using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used to check if "player is in view"
// The argument that is passed into InView checks if the transform position is in camera (in player view)

public class PlayerInView : MonoBehaviour
{
    Camera cam;
    GameObject player;

    // maxX and maxY determine how far away player and other can be to be considered in view, lower the max distance, the closer they must be to eachother
    // maxX and maxY can be infinitely high but will always be bounded by camera view distance
    float maxX = 19f;
    float maxY = 12.5f;

    // To manually check if player is out of view (such as if player entered pyramid in level 8, they should be considereded out of view)
    bool outOfView; 

    public bool OutOfView { get { return outOfView; } set { outOfView = value; } }

    void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Check if other transform is in camera view 
    
    public bool InView(Transform other)
    {
        // Check if out of view manually set to false
        if (outOfView)
        {
            return false;
        }

        Vector3 point = cam.WorldToViewportPoint(other.position);

        // If other transform is in camera view
        if ( (point.z > 0 && point.x < 1f && point.x > 0 && point.y > 0f && point.y < 1f))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Check if other transform is within certain distance of player
    // xMinus and yMinus used to shorten max distance between player and other if needed
    public bool InDistance(Transform other, float xMinus = 1, float yMinus = 1)
    {
        // Distance between player and other
        float xDistance = Mathf.Sqrt(Mathf.Pow(other.position.x - player.transform.position.x, 2));
        float yDistance = Mathf.Sqrt(Mathf.Pow(other.position.y - player.transform.position.y, 2));

        // Check if the distance between player and other transform is less than the max distance
        if (xDistance < (maxX - xMinus) && yDistance < (maxY - yMinus))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Check if player is right outside of camera view of the transform that is passed in as argument, does not care about distance between player and other
    public bool EdgeOfView(Transform other)
    {
        Vector3 point = cam.WorldToViewportPoint(other.position);

        // Return true if other transform is in view or right outside of the camera view
        if (point.z > 0 && point.x < 1.05f && point.x > -.05 && point.y > -.05f && point.y < 1.05f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
