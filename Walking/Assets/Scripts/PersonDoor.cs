using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonDoor : MonoBehaviour {

    public string[] myKeys;

	// Use this for initialization
	void Start () {
        //myKeys = new string[1];
        //myKeys[0] = "Door";
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DoorMet(GameObject door)
    {
        door.SendMessage("TryToOpenDoor", myKeys);
    }
}
