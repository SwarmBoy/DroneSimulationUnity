using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject mainCamera;
    public List<GameObject> drones;

    public Vector3 center = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        drones = this.GetComponent<SwarmModel>().drones;
    }

    // Update is called once per frame
    void Update()
    {
        //get the avg position of all drones
        Vector3 avgPos = new Vector3(0, 0, 0);
        foreach (GameObject drone in drones)
        {
            avgPos += drone.GetComponent<interactionHandler>().positionDrone();
        }
        avgPos /= drones.Count;

        //move the camera to the avg position
        Vector3 cameraPos = mainCamera.transform.position;
        mainCamera.transform.position = new Vector3(avgPos.x, cameraPos.y, avgPos.z);

        center = new Vector3(avgPos.x, avgPos.y, avgPos.z);
    }
}
