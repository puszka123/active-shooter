using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Finder {
    PersonMemory memory;
    public bool Executing;
    public Action ActionToExecute;


    public Finder(PersonMemory memory)
    {
        this.memory = memory;
    }
	
    public void ExecuteAction(Action action, Transform transform)
    {
        if (IsActionExecuting(action)) return; //don't do that again!
        Executing = true;
        switch (action.Command)
        {
            case Command.FIND_ROOM:
                Room room = FindNearestNotInformedRoom(memory.CurrentFloor, transform);
                if (room == null) return;
                SaveRoomInMemory(room);
                action.IsDone = true;
                Executing = false;
                break;
        }
    }

    public bool IsActionExecuting(Action action)
    {
        return (ActionToExecute != null && action.Command == ActionToExecute.Command && Executing);
    }

    public Room FindNearestRoom(int floor, Transform transform)
    {
        GameObject location = GameObject.Find("Checkpoints " + floor);
        List<GameObject> rooms = new List<GameObject>();
        foreach (Transform item in location.transform)
        {
            if (item.GetComponent<PathLocation>().IsRoom)
            {
                rooms.Add(item.gameObject);
            }
        }
        GameObject nearestRoom = rooms[0];
        foreach (var item in rooms)
        {
            if (Vector3.Distance(transform.position, nearestRoom.transform.position)
                >
                Vector3.Distance(transform.position, item.transform.position))
            {
                nearestRoom = item;
            }
        }
        return new Room() { Id = nearestRoom.name,
            Door = nearestRoom.GetComponent<PathLocation>().RoomDoor,
            Employees = nearestRoom.GetComponent<PathLocation>().RoomEmployees.ToArray(),
            Reference = nearestRoom
        };
    }

    public Room FindNearestNotInformedRoom(int floor, Transform transform)
    {
        GameObject location = GameObject.Find("Checkpoints " + floor);
        List<GameObject> rooms = new List<GameObject>();
        foreach (Transform item in location.transform)
        {
            if (item.GetComponent<PathLocation>().IsRoom && !memory.InformedRoom(item.gameObject))
            {
                rooms.Add(item.gameObject);
            }
        }
        if (rooms.Count == 0) return null;
        GameObject nearestRoom = rooms[0];
        foreach (var item in rooms)
        {
            if (Vector3.Distance(transform.position, nearestRoom.transform.position)
                >
                Vector3.Distance(transform.position, item.transform.position))
            {
                nearestRoom = item;
            }
        }
        return new Room()
        {
            Id = nearestRoom.name,
            Door = nearestRoom.GetComponent<PathLocation>().RoomDoor,
            Employees = nearestRoom.GetComponent<PathLocation>().RoomEmployees.ToArray(),
            Reference = nearestRoom
        };
    }

    public void SaveRoomInMemory(Room room)
    {
        memory.SaveFoundRoom(room);
    }

}
