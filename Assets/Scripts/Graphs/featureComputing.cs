using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class featureComputing : MonoBehaviour
{
    public List<Rigidbody> drones;
    private float detectionRadius = 5.0f;  // Radius to detect obstacles
    private string obstacleTag = "Obstacle"; // Tag for obstacles
    public void Start()
    {
        drones = new List<Rigidbody>();
        foreach (GameObject drone in this.GetComponent<SwarmModel>().drones)
        {
            drones.Add(drone.GetComponent<interactionHandler>().drone.GetComponent<Rigidbody>());
        }

        detectionRadius = this.GetComponent<OlftaSaber>().detectionRadius;
        obstacleTag = this.GetComponent<OlftaSaber>().obstacleTag;
    }
    public float computeMinDistance(Rigidbody droneRB)
    {
        float minDistance = float.MaxValue;
        foreach (Rigidbody rb in drones)
        {
            if (rb != droneRB)
            {
                float distance = Vector3.Distance(rb.position, droneRB.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                }
            }
        }
        return minDistance;
    }

    public float computeMinDistanceToObstacle(Rigidbody droneRB)
    {
        float minDistance = float.MaxValue;
        Collider[] hitColliders = Physics.OverlapSphere(droneRB.transform.position, detectionRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(obstacleTag))
            {
                // Find the closest point on the collider to the agent
                Vector3 closestPoint = hitCollider.ClosestPoint(droneRB.transform.position);
                Vector3 directionToObstacle = droneRB.transform.position - closestPoint;

                float distanceToObstacle = directionToObstacle.magnitude;

                if (distanceToObstacle < detectionRadius && distanceToObstacle > 0)
                {
                    if (distanceToObstacle < minDistance)
                    {
                        minDistance = distanceToObstacle;
                    }
                }

            }
        }

        return minDistance;
    }
}
