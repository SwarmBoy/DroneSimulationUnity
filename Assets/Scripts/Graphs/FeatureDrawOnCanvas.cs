using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeatureDrawOnCanvas : MonoBehaviour
{
    public float refDistance = 1.0f;
    public List<GameObject> drones;

    public float drawEvery = 0.01f;
    
    void Start()
    {
        refDistance = this.GetComponent<OlftaSaber>().d_ref;
        drones = this.GetComponent<SwarmModel>().drones;

        //start recurring function
        InvokeRepeating("UpdateDrayOnCanvas", 0.0f, drawEvery);        
    }

    void UpdateDrayOnCanvas()
    {

        foreach (GameObject drone in drones)
        {
            float lastDistance = drone.GetComponent<varibaleManager>().getLastDistanceToDrone();
            if(lastDistance < refDistance)
            {
                //draw a circle around the drone
                Vector3 dronePosition = drone.GetComponent<interactionHandler>().drone.transform.position;
                Vector3 circleCenter = new Vector3(dronePosition.x, 0.1f, dronePosition.z);
                //dray on debug canvas
                float intensity = 1.0f - lastDistance / refDistance;
                drawCircle(circleCenter, refDistance, Color.green, intensity);
            }

            float lastDistanceToObstacle = drone.GetComponent<varibaleManager>().getLastDistanceToObstacle();
            if(lastDistanceToObstacle < this.GetComponent<OlftaSaber>().detectionRadius/4)
            {
                //draw a circle around the obstacle
                Vector3 dronePosition = drone.GetComponent<interactionHandler>().drone.transform.position;
                Vector3 circleCenter = new Vector3(dronePosition.x, 0.1f, dronePosition.z);
                //dray on debug canvas
                float intensity = 1.0f - lastDistanceToObstacle / (this.GetComponent<OlftaSaber>().detectionRadius/4);
                drawCircle(circleCenter, this.GetComponent<OlftaSaber>().detectionRadius/4, Color.red, intensity);
            }
        }
    }

    void drawCircle(Vector3 center, float radius, Color color, float intensity=1.0f)
    {
        //draw a circle on the debug canvas
        int numSegments = 100;
        for(int i = 0; i < numSegments; i++)
        {
            float angle = i * 2 * Mathf.PI / numSegments;
            Vector3 posStart = new Vector3(Mathf.Cos(angle) * radius, 0.1f, Mathf.Sin(angle) * radius) + center;
            angle = (i + 1) * 2 * Mathf.PI / numSegments;
            Vector3 posEnd = new Vector3(Mathf.Cos(angle) * radius, 0.1f, Mathf.Sin(angle) * radius) + center;

            color.a = intensity;
            //draw line from posStart to posEnd
            Debug.DrawLine(posStart, posEnd, color, drawEvery);            
        }
    }
}
