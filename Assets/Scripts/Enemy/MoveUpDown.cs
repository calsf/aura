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
    bool isFalling;

    float startY;
    [SerializeField]
    float upDist;

    void Awake()
    {
        if (disabledEffectPrefab != null)
        {
            disabledEffect = Instantiate(disabledEffectPrefab, transform.position, Quaternion.identity);
            disabledEffect.SetActive(false);
        }

        dmgDefaults = GetComponent<DamagePlayerDefaults>();
    }

    void OnEnable()
    {
        startY = transform.position.y;
    }

    void OnDisable()
    {
        velocity = Vector2.zero;
        if (disabledEffect != null)
        {
            disabledEffect.transform.position = transform.position;
            disabledEffect.SetActive(true);
        }
        isFalling = false;
    }

    void Update()
    {
        if (!isFalling)
        {
            velocity.y = initialY;
            isFalling = true;
        }

        // Once travelled more than upDist, lower speed
        if (Mathf.Abs(startY - transform.position.y) > upDist && velocity.y > (initialY / 3))
        {
            velocity.y = (initialY / 3);
        }

        // Fall down, cap fall speed
        velocity.y += (-7f * (dmgDefaults.Speed / dmgDefaults.DmgPlayer.baseSpeed)) * Time.deltaTime;
        if (velocity.y < -20)
        {
            velocity.y = -20;
        }

        velocity.x = x;

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
