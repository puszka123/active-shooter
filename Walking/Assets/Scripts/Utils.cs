using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Utils
{
    public static Room GetRoom(Task task)
    {
        return task.Limit?.FoundRoom;
    }

    public static Room GetRoom(PersonMemory memory)
    {
        return memory.CurrentRoom;
    }

    public static GameObject GetDoor(Task task)
    {
        return task.Limit?.DoorToOpen;
    }

    public static GameObject[] GetEmployees(Task task)
    {
        return task.Limit?.Employees;
    }

    public static GameObject GetObstacle(Task task)
    {
        return task.Limit?.Obstacle;
    }

    public static string NearestStairs(string stairsType, Transform transform, PersonMemory memory)
    {
        GameObject location = GameObject.Find("Checkpoints " + memory.CurrentFloor);
        List<GameObject> stairs = new List<GameObject>();
        foreach (Transform item in location.transform)
        {
            if (item.GetComponent<Stairs>() != null && item.GetComponent<PathLocation>().StairsDirection == stairsType)
            {
                stairs.Add(item.gameObject);
            }
            if (stairs.Count >= 2) break;
        }
        GameObject nearestStairs = stairs[0];
        foreach (var item in stairs)
        {
            if (Vector3.Distance(transform.position, nearestStairs.transform.position)
                >
                Vector3.Distance(transform.position, item.transform.position))
            {
                nearestStairs = item;
            }
        }
        return nearestStairs.name;
    }

    public static string NearestExit(Transform transform, PersonMemory memory)
    {
        GameObject location = GameObject.Find("Checkpoints " + memory.CurrentFloor);
        List<GameObject> exits = new List<GameObject>();
        foreach (Transform item in location.transform)
        {
            if (item.GetComponent<PathLocation>().IsExit)
            {
                exits.Add(item.gameObject);
            }
            if (exits.Count >= 2) break;
        }
        if (exits.Count == 0) return null;
        GameObject nearestExits = exits[0];
        foreach (var item in exits)
        {
            if (Vector3.Distance(transform.position, nearestExits.transform.position)
                >
                Vector3.Distance(transform.position, item.transform.position))
            {
                nearestExits = item;
            }
        }
        return nearestExits.name;
    }

    public static bool ToFar(GameObject a, GameObject b, float threshold = 0.25f)
    {
        return Vector3.Distance(a.transform.position, b.transform.position) > threshold;
    }

    public static bool DoorIsOpened(GameObject door)
    {
        return door.GetComponent<DoorController>().IsOpen;
    }

    public static bool DoorIsLocked(GameObject door)
    {
        if (door == null) return true;
        if (door.GetComponent<DoorController>().Destroyed()) return false;
        return door.GetComponent<DoorController>().IsLocked;
    }

    public static GameObject NearestRoomLocation(GameObject room, GameObject person)
    {
        GameObject[] roomLocations = room.GetComponent<RoomManager>().RoomLocations.ToArray();
        GameObject nearestRoomLocation = roomLocations[0];
        float nearestDistance = Distance(nearestRoomLocation, person);
        foreach (var roomLocation in roomLocations)
        {
            float distance = Distance(roomLocation, person);
            if(distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestRoomLocation = roomLocation;
            }
        }
        return nearestRoomLocation;
    }

    public static GameObject TheFarthestHidingPlaceInRoom(GameObject person, GameObject room)
    {
        GameObject[] roomLocations = room.GetComponent<RoomManager>().RoomLocations.ToArray();
        GameObject[] hidingPlaces = roomLocations.Where(rl => rl.GetComponent<RoomLocation>().HidingPlace).ToArray();
        GameObject farthestHidingPlace = hidingPlaces[0];
        float farthestDistance = Distance(farthestHidingPlace, person);
        foreach (var hidingPlace in hidingPlaces)
        {
            float distance = Distance(hidingPlace, person);
            if (distance > farthestDistance)
            {
                farthestDistance = distance;
                farthestHidingPlace = hidingPlace;
            }
        }
        return farthestHidingPlace;
    }

    public static float Distance(GameObject a, GameObject b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    public static void UpdateLimitForTask(PersonMemory memory, Command cmd, List<Task> Tasks)
    {
        Task task = Tasks.Where(a => a.Command == cmd).ToArray().ElementAtOrDefault(0);
        if (task == null) return;
        switch (cmd)
        {
            case Command.GO_UP:
                break;
            case Command.GO_DOWN:
                break;
            case Command.EXIT_BUILDING:
                break;
            case Command.SAY_ACTIVE_SHOOTER:
                break;
            case Command.STAY:
                break;
            case Command.GO_TO_ROOM:
                task.Limit.FoundRoom = memory.FoundRoom;
                break;
            case Command.FIND_ROOM:
                break;
            case Command.KNOCK:
                task.Limit.FoundRoom = memory.FoundRoom;
                break;
            case Command.OPEN_DOOR:
                break;
            case Command.GO_TO_DOOR:
                task.Limit.DoorToOpen = memory.CurrentRoom?.Door;
                break;
            case Command.ENTER_ROOM:
                task.Limit.FoundRoom = memory.FoundRoom;
                break;
            case Command.ASK_CLOSE_DOOR:
                task.Limit.FoundRoom = memory.FoundRoom;
                break;
            case Command.CLOSE_DOOR:
                task.Limit.DoorToOpen = memory.CurrentRoom?.Door;
                break;
            case Command.TELL_ABOUT_SHOOTER:
                task.Limit.FoundRoom = memory.FoundRoom;
                break;
            case Command.LOCK_DOOR:
                task.Limit.DoorToOpen = memory.CurrentRoom?.Door;
                break;
            case Command.HIDE_IN_CURRENT_ROOM:
                task.Limit.FoundRoom = memory.CurrentRoom;
                break;
            case Command.BLOCK_DOOR:
                task.Limit.Obstacle = memory.PickedObstacle;
                break;
            case Command.DESTROY_DOOR:
                task.Limit.FoundRoom = memory.FoundRoom;
                break;
            default:
                break;
        }
    }

    public static bool IsHiding(GameObject person, PersonMemory memory)
    {
        return memory.CurrentRoom != null;
    }

    public static bool IsInAnyRoom(PersonMemory memory)
    {
        return memory.CurrentRoom != null;
    }

    public static bool IsInRoom(GameObject person, Room room)
    {
        PersonMemory memory = person.GetComponent<Person>().PersonMemory;
        return memory.CurrentRoom != null && memory.CurrentRoom.Id == room.Id;
    }

    public static void TryCleanMyRoomGoUp(List<Task> tasks, PersonMemory memory)
    {
        if(memory.MyRoomIsAboveMe())
        {
            return;
        }
        tasks.Remove(tasks.Find(t => t.Command == Command.GO_UP));
    }

    public static void TryCleanMyRoomGoDown(List<Task> tasks, PersonMemory memory)
    {
        if (memory.MyRoomIsBelowMe())
        {
            return;
        }
        tasks.Remove(tasks.Find(t => t.Command == Command.GO_DOWN));
    }

    public static void CleanRequiredTasks(Task task, List<Task> tasks)
    {
        List<Task> tasksToRemove = new List<Task>();
        foreach (var item in task.RequiredTasks)
        {
            Task requiredTask = tasks.Find(t => t.Command == item.Command);
            if(requiredTask == null)
            {
                tasksToRemove.Add(item);
            }
        }
        foreach (var item in tasksToRemove)
        {
            task.RequiredTasks.Remove(item);
        }
    }

    public static void MoveAgent(GameObject agent, GameObject location)
    {
        agent.GetComponent<Rigidbody>().isKinematic = true;
        agent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        agent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezeRotationZ;
        agent.transform.position = location.transform.position;
        agent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        agent.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionY;
        agent.GetComponent<Rigidbody>().isKinematic = false;
        agent.GetComponent<Person>().PersonMemory.CurrentFloor = int.Parse(location.transform.parent.name.Split(' ')[1]);
    }

    public static List<GameObject> GetAllChilds(this GameObject Go)
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < Go.transform.childCount; i++)
        {
            list.Add(Go.transform.GetChild(i).gameObject);
        }
        return list;
    }

    public static bool RoomIsAbove(Room room, PersonMemory memory)
    {
        return memory.CurrentFloor < memory.GetRoomFloor(room.Id);
    }

    public static bool RoomIsBelow(Room room, PersonMemory memory)
    {
        return memory.CurrentFloor > memory.GetRoomFloor(room.Id);
    }

    public static bool RoomIsAtMyLevel(Room room, PersonMemory memory)
    {
        return memory.CurrentFloor == memory.GetRoomFloor(room.Id);
    }
}
