using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For Projectiles which will also have DamagePlayerDefaults

public class Projectile : MonoBehaviour
{
    [SerializeField]
    GameObject disabledEffectPrefab;
    GameObject disabledEffect;
    CameraControl cam;
    Rigidbody2D rb;
    DamagePlayerDefaults dmgPlayer;
    bool ignoreGround;  // If ignoreGround true, will not be set inactive on collision with Ground layer
    bool ignoreFirstGround; // If ignoreFirstGround true, ignores first collision with Ground layer but triggers second collision
    bool hasExited;     // For if ignoreFirstGround true
    Vector2 dir;

    LayerMask ground;

    public Vector2 Dir { get { return dir; } set { dir = value; } }

    void Awake()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraControl>();
        rb = GetComponent<Rigidbody2D>();
        dmgPlayer = GetComponent<DamagePlayerDefaults>();

        disabledEffect = Instantiate(disabledEffectPrefab, transform.position, Quaternion.identity);
        disabledEffect.SetActive(false);

        ignoreGround = dmgPlayer.DmgPlayer.ignoreGround;
        ignoreFirstGround = dmgPlayer.DmgPlayer.ignoreFirstGround;

        ground = LayerMask.NameToLayer("Ground");
    }

    // Show effect when projectile is disabled
    void OnDisable()
    {
        if (disabledEffect != null)
        {
            disabledEffect.transform.position = transform.position;
            disabledEffect.SetActive(true);
        }
    }

    void OnEnable()
    {
        if (dmgPlayer == null)
        {
            dmgPlayer = GetComponent<DamagePlayerDefaults>();
        }

        // If ignoring first ground collision, check if projectile spawns inside ground
        if (ignoreFirstGround)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero, 0, LayerMask.GetMask("Ground"));
            if (hit.collider != null)
            {
                hasExited = false;  // If inside ground, ignore this collision
            }
            else
            {
                hasExited = true;   // If not inside ground, treat collisions normally and do not ignore any
            }
        }

        // Reset speed on enable
        dmgPlayer.Speed = dmgPlayer.DmgPlayer.baseSpeed;
    }

    void Update()
    {
        // Once projectile goes out of camera bounds, deactivate object, add offset to camera bounds for it to actually be off camera
        if (transform.position.x > (cam.MaxX + 25) || transform.position.x < (cam.MinX - 25) || transform.position.y > (cam.MaxY + 15) || transform.position.y < (cam.MinY - 15))
        {
            gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        rb.velocity = dir * dmgPlayer.Speed;
    }

    // On collision with ground, deactivate projectile unless it is supposed to ignore ground
    void OnTriggerEnter2D(Collider2D other)
    {
        // Don't collide if waiting for/ignoring first collision with ground
        if (ignoreFirstGround && !hasExited)
        {
            return;
        }

        if (!ignoreGround && other.gameObject.layer == ground)
        {
            gameObject.SetActive(false);
        }
    }

    //Once exits first initial collider, can split upon touching anything else
    void OnTriggerExit2D(Collider2D other)
    {
        if (ignoreFirstGround && !hasExited && other.gameObject.layer == ground)
        {
            hasExited = true;
        }
    }
}
