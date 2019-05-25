using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{

    public int numberOfSlots;

    public static int nameGenerator = 0;

    GameObject objectToInstantiate;
    public string DoorKey;


    float timer = 0.0f;

    // Use this for initialization
    void Awake()
    {
        objectToInstantiate = GameObject.Find("Employee Origin");
        DoorKey = GetDoorKeyForRespawn();
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
            employee.name = "employee " + nameGenerator++;
            employee.GetComponent<PersonDoor>().AddKey(DoorKey);
            timer = 0f;
            --numberOfSlots;
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
