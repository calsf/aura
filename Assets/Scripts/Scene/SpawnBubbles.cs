using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBubbles : MonoBehaviour
{
    // Object pool
    List<GameObject> bubblesPool1;
    [SerializeField]
    GameObject bubblesPrefab1;

    List<GameObject> bubblesPool2;
    [SerializeField]
    GameObject bubblesPrefab2;

    [SerializeField]
    int bubblesNum;

    GameObject player;
    PlayerMoveInput playerMove;

    float nextBubble;
    float bubbleDelayMin = .5f;
    float bubbleDelayMax = 1f;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMove = player.GetComponent<PlayerMoveInput>();

        bubblesPool1 = new List<GameObject>();
        for (int i = 0; i < bubblesNum; i++)
        {
            bubblesPool1.Add(Instantiate(bubblesPrefab1, Vector3.zero, Quaternion.identity));
            bubblesPool1[i].SetActive(false);
        }

        bubblesPool2 = new List<GameObject>();
        for (int i = 0; i < bubblesNum; i++)
        {
            bubblesPool2.Add(Instantiate(bubblesPrefab2, Vector3.zero, Quaternion.identity));
            bubblesPool2[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextBubble && playerMove.Velocity.y < 0)
        {
            nextBubble = Time.time + Random.Range(bubbleDelayMin, bubbleDelayMax);
            GetBubble();
        }
    }

    public void GetBubble()
    {
        int selection = Random.Range(1, 3);
        List<GameObject> bubbles;

        switch(selection)
        {
            case 1: bubbles = bubblesPool1;
                break;
            case 2: bubbles = bubblesPool2;
                break;
            default: bubbles = bubblesPool1;
                break;
        }


        for (int i = 0; i < bubbles.Count; i++)
        {
            if (!bubbles[i].activeInHierarchy)
            {
                bubbles[i].transform.position = transform.position;
                bubbles[i].transform.localScale = player.transform.localScale.x > 0 ? 
                    new Vector2(transform.localScale.x, transform.localScale.y) : new Vector2(-transform.localScale.x, transform.localScale.y);
                bubbles[i].SetActive(true);
                return;
            }
        }
    }
}
