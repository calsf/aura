using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// On Awake, for every position p in pos, places a random object from objects at the position p

public class CornerSpawns : MonoBehaviour
{
    [SerializeField]
    GameObject[] objects;

    [SerializeField]
    Transform[] pos;

    void Awake()
    {
        List<GameObject> objectList = new List<GameObject>();
        foreach (GameObject o in objects)
        {
            objectList.Add(o);
        }

        foreach (Transform t in pos)
        {
            int choice = Random.Range(0, objectList.Count);

            objectList[choice].transform.position = t.position;
            objectList.RemoveAt(choice);
        }
    }
}
