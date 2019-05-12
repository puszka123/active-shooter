using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour {

    public int numberOfSlots;

    public static int nameGenerator = 0;

    GameObject objectToInstantiate;

	// Use this for initialization
	void Awake () {
        objectToInstantiate = GameObject.Find("Employee Origin");
        string doorKey = GetDoorKeyForRespawn();
        if(objectToInstantiate == null)
        {
            Debug.Log(transform.name + ": can't find an employee for instantiating");
        }
        for (int i = 0; i < numberOfSlots; i++)
        {
           GameObject employee = Instantiate(objectToInstantiate, transform.position, transform.rotation);
            employee.name = "employee " + nameGenerator++;
            employee.GetComponent<PersonDoor>().AddKey(doorKey);
        }
	}
	
	public string GetDoorKeyForRespawn()
    {
        string doorKey = null;
        float distance = 9999999f;
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        foreach (var item in doors)
        {
            float dist = Vector3.Distance(this.transform.position, item.transform.position);
            int layerMask = 1 << 9;
            bool blocked = Physics.Linecast(transform.position, item.transform.position, layerMask);
            if (dist < distance && !blocked)
            {
                doorKey = item.transform.name;
                distance = dist;
            }
        }
        return doorKey;
    }
}
