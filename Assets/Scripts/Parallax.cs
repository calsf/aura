using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    float length;
    float startPos;

    [SerializeField]
    GameObject cam;
    [SerializeField] [Range(0, 1)]
    float moveValue;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position.x;
        length = GetComponentInChildren<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float traveled = cam.transform.position.x * (1 - moveValue);
        float dist = cam.transform.position.x * moveValue;

        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        if (traveled > startPos + length)
        {
            startPos += length;
        }
        else if (traveled < startPos -length)
        {
            startPos -= length;
        }
    }
}
