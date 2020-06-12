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

    List<GameObject> mBubblesPool1;
    [SerializeField]
    GameObject mBubblesPrefab1;
    List<GameObject> mBubblesPool2;
    [SerializeField]
    GameObject mBubblesPrefab2;

    [SerializeField]
    int bubblesNum;

    GameObject player;
    PlayerMoveInput playerMove;

    float nextBubble;
    float bubbleDelayMin = 1f;
    float bubbleDelayMax = 2f;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMove = player.GetComponent<PlayerMoveInput>();

        // Init pool of still bubbles
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


        // Init pool of moving bubbles
        mBubblesPool1 = new List<GameObject>();
        for (int i = 0; i < bubblesNum; i++)
        {
            mBubblesPool1.Add(Instantiate(mBubblesPrefab1, Vector3.zero, Quaternion.identity));
            mBubblesPool1[i].SetActive(false);
        }
        mBubblesPool2 = new List<GameObject>();
        for (int i = 0; i < bubblesNum; i++)
        {
            mBubblesPool2.Add(Instantiate(mBubblesPrefab2, Vector3.zero, Quaternion.identity));
            mBubblesPool2[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextBubble)
        {
            nextBubble = Time.time + Random.Range(bubbleDelayMin, bubbleDelayMax);
            GetBubble();
        }
    }

    public void GetBubble()
    {
        // Select one of the bubble pools, use mBubblePools if player is moving
        int selection = Random.Range(1, 3);
        List<GameObject> bubbles;
        switch(selection)
        {
            case 1: bubbles = playerMove.Move != 0 ? mBubblesPool1 : bubblesPool1;
                break;
            case 2: bubbles = playerMove.Move != 0 ? mBubblesPool2 : bubblesPool2;
                break;
            default: bubbles = playerMove.Move != 0 ? mBubblesPool1 : bubblesPool1;
                break;
        }

        // Get an object from the pool
        for (int i = 0; i < bubbles.Count; i++)
        {
            if (!bubbles[i].activeInHierarchy)
            {
                // Set to this object's position and set facing direction equal to player's facing direction
                bubbles[i].transform.position = transform.position;
                bubbles[i].transform.localScale = player.transform.localScale.x > 0 ? 
                    new Vector2(transform.localScale.x, transform.localScale.y) : new Vector2(-transform.localScale.x, transform.localScale.y);

                bubbles[i].SetActive(true);
                return;
            }
        }
    }
}
