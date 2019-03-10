using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder {

    public float GetGoalAngle(GameObject person, Vector3 destination)
    {
        return Vector3.SignedAngle(destination - person.transform.position, person.transform.forward, Vector3.up)*(-1.0f);
    }
}
