using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathLocation : MonoBehaviour {
    public List<GameObject> NearestNeighbours;
    public bool IsStairs = false;
    public bool IsToilet = false;
    public string StairsDirection = "";
    public int Floor;
    public bool IsExit = false;
    public bool IsRoom = false;
    public List<GameObject> RoomEmployees;
    public GameObject RoomDoor;
    public List<GameObject> Obstacles;

    public void FindMyNeighbours()
    {
        NearestNeighbours = new List<GameObject>();
        GameObject[] pathLocations = GameObject.FindGameObjectsWithTag("PathLocation");
        foreach (GameObject item in pathLocations)
        {
            int layerMask = 1 << 9;
            if(!Physics.Linecast(transform.position, item.transform.position, layerMask))
            {
                NearestNeighbours.Add(item);
            }

        }
    }

    public void FindRoomObstacles()
    {
        Obstacles = new List<GameObject>();
        LayerMask layerMask = LayerMask.GetMask("Door", "Wall");
        foreach (var item in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            if(!Physics.Linecast(transform.position, item.transform.position, layerMask))
            {
                Obstacles.Add(item);
            }
        }
    }

    public void InitFloor()
    {
        Floor = int.Parse(transform.parent.name.Split(' ')[1]);
    }

    public void AddRoomEmployee(GameObject employee)
    {
        if(RoomEmployees == null && IsRoom)
        {
            RoomEmployees = new List<GameObject>();
        }
        RoomEmployees.Add(employee);
    }

    public void SetRoomDoor(GameObject door)
    {
        if(IsRoom)
        {
            RoomDoor = door;
        }
    }

    public void PickedObstacle(GameObject obstacle)
    {
        Obstacles.Remove(Obstacles.Where(o => o.name == obstacle.name).ToArray().ElementAtOrDefault(0));
    }
}
