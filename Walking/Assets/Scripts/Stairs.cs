using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour {
    public GameObject NextLocation;

    public void MoveAgent(GameObject agent)
    {
        agent.GetComponent<Rigidbody>().isKinematic = true;
        agent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        agent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX 
            | RigidbodyConstraints.FreezeRotationZ;
        agent.transform.position = NextLocation.transform.position;
        agent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        agent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX 
            | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        agent.GetComponent<Rigidbody>().isKinematic = false;
        agent.GetComponent<Person>().PersonMemory.CurrentFloor = int.Parse(NextLocation.transform.parent.name.Split(' ')[1]);
    }

    public void TeleportMePls(Object obj)
    {
        GameObject agent = obj as GameObject;
        if (NextLocation != null)
        {
            MoveAgent(agent);
        }
    }
}
