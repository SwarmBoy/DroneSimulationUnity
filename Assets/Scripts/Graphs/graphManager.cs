using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//text mesh pro
using TMPro;

public class graphManager : MonoBehaviour
{
    public GameObject droneSelected = null;
    public TMP_Dropdown dropdown;

    public List<String> optionsList = new List<String>();

    void Update()
    {
        //check if the user pressed on a drone
        if (Input.GetMouseButtonDown(0))
        {

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Clicked on the UI");
                return;
            }

            List<GameObject> allDrones = this.GetComponent<SwarmModel>().drones;
            foreach (GameObject d in allDrones)
            {
                d.GetComponent<interactionHandler>().drone.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.black;
            }

            if (Physics.Raycast(ray, out hit))
            {
                print(hit.transform.tag);
                if (hit.transform.tag == "Player")
                {
                    //get the drone
                    GameObject drone = hit.transform.parent.gameObject;
                    print("Drone selected: " + drone.name);
                    droneSelected = drone;

                    // change  material color of drone.transform.GetChild(0) to red
                    hit.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;

                    //update the options
                    updateOptions();
                }
                else
                {
                    droneSelected = null;
                    Debug.Log("No drone selected");
                    dropdown.ClearOptions();
                }
            }
            else
            {
                droneSelected = null;
                Debug.Log("No drone selected");
                dropdown.ClearOptions();
            }
        }
    }

    void updateOptions()
    {
        //get the variable in he header "Data to Plot
        varibaleManager vm = droneSelected.GetComponent<varibaleManager>();

        //clear the dropdown
        dropdown.ClearOptions();

        //add the options
        List<string> options = new List<string>();
        foreach (VariableToPlot variable in vm.variables)
        {
            options.Add(variable.name);
            optionsList.Add(variable.name);
        }

        dropdown.AddOptions(options);
    }



    public List<float> getValues()
    {
        if(droneSelected == null)
        {
            return new List<float>();
        }
        String selectedOption = dropdown.options[dropdown.value].text;
        varibaleManager vm = droneSelected.GetComponent<varibaleManager>();

        foreach (VariableToPlot variable in vm.variables)
        {
            if (variable.name == selectedOption)
            {
                return variable.values;
            }
        }

        return new List<float>();
    }
}
