using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScrolling : MonoBehaviour
{
    [SerializeField]
    float length;

    float startPos;
    [SerializeField]
    [Range(0, 1)]
    float moveValue;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float dist = transform.position.x + moveValue;

        // Move background
        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        // Reset once past certain length
        if (transform.position.x > startPos + length)
        {
            transform.position = new Vector2(startPos, transform.position.y);
        }

    }
}
