using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeNeck : MonoBehaviour
{
    [SerializeField]
    Transform meleeBoss;

    // Update is called once per frame
    void Update()
    {
        // Stretch neck scale to match distance between this object position and the melee boss position
        float x = Mathf.Abs(transform.position.x - meleeBoss.position.x);
        transform.localScale = new Vector2(x, transform.localScale.y);

        // Move y position to same position as melee boss target
        transform.position = new Vector2(transform.position.x, meleeBoss.position.y);
    }
}
