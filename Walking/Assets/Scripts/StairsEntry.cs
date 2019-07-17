using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsEntry : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        Person person = other.GetComponent<Person>();
        if(person != null)
        {
            person.PersonMemory.ToggleIsAtStaircase();
        }
    }
}
