using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReynoldModel : MonoBehaviour
{

    public List<GameObject> drones;
    public Transform droneHolder;
    public Transform target;

    [Header("Reynold's Model")]
    public float separationWeight = 1.0f;
    public float alignmentWeight = 1.0f;
    public float cohesionWeight = 1.0f;
    public float targetWeight = 1.0f;

    public float RangeApplicationReynold = 10.0f;
    public float maxForceTarget = 10.0f;

    [Header("Obstacle Avoidance")]
    public float detectionRadius = 5.0f;  // Radius to detect obstacles
    public float obstacleAvoidanceForceWeight = 2.0f; // Weight for obstacle avoidance force
    public float maxAvoidForce = 10.0f;   // Maximum avoidance force
    public string obstacleTag = "Obstacle"; // Tag for obstacles

    public Vector3 lastForce = Vector3.zero;


    public float radius = 1.0f;
    

    // Start is called before the first frame update

   /* void Update()
    {
        foreach (GameObject drone in drones)
        {
            Vector3 forceReynold = computeReynoldForce(drone);
            Vector3 forceObstacle = obstacleAvoidanceForce(drone);

            //dray the ray in blue the reynold force and in red the obstacle avoidance force
            Debug.DrawRay(drone.GetComponent<interactionHandler>().positionDrone(), forceReynold, Color.blue);
            Debug.DrawRay(drone.GetComponent<interactionHandler>().positionDrone(), forceObstacle, Color.red);

            drone.GetComponent<interactionHandler>().applyForce(forceReynold + forceObstacle);
        }
    }*/

    public Vector3 computeOlfatiSaber(GameObject drone)
    {
        Vector3 positionDrone = drone.GetComponent<interactionHandler>().positionDrone();
        
        return new Vector3(0, 0, 0);
    }

    public Vector3 computeReynoldForce(GameObject drone)
    {
        Vector3 positionDrone = drone.GetComponent<interactionHandler>().positionDrone();

        Vector3 separation = new Vector3(0, 0, 0);
        Vector3 alignment = new Vector3(0, 0, 0);
        Vector3 cohesion = new Vector3(0, 0, 0);
        Vector3 targetForce = new Vector3(0, 0, 0);

        int count = 0;
        foreach (GameObject d in drones)
        {
            if (d != drone)
            {
                GameObject realDrone = d.GetComponent<interactionHandler>().drone;
                float distance = Vector3.Distance(realDrone.transform.position, positionDrone);
                if (distance < RangeApplicationReynold)
                {
                    count++;
                    separation += (positionDrone - realDrone.transform.position).normalized / distance;
                    alignment += realDrone.GetComponent<Rigidbody>().velocity;
                    cohesion += realDrone.transform.position;
                }
            }
        }

        if (count > 0)
        {
            alignment /= count;
            cohesion /= count;
        }

        separation *= separationWeight;
        alignment =  (alignment - drone.GetComponent<interactionHandler>().velocityDrone()) * alignmentWeight;
        cohesion = (cohesion - positionDrone) * cohesionWeight;
        targetForce = (target.transform.position - positionDrone) * targetWeight;

        //maxForce = 10.0f;
        if (targetForce.magnitude > maxForceTarget)
        {
            targetForce = targetForce.normalized * maxForceTarget;
        }

        drone.GetComponent<varibaleManager>().lastAlignment = alignment;
        drone.GetComponent<varibaleManager>().lastCohesion = cohesion;
        drone.GetComponent<varibaleManager>().lastSeparation = separation;
        drone.GetComponent<varibaleManager>().lastTarget = targetForce;
        

        Vector3 totalForce = separation + alignment + cohesion + targetForce;

        return totalForce;
    }


    public Vector3 obstacleAvoidanceForce(GameObject d)
    {

        GameObject drone = d.GetComponent<interactionHandler>().drone;
        Vector3 avoidForce = Vector3.zero;
        Collider[] hitColliders = Physics.OverlapSphere(drone.transform.position, detectionRadius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(obstacleTag))
            {
                // Find the closest point on the collider to the agent
                Vector3 closestPoint = hitCollider.ClosestPoint(drone.transform.position);
                Vector3 directionToObstacle = drone.transform.position - closestPoint;
                float distanceToObstacle = directionToObstacle.magnitude;

                if (distanceToObstacle < detectionRadius && distanceToObstacle > 0)
                {
                    float forceMagnitude = Mathf.Min(maxAvoidForce, 1 / (distanceToObstacle*distanceToObstacle) * obstacleAvoidanceForceWeight);
                    avoidForce += directionToObstacle.normalized * forceMagnitude;

                }
            }
        }

        d.GetComponent<varibaleManager>().lastObstacle = avoidForce;
        return avoidForce;
    }

    public Vector3 computeObstacleAvoidanceForceVector(Vector3 position)
    {
        Vector3 avoidForce = Vector3.zero;
        Collider[] hitColliders = Physics.OverlapSphere(position, detectionRadius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(obstacleTag))
            {
                // Find the closest point on the collider to the agent
                Vector3 closestPoint = hitCollider.ClosestPoint(position);
                Vector3 directionToObstacle = position - closestPoint;
                float distanceToObstacle = directionToObstacle.magnitude;

                if (distanceToObstacle < detectionRadius && distanceToObstacle > 0)
                {
                    float forceMagnitude = Mathf.Min(maxAvoidForce, 1 / (distanceToObstacle * distanceToObstacle) * obstacleAvoidanceForceWeight);
                    avoidForce += directionToObstacle.normalized * forceMagnitude;

                }
            }
        }

        return avoidForce;
    }

    public Vector3 computeSeparationForceVector(Vector3 position)
    {
        Vector3 separation = new Vector3(0, 0, 0);
        foreach (GameObject d in drones)
        {
            GameObject realDrone = d.GetComponent<interactionHandler>().drone;
            float distance = Vector3.Distance(realDrone.transform.position, position);
            if (distance < RangeApplicationReynold)
            {
                separation += (position - realDrone.transform.position).normalized / distance;
            }
        }

        return separation;
    }

    public float computeObstacleAvoidanceForce(Vector3 position) //compute the obstacle avoidance force of a virtual position
    {
        Vector3 ostacleForce = computeObstacleAvoidanceForceVector(position);
        //scale the force to the range [0, 1]
        float magnitude = ostacleForce.magnitude;
        magnitude = Mathf.Min(magnitude, maxAvoidForce);
        return magnitude / maxAvoidForce;
    }

    public float computeObstacleAndSeparation (Vector3 position)
    {
        //compute the force of separation and obstacle avoidance normalized to [0, 1]
        Vector3 separation = computeSeparationForceVector(position);
        Vector3 obstacleAvoidance = computeObstacleAvoidanceForceVector(position);

        Vector3 total = separation + obstacleAvoidance;
        float magnitude = total.magnitude;
        
        return magnitude / (maxAvoidForce*1);
    }
}
