using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOnHit : MonoBehaviour
{
    [SerializeField]
    float growRate;

    // OnDamaged event
    public void GrowOnHit()
    {
        // Increase size by grow rate when hit
        if (transform.localScale.x < 1)
        {
            transform.localScale = new Vector2(transform.localScale.x + growRate, transform.localScale.y + growRate);
        }
    }
}
