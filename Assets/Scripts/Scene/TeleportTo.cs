using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Teleports to another TeleportTo object

public class TeleportTo : MonoBehaviour
{
    [SerializeField]
    TeleportTo teleTo;

    Dictionary<GameObject, bool> entered = new Dictionary<GameObject, bool>();

    public Dictionary<GameObject, bool> Entered { get { return entered; } set { entered = value; } }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" || other.tag == "Enemy")
        {
            // Check if object is entering current portal
            if (!entered.ContainsKey(other.gameObject))
            {
                entered.Add(other.gameObject, true);
            }
            else if (entered[other.gameObject])
            {
                return;
            }

            // Set other teleport to mark other as entered so object does not get teleported back
            if (!teleTo.Entered.ContainsKey(other.gameObject))
            {
                teleTo.Entered.Add(other.gameObject, true);
            }

            // Teleport to destination
            teleTo.Entered[other.gameObject] = true;
            other.transform.position = teleTo.transform.position;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // Reset entered status for exiting objects
        if (other.tag == "Player" || other.tag == "Enemy")
        {
            if (!entered.ContainsKey(other.gameObject))
            {
                entered.Add(other.gameObject, false);
            }

            entered[other.gameObject] = false;
        }
    }
}
