﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonDoor : MonoBehaviour
{

    public List<string> myKeys;
    public GameObject door;
    public GameObject Door { get; }

    public void DoorMet(GameObject door)
    {
        if (WantToOpenDoor(door))
        {
            this.door = door;
            TryToOpenDoor();
        }
    }

    public void TryToOpenDoor()
    {
        door.SendMessage("TryToOpenDoor", new object[] { myKeys.ToArray(), gameObject });
    }

    public void AddKey(string key)
    {
        if (key == null)
        {
            Debug.Log(transform.name + " AddKey: key is null. => return null");
            return;
        }
        if (myKeys == null)
        {
            myKeys = new List<string>(new string[] { key });
        }
        else
        {
            myKeys.Add(key);
        }
    }

    public bool WantToOpenDoor(GameObject door)
    {
        Person person = GetComponent<Person>();
        Walking walking = person.walkingModule;

        if (walking.Path == null 
            || walking.Path.Count == 0 
            || walking.currentNodeIndex >= walking.Path.Count) return false;

        Node target = walking.Path[walking.currentNodeIndex];
        return IsTargetBehindDoor(target, door);
        
    }

    public bool IsTargetBehindDoor(Node target, GameObject door)
    {
        int layerMask = 1 << 10;
        RaycastHit hit;
        if (Physics.Linecast(target.Position, transform.position, out hit, layerMask))
        {
            return hit.transform.name == door.name;
        }
        else
        {
            return false;
        }
    }
}
