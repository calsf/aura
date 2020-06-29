using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// DamagePlayer object moves upwards and then falls back down - can also move in an x direction (golem shard)

public class MoveUpDown : MonoBehaviour
{
    DamagePlayerDefaults dmgDefaults;

    [SerializeField]
    float x;
    [SerializeField]
    float initialY;

    [SerializeField]
    GameObject disabledEffectPrefab;
    GameObject disabledEffect;

    // Object velocity
    Vector2 velocity;

    // Total falling speed to add to initial velocity
    float descendingSpeed;

    // Should be negative, adds falling velocity
    [SerializeField]
    float fallRate;

    void Awake()
    {
        if (disabledEffectPrefab != null)
        {
            disabledEffect = Instantiate(disabledEffectPrefab, transform.position, Quaternion.identity);
            disabledEffect.SetActive(false);
        }

        dmgDefaults = GetComponent<DamagePlayerDefaults>();
    }

    void OnDisable()
    {
        velocity = Vector2.zero;
        if (disabledEffect != null)
        {
            disabledEffect.transform.position = transform.position;
            disabledEffect.SetActive(true);
        }

        descendingSpeed = 0;
    }

    void Update()
    {
        // Adjust velocity y based on dmg player object's current speed
        velocity.y = (initialY + descendingSpeed) * dmgDefaults.Speed;

        // Gradually fall down unless speed has been affected
        if (dmgDefaults.Speed >= dmgDefaults.DmgPlayer.baseSpeed)
        {
            descendingSpeed += (fallRate) * Time.deltaTime;
        }
        
        // Cap fall speed
        if (velocity.y < -20)
        {
            velocity.y = -20;
        }

        // Adjust velocity x by a multiplier based on dmg player object's current speed
        velocity.x = x * (dmgDefaults.Speed / dmgDefaults.DmgPlayer.baseSpeed);

        transform.Translate(velocity * Time.deltaTime);
    }

    // If collides with ground layer, deactivate
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            gameObject.SetActive(false);
        }
    }
}
