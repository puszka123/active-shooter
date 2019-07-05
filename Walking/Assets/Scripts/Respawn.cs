using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Respawn : MonoBehaviour
{

    public int numberOfSlots;

    public static int nameGenerator = 0;

    GameObject objectToInstantiate;
    public string DoorKey;
    public string RoomId;


    float timer = 0.0f;
    bool testOnly = true;
    // Use this for initialization
    void Awake()
    {
        objectToInstantiate = GameObject.Find("Employee Origin");
        if (objectToInstantiate == null)
        {
            Debug.Log(transform.name + ": can't find an employee for instantiating");
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f && numberOfSlots > 0)
        {
            GameObject employee = Instantiate(objectToInstantiate, transform.position, transform.rotation);
            employee.GetComponent<Person>().Init(int.Parse(transform.parent.name.Split(' ')[1]), GetRoomIdForRespawn());
            employee.name = "employee " + nameGenerator++;
            DoorKey = GetDoorKeyForRespawn();
            employee.GetComponent<PersonDoor>().AddKey(DoorKey);
            timer = 0f;
            --numberOfSlots;
        }

        //test
        if(testOnly)
        {
            testOnly = false;
            GameObject informer = GameObject.Find("Informer");
            informer.GetComponent<Person>().Init(3, GetRoomIdForRespawn());
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

    public string GetRoomIdForRespawn()
    {
        string roomId = null;
        float distance = 9999999f;
        GameObject[] rooms = GameObject.FindGameObjectsWithTag("PathLocation").
            Where(pathLocation => pathLocation.GetComponent<PathLocation>().IsRoom).ToArray();
        foreach (var item in rooms)
        {
            float dist = Vector3.Distance(this.transform.position, item.transform.position);
            int layerMask = 1 << 9;
            bool blocked = Physics.Linecast(transform.position, item.transform.position, layerMask);
            if (dist < distance && !blocked)
            {
                roomId = item.transform.name;
                distance = dist;
            }
        }
        return roomId;
    }
}
