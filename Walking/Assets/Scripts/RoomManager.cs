using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomManager : MonoBehaviour {
    public List<GameObject> RoomLocations;
    public int Floor;

    public void Init(int floor)
    {
        Floor = floor;
        GetRoomLocations();
        InitRoomLocations();
        InitDoors();
    }

    public void GetRoomLocations()
    {
        LayerMask layerMask = LayerMask.GetMask("Wall", "Door");
        RoomLocations = new List<GameObject>();

        foreach (Transform item in GameObject.Find("RoomLocations " + Floor).transform)
        {

            if(!Physics.Linecast(transform.position, item.position, layerMask))
            {
                item.GetComponent<RoomLocation>().MyRoom = gameObject;
                RoomLocations.Add(item.gameObject);
            }
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
        foreach (Transform item in GameObject.Find("Doors " + Floor).transform)
        {
            float dist = Utils.Distance(transform.position, item.position);
            int layerMask = 1 << 9;
            bool blocked = Physics.Linecast(transform.position, item.position, layerMask);
            if (dist < distance && !blocked)
            {
                doorKey = item.gameObject;
                distance = dist;
            }
        }
        return doorKey;
    }
}
