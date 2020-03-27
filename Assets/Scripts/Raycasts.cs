using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycasts : MonoBehaviour
{
    // The distance/offset in which rays are fired from within the object so that rays aren't fired directly from edge of object
    protected const float offset = .02f;

    // Number of raycasts
    protected int horizontalRayCount = 10;
    protected int verticalRayCount = 10;

    // Spacing between each raycast in each group
    protected float horizontalRaySpacing;
    protected float verticalRaySpacing;

    // Raycast origins will be set starting from the corners of a box collider
    BoxCollider2D boxCollider;
    protected RaycastOrigins raycastOrigins;
    public struct RaycastOrigins
    {
        public Vector2 topLeft;
        public Vector2 topRight;
        public Vector2 bottomLeft;
        public Vector2 bottomRight;
    }

    public virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    // Update raycast origins
    public void UpdateRaycastOrigins()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(offset * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    // Calculate ray spacing at start
    public void CalculateRaySpacing()
    {
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(offset * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }
}
