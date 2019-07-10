using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomManager : MonoBehaviour {
    public List<GameObject> RoomLocations;

    public void Init()
    {
        GetRoomLocations();
        InitRoomLocations();
        InitDoors();
    }

    public void GetRoomLocations()
    {
        GameObject[] roomLocations = GameObject.FindGameObjectsWithTag("RoomLocation");
        LayerMask layerMask = LayerMask.GetMask("Wall", "Door");
        roomLocations = roomLocations.Where(
            roomLocation => !Physics.Linecast(transform.position, roomLocation.transform.position, layerMask))
            .ToArray();
        RoomLocations = new List<GameObject>(roomLocations);
        foreach (var item in RoomLocations)
        {
            item.GetComponent<RoomLocation>().MyRoom = gameObject;
        }
    }

    public void InitRoomLocations()
    {
        foreach (var item in RoomLocations)
        {
            item.GetComponent<RoomLocation>().InitMyNeighbours(RoomLocations);
        }
    }

    public void InitDoors()
    {
        GameObject door = GetDoorForRoom();
        door.GetComponent<RoomLocation>().InitDoorNeighbours(RoomLocations, gameObject);
    }

    public GameObject GetDoorForRoom()
    {
        GameObject doorKey = null;
        float distance = 9999999f;
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        foreach (var item in doors)
        {
            float dist = Vector3.Distance(transform.position, item.transform.position);
            int layerMask = 1 << 9;
            bool blocked = Physics.Linecast(transform.position, item.transform.position, layerMask);
            if (dist < distance && !blocked)
            {
                doorKey = item;
                distance = dist;
            }
        }
        return doorKey;
    }
}
