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
    Collider2D[] firstGroundResults = new Collider2D[1];
    ContactFilter2D contactFilter;
    Collider2D coll;

    Vector2 dir;

    LayerMask ground;

    Vector2 origScale;
    bool pause;

    bool ignoreCameraBounds = false;    // Only for projectiles that will be gauranteed disabled some other way, such as final boss returning projectile

    public Vector2 Dir { get { return dir; } set { dir = value; } }
    public GameObject DisabledEffect { get { return disabledEffect; } }
    public bool IgnoreCameraBounds { set { ignoreCameraBounds = value; } }

    void Awake()
    {
        origScale = transform.localScale;
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraControl>();
        rb = GetComponent<Rigidbody2D>();
        dmgPlayer = GetComponent<DamagePlayerDefaults>();

        if (disabledEffectPrefab != null)
        {
            disabledEffect = Instantiate(disabledEffectPrefab, transform.position, Quaternion.identity);
            disabledEffect.SetActive(false);
        }

        ignoreGround = dmgPlayer.DmgPlayer.ignoreGround;
        ignoreFirstGround = dmgPlayer.DmgPlayer.ignoreFirstGround;

        coll = GetComponent<Collider2D>();
        ground = LayerMask.NameToLayer("Ground");
        contactFilter.SetLayerMask(LayerMask.GetMask("Ground"));    // Set contact filter to only look for ground layers
    }

    // Show effect when projectile is disabled
    void OnDisable()
    {
        if (disabledEffect != null)
        {
            disabledEffect.transform.position = transform.position;
            disabledEffect.SetActive(true);
        }

        // Reset scale to original scale so it is the correct scale when re-activated
        transform.localScale = origScale;
        firstGroundResults[0] = null;
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
            // Is collider overlapping with a ground layer on spawn?
            coll.OverlapCollider(contactFilter, firstGroundResults);
            
            if (firstGroundResults[0] != null)
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
            if (!ignoreCameraBounds)
            {
                gameObject.SetActive(false);
            }
        }

        // If rotate of dmgPlayer object is true, rotate projectile in direction it is moving
        if (dmgPlayer.DmgPlayer.rotate)
        {
            transform.right = dir;
        }
    }

    void FixedUpdate()
    {
        if (pause)
        {
            return;
        }

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

    public void PauseProjectile()
    {
        pause = true;
    }

    public void ResumeProjectile()
    {
        pause = false;
    }
}
