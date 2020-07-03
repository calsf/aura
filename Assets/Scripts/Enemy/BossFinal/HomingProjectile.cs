using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    [SerializeField]
    GameObject disabledEffectPrefab;
    GameObject disabledEffect;

    DamagePlayerDefaults dmgPlayer;

    Vector2 origScale;

    GameObject player;

    public GameObject DisabledEffect { get { return disabledEffect; } }

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        origScale = transform.localScale;
        dmgPlayer = GetComponent<DamagePlayerDefaults>();

        if (disabledEffectPrefab != null)
        {
            disabledEffect = Instantiate(disabledEffectPrefab, transform.position, Quaternion.identity);
            disabledEffect.SetActive(false);
        }

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
    }

    void OnEnable()
    {
        if (dmgPlayer == null)
        {
            dmgPlayer = GetComponent<DamagePlayerDefaults>();
        }

        // Reset speed on enable
        dmgPlayer.Speed = dmgPlayer.DmgPlayer.baseSpeed;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, dmgPlayer.Speed * Time.deltaTime);
    }
}