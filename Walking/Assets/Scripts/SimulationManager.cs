using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        foreach (var item in GameObject.FindGameObjectsWithTag("Respawn"))
        {
            item.GetComponent<Respawn>().enabled = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
