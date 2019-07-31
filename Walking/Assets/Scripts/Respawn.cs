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
    public GameObject room;
    public GameObject door;
    public GameObject employee;

    float timer = 0.0f;
    float testTimer = 0.0f;
    static bool testOnly = true;
    static System.Random random = new System.Random();
    // Use this for initialization
    private void Start()
    {
        room = GetRoomForRespawn();
        door = room.GetComponent<RoomManager>().Door; 
    }

    void Awake()
    {
        numberOfSlots = 1;
        objectToInstantiate = GameObject.Find("Employee Origin");
        if (objectToInstantiate == null)
        {
            Debug.Log(transform.name + ": can't find an employee for instantiating");
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        testTimer += Time.deltaTime;
        if (timer >= 1f && numberOfSlots > 0) //test
        {
            employee = Utils.GetNearestEmployee(gameObject, int.Parse(transform.parent.name.Split(' ')[1]));
            room.GetComponent<PathLocation>().AddRoomEmployee(employee);
            employee.name = "employee " + nameGenerator++;
            DoorKey = door.name;
            employee.GetComponent<PersonDoor>().AddKey(DoorKey);
            if (GetComponent<RoomLocation>().Workplace)
            {
                GetComponent<RoomLocation>().WorkEmployee = employee;
            }

            //start work 
            //int rand = random.Next(0, 100);
            //if (rand < 10)
            //{
            //    GameObject floor0 = GameObject.Find("Checkpoints 0");
            //    GameObject[] floorLocations = floor0.GetAllChilds().ToArray();
            //    int selected = random.Next(0, floor0.transform.childCount);
            //    employee.GetComponent<Person>().PersonMemory.ClearCurrentRoom();
            //    Utils.MoveAgent(employee, floorLocations[selected]);
            //}
            timer = 0f;
            --numberOfSlots;
        }
    }

    public GameObject GetDoorForRespawn()
    {
        GameObject doorKey = null;
        float distance = 9999999f;

        foreach (Transform item in GameObject.Find("Doors " + transform.parent.name.Split(' ')[1]).transform)
        {
            float dist = Utils.Distance(this.transform.position, item.transform.position);
            int layerMask = 1 << 9;
            bool blocked = Physics.Linecast(transform.position, item.transform.position, layerMask);
            if (dist < distance && !blocked)
            {
                doorKey = item.gameObject;
                distance = dist;
            }
        }
        return doorKey;
    }

    public GameObject GetRoomForRespawn()
    {
        GameObject roomId = null;
        float distance = 9999999f;

        foreach (Transform item in GameObject.Find("Checkpoints " + transform.parent.name.Split(' ')[1]).transform)
        {
            if(!item.GetComponent<PathLocation>().IsRoom)
            {
                continue;
            }

            float dist = Utils.Distance(this.transform.position, item.transform.position);
            int layerMask = 1 << 9;
            bool blocked = Physics.Linecast(transform.position, item.transform.position, layerMask);
            if (dist < distance && !blocked)
            {
                roomId = item.gameObject;
                distance = dist;
            }
        }
        return roomId;
    }

    public void InitEmployee()
    {
        employee.GetComponent<Person>().Init(int.Parse(transform.parent.name.Split(' ')[1]), room.name);
    }
}
