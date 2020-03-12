using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attached to camera
// Shake camera when an enemy is hit - Call Shake() when OnDamaged event occurs for all enemies

public class CameraShake : MonoBehaviour
{
    bool shaking;

    public void Shake()
    {
        if (!shaking)
        {
            StartCoroutine(Shake(.5f, .01f));
        }
    }

    IEnumerator Shake (float dur, float shake)
    {
        shaking = true;
        
        float shakeTime = 0f;
        float x = Random.Range(-1f, 1f) * shake;
        float y = Random.Range(-1f, 1f) * shake;
        float z = shake;

        while (shakeTime < dur)
        {
            x = -x;
            y = -y;
            z = -z;

            Vector3 camPos = transform.localPosition;
            transform.localPosition = new Vector3(camPos.x + x, camPos.y + y, camPos.z - z);

            shakeTime += Time.deltaTime;

            yield return null;
        }

        shaking = false;
    }
}
