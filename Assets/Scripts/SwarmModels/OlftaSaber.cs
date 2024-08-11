using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class OlftaSaber : MonoBehaviour
{
    public Transform migrationTarget;

    public float v_ref = 1.0f;
    public float d_ref = 1.0f;
    public float r0_coh = 20.0f;
    public float delta = 0.1f;
    public float a = 0.3f;
    public float b = 0.5f;
    public float gamma = 1.0f;
    public float c_vm = 1.0f;

    public float maxMigrationDistance = 10.0f;


    public float detectionRadius = 5.0f;  // Radius to detect obstacles
    public float obstacleAvoidanceForceWeight = 2.0f; // Weight for obstacle avoidance force
    public float maxAvoidForce = 10.0f;   // Maximum avoidance force
    public string obstacleTag = "Obstacle"; // Tag for obstacles


    public Vector3 ComputeOlfatiSaberInput(Rigidbody droneRb, List<Transform> neighbourDrones)
    {
        Vector3 accCoh = Vector3.zero;
        Vector3 accMig = Vector3.zero;
        Vector3 accVel = c_vm * (v_ref * droneRb.velocity.normalized - droneRb.velocity);

        float radius = droneRb.gameObject.transform.parent.GetComponent<interactionHandler>().radiusOfCollider;

        // Calculate the cohesion force
        foreach (Transform neighbour in neighbourDrones)
        {
            Vector3 posRel = neighbour.position - droneRb.position;
            float dist = posRel.magnitude - 2*radius;

            accCoh += GetCohesionForce(dist, d_ref, a, b, r0_coh, delta) * posRel.normalized;
        }

        // Calculate the migration force
        if (migrationTarget != null)
        {
            accMig = GetMigrationForce(migrationTarget.position, droneRb.position, v_ref, droneRb.velocity, gamma);
        }

        Vector3 accObs = obstacleAvoidanceForce(droneRb);

        // Sum up the forces
        Vector3 accCommand = accVel + accCoh + accMig + accObs;
        return accCommand;
    }

    public Vector3 obstacleAvoidanceForce(Rigidbody droneRB)
    {

        Vector3 avoidForce = Vector3.zero;
        Collider[] hitColliders = Physics.OverlapSphere(droneRB.transform.position, detectionRadius);
        float radius = droneRB.gameObject.transform.parent.GetComponent<interactionHandler>().radiusOfCollider;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(obstacleTag))
            {
                // Find the closest point on the collider to the agent
                Vector3 closestPoint = hitCollider.ClosestPoint(droneRB.transform.position);
                Vector3 directionToObstacle = droneRB.transform.position - closestPoint;

                float distanceToObstacle = directionToObstacle.magnitude-radius;

                if (distanceToObstacle < detectionRadius && distanceToObstacle > 0)
                {
                    float forceMagnitude = Mathf.Min(maxAvoidForce, 1 / (distanceToObstacle*distanceToObstacle) * obstacleAvoidanceForceWeight);
                    avoidForce += directionToObstacle.normalized * forceMagnitude;

                }
            }
        }

        droneRB.transform.parent.GetComponent<varibaleManager>().lastObstacle = avoidForce;
        return avoidForce;
    }

    private float GetCohesionForce(float r, float d_ref, float a, float b, float r0, float delta)
    {
        float cohesionIntensity = GetCohesionIntensity(r, d_ref, a, b);
        float neighbourWeight = GetNeighbourWeight(r, r0, delta);
        float cohesionIntensityDer = GetCohesionIntensityDer(r, d_ref, a, b);
        return 1 / r0 * GetNeighbourWeightDer(r, r0, delta) * cohesionIntensity + neighbourWeight * cohesionIntensityDer;
    }

    private float GetCohesionIntensity(float r, float d_ref, float a, float b)
    {
        float diff = r - d_ref;

        return ((a + b) / 2 * (Mathf.Sqrt(1 + (diff * diff)) - 1) + (a - b) * diff / 2);
    }

    private float GetCohesionIntensityDer(float r, float d_ref, float a, float b)
    {
        float diff = r - d_ref;
        return (a + b) / 2 * diff / Mathf.Sqrt(1 + diff * diff) + (a - b) / 2;
    }

    private float GetNeighbourWeight(float r, float r0, float delta)
    {
        float rRatio = r / r0;

        if (rRatio < delta)
        {
            return 1;
        }
        else if (rRatio < 1)
        {
            return 0.25f * Mathf.Pow(1 + Mathf.Cos(Mathf.PI * (rRatio - delta) / (1 - delta)), 2);
        }
        else
        {
            return 0;
        }
    }

    private float GetNeighbourWeightDer(float r, float r0, float delta)
    {
        float rRatio = r / r0;

        if (rRatio < delta)
        {
            return 0;
        }
        else if (rRatio < 1)
        {
            float arg = Mathf.PI * (rRatio - delta) / (1 - delta);
            return -0.5f * Mathf.PI / (1 - delta) * (1 + Mathf.Cos(arg)) * Mathf.Sin(arg);
        }
        else
        {
            return 0;
        }
    }

    private Vector3 GetMigrationForce(Vector3 p_mig, Vector3 p_i, float v_ref, Vector3 v_i, float gamma)
    {
        float d = Mathf.Min(Vector3.Distance(p_mig, p_i), maxMigrationDistance);
        Vector3 u_i = (p_mig - p_i).normalized;
        return gamma * d * (v_ref * u_i - v_i);
    }
}