using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterFlock : Character
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void MoveToFlock(Vector3 dir)
    {
        List<Character> neighbors = GetNearbyObjs();
        Vector3 cohesion = ComputeCohesion(neighbors) * cohesionWeight;
        Vector3 alignment = ComputeAlignment(neighbors) * alignmentWeight;
        Vector3 separation = ComputeSeparation(neighbors) * separationWeight;
        Vector3 targetDirection = dir.normalized * targetWeight;

        Vector3 flockingDirection = cohesion + alignment + separation + targetDirection;
        Vector3 velocity = flockingDirection.normalized;
        MoveToVel(velocity);
        // animationController.Walk();
    }
    float neighborRadius = 2;
    float targetWeight = 2f;
    List<Character> GetNearbyObjs()
    {
        // List<Bot> neighbors = new List<Bot>();
        // foreach (Bot other in FindObjectsOfType<Bot>())
        // {
        //     if (other != this && Vector3.Distance(transform.position, other.transform.position) < neighborRadius)
        //     {
        //         neighbors.Add(other);
        //     }
        // }
        Collider[] bots = Physics.OverlapSphere(transform.position, neighborRadius, LayerMask.GetMask("Bot"));
        var botComponents = bots.Select(x => x.GetComponent<Character>());
        return botComponents.Where(x => x != this).ToList();
    }
    float cohesionWeight = 1.0f; // Weight of the cohesion behavior
    Vector3 ComputeCohesion(List<Character> neighbors)
    {
        if (neighbors.Count == 0)
            return Vector3.zero;

        Vector3 centerOfMass = Vector3.zero;
        foreach (Character neighbor in neighbors)
        {
            centerOfMass += neighbor.transform.position;
        }
        centerOfMass.y = 0;
        centerOfMass /= neighbors.Count;
        return (centerOfMass - transform.position).normalized;
    }
    float alignmentWeight = 2f; // Weight of the alignment behavior
    Vector3 ComputeAlignment(List<Character> neighbors)
    {
        if (neighbors.Count == 0)
            return transform.forward;

        Vector3 averageDirection = Vector3.zero;
        foreach (Character neighbor in neighbors)
        {
            averageDirection += neighbor.transform.forward;
        }
        averageDirection.y = 0;
        averageDirection /= neighbors.Count;
        return averageDirection.normalized;
    }
    float separationWeight = 1f; // Weight of the separation behavior
    float separationRadius = 0.5f; // Radius to maintain separation from other passengers
    Vector3 ComputeSeparation(List<Character> neighbors)
    {
        if (neighbors.Count == 0)
            return Vector3.zero;

        Vector3 separationVector = Vector3.zero;
        foreach (Character neighbor in neighbors)
        {
            Vector3 directionToNeighbor = transform.position - neighbor.transform.position;
            if (directionToNeighbor.magnitude < separationRadius)
            {
                separationVector += directionToNeighbor.normalized / directionToNeighbor.magnitude;
            }
        }
        separationVector.y = 0;
        return separationVector.normalized;
    }
    public Transform leader;
    private Vector3 ComputeLeaderDirection()
    {
        if (leader != null)
        {
            // Calculate direction towards the leader
            Vector3 leaderDirection = (leader.position - transform.position).normalized;
            // Optionally, you can weight the direction or add additional logic here
            return leaderDirection;
        }
        else
        {
            // If there's no leader, return zero vector
            return Vector3.zero;
        }
    }

    // Method to compute obstacle avoidance force
    private Vector3 ComputeObstacleAvoidance()
    {
        // Define the detection radius for obstacle avoidance
        float avoidanceRadius = 5f;
        // Cast a sphere around the agent to detect obstacles
        Collider[] obstacles = Physics.OverlapSphere(transform.position, avoidanceRadius, LayerMask.GetMask("Obs"));

        // If there are no obstacles, return zero vector
        if (obstacles.Length == 0)
        {
            return Vector3.zero;
        }

        Vector3 avoidanceForce = Vector3.zero;
        // Calculate avoidance force based on the direction away from obstacles
        foreach (Collider obstacle in obstacles)
        {
            Vector3 avoidanceDirection = (transform.position - obstacle.transform.position).normalized;
            // Optionally, you can weight the avoidance force or add additional logic here
            avoidanceForce += avoidanceDirection;
        }

        // Return the total avoidance force
        return avoidanceForce.normalized;
    }
}
