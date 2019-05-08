using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour {

    public int numberOfSlots;

    GameObject objectToInstantiate;

	// Use this for initialization
	void Awake () {
        objectToInstantiate = GameObject.Find("Employee Origin");
        if(objectToInstantiate == null)
        {
            Debug.Log(transform.name + ": can't find an employee for instantiating");
        }
        for (int i = 0; i < numberOfSlots; i++)
        {
            Instantiate(objectToInstantiate, transform.position, transform.rotation);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
