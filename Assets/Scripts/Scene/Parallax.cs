using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    float length;
    float startPos;

    CameraControl cam;
    [SerializeField] [Range(0, 1)]
    float moveValue;

    [SerializeField]
    bool noRepeat;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraControl>();
        startPos = transform.position.x;
        length = GetComponentInChildren<SpriteRenderer>().bounds.size.x;

        cam.OnFarTeleport.AddListener(ResetBG);
    }

    void OnDisable()
    {
        cam.OnFarTeleport.RemoveAllListeners();
    }

    // Update is called once per frame
    void Update()
    {
        float traveled = cam.transform.position.x * (1 - moveValue);
        float dist = cam.transform.position.x * moveValue;

        // Move background
        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        // Move backgrounds after a certain distance moved so backgrounds repeat
        if (!noRepeat && traveled > startPos + length)
        {
            startPos += length;
        }
        else if (traveled < startPos -length)
        {
            startPos -= length;
        }
    }

    public void ResetBG()
    {
        // Reset background as needed for OnFarTeleports, this will avoid sliding background if player/camera ends up too far
        float traveled = cam.transform.position.x * (1 - moveValue);
        if (traveled > startPos + length || traveled < startPos - length)
        {
            startPos = cam.transform.position.x;
            transform.position = new Vector3(startPos, transform.position.y, transform.position.z);
        }
    }
}
