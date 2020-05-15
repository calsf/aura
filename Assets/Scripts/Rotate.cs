using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] float rotation;
	void FixedUpdate () {
        transform.Rotate(0, 0, rotation);
	}
}
