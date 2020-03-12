using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    [SerializeField]
    float maxX;
    [SerializeField]
    float minX;
    [SerializeField]
    float maxY;
    [SerializeField]
    float minY;

    Transform target;
    float smoothing;
    Vector3 offset;
    bool followPlayer;

    public float MaxX { get { return maxX; } set { maxX = value; } }
    public float MinX { get { return minX; } set { minX = value; } }
    public float MaxY { get { return maxY; } set { maxY = value; } }
    public float MinY { get { return minY; } set { minY = value; } }
    public bool FollowPlayer { get { return followPlayer; } set { followPlayer = value; } }

    // Use this for initialization
    void Awake()
    {
        followPlayer = true;
        target = player.GetComponent<Transform>();
        offset = transform.position - target.position;
        smoothing = 10f;
        GetComponent<Camera>().transparencySortMode = TransparencySortMode.Orthographic;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target != null && followPlayer)
        {
            Vector3 cameraPos = target.position + offset;
            transform.position = Vector3.Lerp(transform.position, cameraPos, smoothing * Time.deltaTime);

            if (transform.position.y < minY)
            {
                transform.position = new Vector3(transform.position.x, minY, transform.position.z);
            }
            if (transform.position.y > maxY)
            {
                transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
            }
            if (transform.position.x > maxX)
            {
                transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
            }
            if (transform.position.x < minX)
            {
                transform.position = new Vector3(minX, transform.position.y, transform.position.z);
            }
        }
    }
}
