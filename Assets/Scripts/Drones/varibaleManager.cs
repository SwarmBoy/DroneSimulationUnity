using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VariableToPlot
{
    public List<float> values = new List<float>();
    public string name;

    public void Add(float value)
    {
        values.Add(value);
    }
}
public class varibaleManager : MonoBehaviour
{
    [Header("Data to Plot")]
    [SerializeField]
    public List<VariableToPlot> variables = new List<VariableToPlot>();

    [Header("Recorded Data")]
    [SerializeField]
    private List<Vector3> position = new List<Vector3>();
    private List<Vector3> velocity = new List<Vector3>();
    private List<Vector3> forces = new List<Vector3>();
    private List<Vector3> separation = new List<Vector3>();
    private List<Vector3> alignment = new List<Vector3>();
    private List<Vector3> cohesion = new List<Vector3>();
    private List<Vector3> target = new List<Vector3>();
    private List<Vector3> obstacle = new List<Vector3>();
    private List<float> distanceToDrone = new List<float>();
    private List<float> distanceToObstacle = new List<float>();

    [Header("Last Forces")]
    [SerializeField]
    public Vector3 lastSeparation = Vector3.zero;
    public Vector3 lastAlignment = Vector3.zero;
    public Vector3 lastCohesion = Vector3.zero;
    public Vector3 lastTarget = Vector3.zero;
    public Vector3 lastObstacle = Vector3.zero;

    [SerializeField]
    private GameObject drone;
    private GameObject gm;

    public float recordEvery = 0.1f;
    void Start()
    {
        drone = this.GetComponent<interactionHandler>().drone;
        gm = GameObject.FindWithTag("gm");

        recordEvery = gm.GetComponent<config>().recordEvery;

        variables.Add(new VariableToPlot { name = "Force Separation", values = new List<float>() });
        variables.Add(new VariableToPlot { name = "Force Obstacle", values = new List<float>() });
        variables.Add(new VariableToPlot { name = "Change of Force Separation", values = new List<float>() });
        variables.Add(new VariableToPlot { name = "Change of Force Obstacle", values = new List<float>() });
        variables.Add(new VariableToPlot { name = "Distance to Drone", values = new List<float>() });
        variables.Add(new VariableToPlot { name = "Distance to Obstacle", values = new List<float>() });
    }

    void Update()
    {
        if (Time.time % recordEvery < 0.01)
        {
            position.Add(drone.transform.position);
            alignment.Add(lastAlignment);
            cohesion.Add(lastCohesion);
            separation.Add(lastSeparation);
            target.Add(lastTarget);
            obstacle.Add(lastObstacle);
            velocity.Add(drone.GetComponent<Rigidbody>().velocity);
            distanceToDrone.Add(gm.GetComponent<featureComputing>().computeMinDistance(drone.GetComponent<Rigidbody>()));
            distanceToObstacle.Add(gm.GetComponent<featureComputing>().computeMinDistanceToObstacle(drone.GetComponent<Rigidbody>()));


            forces.Add(lastAlignment + lastCohesion + lastSeparation + lastTarget + lastObstacle);

            if (position.Count > 1)
            {
                variables[0].Add(separation[separation.Count -1].magnitude);
                variables[1].Add(obstacle[obstacle.Count - 1].magnitude);
                variables[2].Add((separation[separation.Count - 1] - separation[separation.Count - 2]).magnitude);
                variables[3].Add((obstacle[obstacle.Count - 1] - obstacle[obstacle.Count - 2]).magnitude);
                variables[4].Add(distanceToDrone[distanceToDrone.Count - 1]);
                variables[5].Add(distanceToObstacle[distanceToObstacle.Count - 1]);
            }

        }
    }

    public float getLastDistanceToDrone()
    {
        return distanceToDrone[distanceToDrone.Count - 1];
    }

    public float getLastDistanceToObstacle()
    {
        return distanceToObstacle[distanceToObstacle.Count - 1];
    }

    //create JSON with all the lists
    public DroneData getDroneDataToJSON()
    {
        DroneData data = new DroneData();
        data.position = position;
        data.velocity = velocity;
        data.forces = forces;
        data.separation = separation;
        data.alignment = alignment;
        data.cohesion = cohesion;
        data.target = target;
        data.obstacle = obstacle;
        data.distanceToDrone = distanceToDrone;
        data.distanceToObstacle = distanceToObstacle;

        return data;
    }
}


public class DroneData
{
    public List<Vector3> position;
    public List<Vector3> velocity;
    public List<Vector3> forces;
    public List<Vector3> separation;
    public List<Vector3> alignment;
    public List<Vector3> cohesion;
    public List<Vector3> target;
    public List<Vector3> obstacle;
    public List<float> distanceToDrone;
    public List<float> distanceToObstacle;
}

