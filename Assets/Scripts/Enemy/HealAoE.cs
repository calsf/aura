using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Child object of main enemy, acts as an area of effect heal, healing enemies inside the area

public class HealAoE : MonoBehaviour
{
    // Keep track of all affected enemies
    Dictionary<Collider2D, Affected> affected;

    int healAmount = 25;
    float healDelay = .1f;

    // An affected enemy has their enemy defaults and a timer for next heal
    public class Affected
    {
        public EnemyDefaults enemy;
        public float nextHeal;  // Time of next heal for the enemy
    }

    void Awake()
    {
        affected = new Dictionary<Collider2D, Affected>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Parent object is main enemy, do not heal self by checking other transform with parent transform
        if (other.tag == "Enemy" && other.gameObject.transform != gameObject.transform.parent)
        {
            // If enemy has not been kept track of, add to dictionary
            if (!affected.ContainsKey(other))
            {
                affected.Add(other, new Affected());
                affected[other].enemy = other.GetComponent<EnemyDefaults>();
                affected[other].nextHeal = 0;
            }

            // Check if can heal enemy
            if (Time.time > affected[other].nextHeal)
            {
                // Check for over heal
                int heal = (affected[other].enemy.HP + healAmount > affected[other].enemy.Enemy.maxHP) ?
                    healAmount - ((affected[other].enemy.HP + healAmount) - affected[other].enemy.Enemy.maxHP) : healAmount;

                // Heal enemy and add delay to next heal
                affected[other].enemy.HP += heal;
                affected[other].enemy.DisplayHealNum(heal);
                affected[other].nextHeal = Time.time + healDelay;
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        OnTriggerEnter2D(other);
    }
}
