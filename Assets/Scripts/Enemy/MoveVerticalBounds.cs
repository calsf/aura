using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Move towards a camera y bound and resets to opposite y bound once it has gone out of camera bound's view

public class MoveVerticalBounds : MonoBehaviour
{
    CameraControl cam;
    EnemyDefaults enemyDefaults;
    Vector2 nextPos;
    Vector2 resetPos;

    [SerializeField]
    bool isMoveDown;    // Is enemy moving down?

    void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraControl>();

        // Set position to move towards, include offset so enemy is not in view when position is reset
        nextPos = isMoveDown ? new Vector2(transform.position.x, cam.MinY - 25) : new Vector2(transform.position.x, cam.MaxY + 25);

        // Set position to reset position to after position is reached
        resetPos = isMoveDown ? new Vector2(transform.position.x, cam.MaxY + 25) : new Vector2(transform.position.x, cam.MinY - 25);
    }

    void Start()
    {
        enemyDefaults = GetComponent<EnemyDefaults>();
    }

    void Update()
    {
        // Reset position once nextPos has been reached
        if ((!isMoveDown && transform.position.y >= nextPos.y) || (isMoveDown && transform.position.y <= nextPos.y))
        {
            transform.position = resetPos;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, nextPos, enemyDefaults.MoveSpeed * Time.deltaTime);
        }
    }
}
