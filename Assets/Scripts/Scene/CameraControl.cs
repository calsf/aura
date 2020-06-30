using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraControl : MonoBehaviour
{
    public UnityEvent OnFarTeleport;

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
        ResetCam(transform.position);

    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, .4f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }

    // Moves camera position to pos, calculating any min/max bounds before setting new position
    public void ResetCam(Vector3 pos)
    {
        // Max y and x camera bounds
        if (pos.y < minY)
        {
            pos = new Vector3(pos.x, minY, pos.z);
        }
        if (pos.y > maxY)
        {
            pos = new Vector3(pos.x, maxY, pos.z);
        }
        if (pos.x > maxX)
        {
            pos = new Vector3(maxX, pos.y, pos.z);
        }
        if (pos.x < minX)
        {
            pos = new Vector3(minX, pos.y, pos.z);
        }

        transform.position = pos;
    }

    // Updates focus area and cam position without any smoothing, making cam movement instant
    // Used for instant teleports such as resetting player on death or astral aura teleport
    public void MoveCamInstant()
    {
        // Update focus area with target collider
        focusArea.UpdateFocus(targetCollider.bounds);

        // Apply center offsets
        Vector2 focusPos = focusArea.center + (Vector2.up * yOffset) + (Vector2.right * xOffset);

        // Set cam position
        transform.position = new Vector3(focusPos.x, focusPos.y, -10);

        ResetCam(transform.position);

        // UPDATE AGAIN
        // Update focus area with target collider
        focusArea.UpdateFocus(targetCollider.bounds);

        // Apply center offsets
        focusPos = focusArea.center + (Vector2.up * yOffset) + (Vector2.right * xOffset);

        // Set cam position
        transform.position = new Vector3(focusPos.x, focusPos.y, -10);
    }
}
