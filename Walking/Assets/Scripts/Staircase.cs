using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staircase : MonoBehaviour {
    public float SlowFactor;

	// Use this for initialization
	void Start () {
        SlowFactor = 2f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        Person person = other.GetComponent<Person>();
        if (person != null)
        {
            person.PersonMemory.AddCurrentStaircase(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Person person = other.GetComponent<Person>();
        if (person != null)
        {
            person.PersonMemory.ClearCurrentStaircase();
        }
    }
}
