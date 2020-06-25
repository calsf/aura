using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisitedBoss2 : MonoBehaviour
{
    SaveData saveData;

    // Start is called before the first frame update
    void Start()
    {
        saveData = GameObject.FindGameObjectWithTag("SaveData").GetComponent<SaveData>();
        saveData.UpdateVisitedBoss2(true);
    }

}
