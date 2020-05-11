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
    BoxCollider2D targetCollider;
    bool followPlayer;

    // Camera offsets
    float yOffset = 0f;
    float xOffset = 0f;

    // Smoothing time for y movement of camera
    float smoothTimeY = .1f;
    float smoothVelocityY;

    // Area around player that camera will focus on
    Vector2 focusAreaSize;
    FocusArea focusArea;
    struct FocusArea
    {
        public Vector2 center;
        public Vector2 velocity;
        float left;
        float right;
        float top;
        float bottom;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        // Update the focus area 
        public void UpdateFocus(Bounds targetBounds)
        {
            // Left and right
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            // Top and bottom
            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            top += shiftY;
            bottom += shiftY;

            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }

    public float MaxX { get { return maxX; } set { maxX = value; } }
    public float MinX { get { return minX; } set { minX = value; } }
    public float MaxY { get { return maxY; } set { maxY = value; } }
    public float MinY { get { return minY; } set { minY = value; } }
    public bool FollowPlayer { get { return followPlayer; } set { followPlayer = value; } }

    // Use this for initialization
    void Awake()
    {
        focusAreaSize = new Vector2(1, 6);
        target = player.GetComponent<Transform>();
        targetCollider = player.GetComponent<BoxCollider2D>();
        focusArea = new FocusArea(targetCollider.bounds, focusAreaSize);

        followPlayer = true;
        GetComponent<Camera>().transparencySortMode = TransparencySortMode.Orthographic;
    }

    void LateUpdate()
    {
        // Return if not following player or if player null
        if (!followPlayer || target == null)
        {
            return;
        }

        // Update focus area with target collider
        focusArea.UpdateFocus(targetCollider.bounds);

        // Apply center offsets
        Vector2 focusPos = focusArea.center + (Vector2.up * yOffset) + (Vector2.right * xOffset);

        // Set the focusArea position
        if (Time.deltaTime != 0)    // Avoid dividing by deltaTime 0
        {
            focusPos.y = Mathf.SmoothDamp(transform.position.y, focusPos.y, ref smoothVelocityY, smoothTimeY);
        }
        else
        {
            focusPos.y = Mathf.SmoothDamp(transform.position.y, focusPos.y, ref smoothVelocityY, smoothTimeY, Mathf.Infinity, .001f);
        }

        // Set cam position
        transform.position = new Vector3(focusPos.x, focusPos.y, -10);

        // Max y and x camera bounds
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

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, .4f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }
}
