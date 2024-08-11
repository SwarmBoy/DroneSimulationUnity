using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactionHandler : MonoBehaviour
{
    public GameObject drone;
    public float radiusOfCollider; 

    public void Start()
    {
        radiusOfCollider = drone.GetComponent<SphereCollider>().radius;
    }
    // Start is called before the first frame update
    public void applyForce(Vector3 force)
    {
        drone.GetComponent<Rigidbody>().AddForce(force);
    }
    public Vector3 positionDrone()
    {
        return drone.transform.position;
    }
    public Vector3 velocityDrone()
    {
        return drone.GetComponent<Rigidbody>().velocity;
    }
}
