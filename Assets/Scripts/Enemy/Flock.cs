using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemies will flock together if too close rather than overlapping, used for enemies that chase player

public class Flock : MonoBehaviour
{
    [SerializeField]
    FlockObject[] flockObjects;

    // Max speed of flock movement adjustments
    [SerializeField]
    float maxSpeed;

    // For calculating objects that count as neighbors
    [Range(1f, 10f)] [SerializeField]
    float neighborRadius;
    [Range(0f, 1f)] [SerializeField]
    float avoidRadiusMultiplier;       // 0 no avoidance radius, 1 is same avoidance radius as neighbor radius

    // For calculation purposes
    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidRadius;

    public float SquareAvoidRadius { get { return squareAvoidRadius; } }

    // Start is called before the first frame update
    void Start()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidRadius = squareNeighborRadius * avoidRadiusMultiplier * avoidRadiusMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        // For every flock object, calculate the movement to be applied
        foreach (FlockObject obj in flockObjects)
        {
            List<Transform> context = GetNearbyObjects(obj);

            Vector2 move = CalculateMove(obj, context, this);
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }

            obj.Move(move);
        }
    }

    // Return transforms of all objects within a radius using an overlap circle, return all other objects that overlap (within radius)
    List<Transform> GetNearbyObjects(FlockObject obj)
    {
        List<Transform> context = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(obj.transform.position, neighborRadius);

        foreach(Collider2D coll in contextColliders)
        {
            // Do not include self or player, only other nearby objects
            if (coll != obj.Coll && coll.gameObject.tag != "PlayerDamaged" && coll.gameObject.tag != "Player")
            {
                context.Add(coll.transform);
            }
        }

        return context;
    }

    // Calculate movement to avoid enemies from overlapping with eachother
    Vector2 CalculateMove(FlockObject obj, List<Transform> context, Flock flock)
    {
        // If no nearby objects, do not adjust position of object
        if (context.Count == 0)
        {
            return Vector2.zero;
        }

        // Calculate avoid movement
        Vector2 avoidMove = Vector2.zero;
        int avoidAmount = 0;
        foreach (Transform other in context)
        {
            if (Vector2.SqrMagnitude(other.position - obj.transform.position) < flock.SquareAvoidRadius)
            {
                avoidAmount++;
                avoidMove += (Vector2)(obj.transform.position - other.position);
            }
        }

        avoidMove /= context.Count;

        if (avoidAmount > 0)
        {
            avoidMove /= avoidAmount;
        }

        return avoidMove;
    }
}
