using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {
    public GameObject SimulationManager;
    public List<GameObject> Workplaces;
    public Canvas GUICanvas;
    public bool MenuOpened = true;

    public void InitEmployees()
    {
        foreach (var workplace in Workplaces)
        {
            workplace.GetComponent<Respawn>().InitEmployee();
        }
        //GameObject.FindGameObjectWithTag("InformManager").GetComponent<InformManager>().StartInitialization();
        SimulationManager.GetComponent<SimulationManager>().InitActiveShooter();
        foreach (Transform item in GUICanvas.transform)
        {
            if (item.name != "Reset")
            {
                item.gameObject.SetActive(false);
            }
        }
        MenuOpened = false;
    }

    public void InitWorkplaces()
    {
        SimulationManager = GameObject.FindGameObjectWithTag("SimulationManager");
        Workplaces = SimulationManager.GetComponent<SimulationManager>().workplaces;
    }
}
