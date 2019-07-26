using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Finder {
    PersonMemory memory;
    public bool Executing;
    public Task TaskToExecute;


    public Finder(PersonMemory memory)
    {
        this.memory = memory;
    }
	
    public void ExecuteTask(Task task, Transform transform)
    {
        if (IsTaskExecuting(task)) return; //don't do that again!
        Executing = true;
        TaskToExecute = task;
        switch (task.Command)
        {
            case Command.FIND_ROOM:
                if(memory.FoundRoom != null)
                {
                    FinishFind();
                    return;
                }
                Room room = FindNearestNotInformedRoom(memory.CurrentFloor, transform);
                if (room == null) return;
                SaveRoomInMemory(room);
                task.IsDone = true;
                Executing = false;
                break;
            case Command.FIND_ANY_ROOM:
                if(Utils.IsInAnyRoom(memory))
                {
                    FinishFind();
                    return;
                }
                Room room1 = FindNearestNotInformedRoom(memory.CurrentFloor, transform);
                if (room1 == null) return;
                SaveRoomInMemory(room1);
                FinishFind();
                break;
        }
    }

    public bool IsTaskExecuting(Task task)
    {
        return (TaskToExecute != null && task.Command == TaskToExecute.Command && Executing);
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
            if (Utils.Distance(transform.gameObject, nearestRoom)
                >
                Utils.Distance(transform.gameObject, item))
            {
                nearestRoom = item;
            }
        }
        return new Room() { Id = nearestRoom.name,
            Door = nearestRoom.GetComponent<PathLocation>().RoomDoor,
            Employees = nearestRoom.GetComponent<PathLocation>().RoomEmployees.ToArray(),
            Reference = nearestRoom,
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
            if (Utils.Distance(transform.gameObject, nearestRoom)
                >
                Utils.Distance(transform.gameObject, item))
            {
                nearestRoom = item;
            }
        }
        return new Room()
        {
            Id = nearestRoom.name,
            Door = nearestRoom.GetComponent<PathLocation>().RoomDoor,
            Employees = nearestRoom.GetComponent<PathLocation>().RoomEmployees.ToArray(),
            Reference = nearestRoom,
        };
    }

    public void SaveRoomInMemory(Room room)
    {
        memory.SaveFoundRoom(room);
    }

    public void FinishFind()
    {
        TaskToExecute.IsDone = true;
        Executing = false;
    }

}
