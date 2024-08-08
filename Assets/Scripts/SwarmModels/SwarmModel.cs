using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SwarmModel : MonoBehaviour
{
    public List<GameObject> drones;
    public List<Transform> droneRBs;
    
    public Transform swarmHolder;

    private bool isApplyingForce = false;

    void Awake()
    {
        drones = new List<GameObject>();
        foreach (Transform child in swarmHolder)
        {
            drones.Add(child.gameObject);
            droneRBs.Add(child.GetComponent<interactionHandler>().drone.transform);
        }
    }

    void Start()
    {
        StartCoroutine(forceApplied());
    }

    void Update()
    {
        if(!isApplyingForce)
        {
            StartCoroutine(forceApplied());
            isApplyingForce = true;
        }
    }
    public void applyForce(Rigidbody droneRB)
    {
        Vector3 force = this.GetComponent<OlftaSaber>().ComputeOlfatiSaberInput(droneRB, droneRBs);
        force.y = 0;
        droneRB.AddForce(force);
    }

    IEnumerator forceApplied()
    {
        foreach (GameObject drone in drones)
        {
            Rigidbody droneRB = drone.GetComponent<interactionHandler>().drone.GetComponent<Rigidbody>();
            applyForce(droneRB);
        }
        yield return new WaitForSeconds(0.05f);
        isApplyingForce = false;
    }
}
