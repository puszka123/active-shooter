using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour {
    int respawnNameGenerator = 1;
    int doorNameGenerator = 1;
    int pathLocationNameGenerator = 1;
	// Use this for initialization
	void Start () {
        foreach (var item in GameObject.FindGameObjectsWithTag("PathLocation"))
        {
            item.name = "CheckPoint " + pathLocationNameGenerator++;
            item.GetComponent<PathLocation>().InitFloor();
        }
        foreach (var item in GameObject.FindGameObjectsWithTag("Door"))
        {
            item.name = "Door " + doorNameGenerator++;
        }
        foreach (var item in GameObject.FindGameObjectsWithTag("Respawn"))
        {
            item.name = "Respawn " + respawnNameGenerator++;
            item.GetComponent<Respawn>().enabled = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
