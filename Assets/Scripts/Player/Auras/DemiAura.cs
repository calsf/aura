using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Temporarily halves enemy maxHP, after some time, enemy will restore any lost health from the halving of max health

public class DemiAura : MonoBehaviour
{
    [SerializeField]
    Material demiEffect;
    float restoreHPDelay = 10f;
    float resetRestoreRate = .2f; // Rate at which restoreHPTime should be reset

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            EnemyDefaults enemy = other.GetComponent<EnemyDefaults>();
            if (enemy != null)
            {
                if (Time.time > enemy.RestoreHPTime)
                {
                    enemy.RestoreHPTime = Time.time + restoreHPDelay;

                    // Take max hp of enemy and half it
                    int halvedMaxHP = enemy.Enemy.maxHP / 2;

                    // If enemy's current hp is more than the halved max health, set current hp to the halved max
                    if (enemy.HP > halvedMaxHP)
                    {
                        // The difference between enemy's hp and halved hp is the amount to be restored after restoreHPDelay expires
                        enemy.RestoreHPAmount = enemy.HP - halvedMaxHP;
                        enemy.DisplayDmgNum(enemy.RestoreHPAmount);

                        enemy.SpriteRender.material = demiEffect;   // Apply different material on enemy to indicate is affected by demi
                        enemy.HP = halvedMaxHP;
                        enemy.OnDamaged.Invoke();
                        enemy.StartColorChange();
                    }
                    else
                    {
                        enemy.DisplayDmgNum(0);

                        enemy.SpriteRender.material = demiEffect;   // Apply different material on enemy to indicate is affected by demi
                        enemy.OnDamaged.Invoke();
                        enemy.StartColorChange();
                    }
                }
                else if ((enemy.RestoreHPTime - Time.time) < (restoreHPDelay - resetRestoreRate))
                {
                    // Reset restore timer
                    enemy.RestoreHPTime = Time.time + restoreHPDelay;

                    enemy.DisplayDmgNum(0);
                    enemy.OnDamaged.Invoke();
                    enemy.StartColorChange();
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }
}
