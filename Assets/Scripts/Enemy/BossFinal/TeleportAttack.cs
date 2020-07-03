using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportAttack : MonoBehaviour
{
    [SerializeField]
    ShootFixedBehaviour shootBehaviour;

    Projectile[] projectiles;

    GameObject player;

    // Offset to teleport to player
    [SerializeField]
    float offsetX;
    float offsetY = 1;

    // Are projectiles returning to this object?
    bool returnProjectiles;

    Animator anim;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();

        projectiles = new Projectile[shootBehaviour.numProj];
        for (int i = 0; i < shootBehaviour.numProj; i++)
        {
            projectiles[i] = Instantiate(shootBehaviour.projectilePrefab, Vector3.down * 50, Quaternion.identity).GetComponent<Projectile>();
            projectiles[i].IgnoreCameraBounds = true;
            projectiles[i].gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (returnProjectiles)
        {
            // Check if all projectiles have returned, if so, will return to idle
            foreach (Projectile proj in projectiles)
            {
                if (proj.gameObject.activeInHierarchy)
                {
                    return;
                }
            }

            returnProjectiles = false;
            anim.SetTrigger("ReturnToIdle");
        }
    }

    void FixedUpdate()
    {
        if (returnProjectiles)
        {
            // Check if reached back to target, if so, disable projectile
            foreach (Projectile proj in projectiles)
            {
                if (Vector3.Distance(proj.gameObject.transform.position, transform.position) <= 0.5f)
                {
                    proj.DisabledEffect.transform.rotation = proj.transform.rotation;
                    proj.gameObject.SetActive(false);
                }
            }
        }
    }
    
    /****** Called in animation ******/
    public void Shoot()
    {
        for (int i = 0; i < shootBehaviour.numProj; i++)
        {
            projectiles[i].gameObject.transform.position = transform.position;
            projectiles[i].gameObject.SetActive(true);

            // Shoot projectiles in their fixed directions
            projectiles[i].Dir = (new Vector2(shootBehaviour.xDirection[i], shootBehaviour.yDirection[i]));
        }
    }

    // Teleport an offset behind player
    public void TeleportBehind()
    {
        transform.position = new Vector2(player.transform.position.x - offsetX, player.transform.position.y + offsetY);
    }

    // teleport an offset in front of player
    public void TeleportAhead()
    {
        transform.position = new Vector2(player.transform.position.x + offsetX, player.transform.position.y + offsetY);
    }

    // Return projectiles back to this object
    public void ReturnProjectiles()
    {
        returnProjectiles = true;

        // Reverse direction of projectiles
        foreach (Projectile proj in projectiles)
        {
            proj.Dir = -(proj.gameObject.transform.position - transform.position).normalized;
        }
    }


}
