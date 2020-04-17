using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockObject : MonoBehaviour
{
    Collider2D coll;

    public Collider2D Coll { get { return coll; } }

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collider2D>();
    }

    // Apply flock movement to this object, this will still allow for other movement while maintaining flock behavior
    public void Move(Vector2 velocity)
    {
        transform.position += (Vector3) velocity * Time.deltaTime;
    }
}
