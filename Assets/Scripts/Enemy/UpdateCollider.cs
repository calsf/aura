using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Updates a polygon collider to match sprite's physics shape

public class UpdateCollider : MonoBehaviour
{
    PolygonCollider2D coll;
    SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<PolygonCollider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (sr.sprite == null)
        {
            return;
        }

        // Reset the polygon collider
        Vector2[] empty = { Vector2.zero };
        for (int i = 0; i < coll.pathCount; i++)
        {
            coll.SetPath(i, empty);
        }
        coll.pathCount = sr.sprite.GetPhysicsShapeCount();

        // Update the polygon collider with sprite's physics shape
        List<Vector2> path = new List<Vector2>();
        for (int i = 0; i < coll.pathCount; i++)
        {
            path.Clear();
            sr.sprite.GetPhysicsShape(i, path);
            coll.SetPath(i, path.ToArray());
        }
    }
}
