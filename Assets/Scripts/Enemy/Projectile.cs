using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For Projectiles which will also have DamagePlayerDefaults

public class Projectile : MonoBehaviour
{
    CameraControl cam;
    Rigidbody2D rb;
    DamagePlayerDefaults dmgPlayer;
    bool ignoreGround;  // If ignoreGround true, will not be set inactive on collision with Ground layer

    void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraControl>();
        rb = GetComponent<Rigidbody2D>();
        dmgPlayer = GetComponent<DamagePlayerDefaults>();
    }

    void Update()
    {
        // Once projectile goes out of camera bounds, deactivate object, add offset to camera bounds for it to actually be off camera
        if (transform.position.x > (cam.MaxX + 25) || transform.position.x < (cam.MinX - 25) || transform.position.y > (cam.MaxY + 15) || transform.position.y < (cam.MinY - 15))
        {
            gameObject.SetActive(false);
        }
    }

    public void SetDirection(Vector2 dir)
    {
        rb.velocity = dir * dmgPlayer.Speed;
    }

    // On collision with ground, deactivate projectile unless it is supposed to ignore ground
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!ignoreGround && other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            gameObject.SetActive(false);
        }
    }
}
