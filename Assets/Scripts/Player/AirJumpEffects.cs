using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages the AirJumpEffects

public class AirJumpEffects : MonoBehaviour
{
    // Object pool
    List<GameObject> fxPool;
    [SerializeField]
    GameObject fxPrefab;
    [SerializeField]
    int fxNum;

    GameObject player;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        fxPool = new List<GameObject>();
        for (int i = 0; i < fxNum; i++)
        {
            fxPool.Add(Instantiate(fxPrefab, Vector3.zero, Quaternion.identity));
            fxPool[i].SetActive(false);
        }
    }

    // Activate an air jump effect OnAirJump event in PlayerMove
    public void ActivateEffect()
    {
        for (int i = 0; i < fxPool.Count; i++)
        {
            if (!fxPool[i].activeInHierarchy)
            {
                fxPool[i].transform.position = player.transform.position;
                fxPool[i].SetActive(true);
                return;
            }
        }
    }
}
