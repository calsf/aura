using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// From the platform's starting position, the moving platform moves to each pos points, also handles moving player on platform once player enters

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    Transform[] pos;

    int currIndex;
    Transform nextPos;

    [SerializeField]
    float speed;

    void Start()
    {
        nextPos = pos[0];
    }

    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextPos.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, nextPos.position) <= 0.1f)
        {
            // Increment currIndex and then check if it has reached past all positions in pos array, reset to 0 if it has, else keep currIndex
            currIndex++;
            currIndex = currIndex > (pos.Length - 1) ? 0 : currIndex;
  
            // Move to nextPos in the pos array
            nextPos = pos[currIndex];
        }
    }

    void OnCollisionEnter2D (Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.transform.SetParent(transform);
        }
    }

    void OnCollisionExit2D (Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.transform.SetParent(null);
        }
    }
}
