using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonPicker : MonoBehaviour {

	public void PickMe(GameObject obstacle)
    {
        GetComponent<Person>().walkingModule.OnObstaclePick(obstacle, gameObject);
    }

    public void StopPickingMe(GameObject obstacle)
    {
        GetComponent<Person>().walkingModule.OnBeNearObstacle(obstacle, gameObject);
    }

}
