using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            if (item.GetComponent<PathLocation>().IsStairsway && !BlockedByShooter(item.gameObject, memory))
            {
                stairs.Add(item.gameObject);
            }
            if (stairs.Count >= 2) break;
        }
        GameObject nearestStairs = null;
        if (stairs.Count == 0) return null;
        if (stairs.Count == 1)
        {
            nearestStairs = stairs[0];
        }
        else
        {
            nearestStairs = Distance(transform.gameObject, stairs[0]) < Distance(transform.gameObject, stairs[1])
                ? stairs[0] : stairs[1];
        }
        PathLocation pl = nearestStairs.GetComponent<PathLocation>();
        if (pl.MyStairs[0].GetComponent<PathLocation>().StairsDirection == stairsType)
        {
            return pl.MyStairs[0].name;
        }
        else
        {
            return pl.MyStairs[1].name;
        }
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
            if (Utils.Distance(transform.position, nearestExits.transform.position)
                >
                Utils.Distance(transform.position, item.transform.position))
            {
                nearestExits = item;
            }
        }
        return nearestExits.name;
    }

    public static bool ToFar(GameObject a, GameObject b, float threshold = 0.25f)
    {
        return Utils.Distance(a.transform.position, b.transform.position) > threshold;
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
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestRoomLocation = roomLocation;
            }
        }
        return nearestRoomLocation;
    }

    public static GameObject FarthestHidingPlaceInRoom(GameObject person, GameObject room)
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

    public static GameObject NearestHidingPlaceInRoom(GameObject person, GameObject room)
    {
        GameObject[] roomLocations = room.GetComponent<RoomManager>().RoomLocations.ToArray();
        GameObject[] hidingPlaces = roomLocations.Where(rl => rl.GetComponent<RoomLocation>().HidingPlace).ToArray();
        if (hidingPlaces.Length == 0) return null;
        GameObject nearestHidingPlace = hidingPlaces[0];
        float nearestDistance = Distance(nearestHidingPlace, person);
        foreach (var hidingPlace in hidingPlaces)
        {
            float distance = Distance(hidingPlace, person);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestHidingPlace = hidingPlace;
            }
        }
        return nearestHidingPlace;
    }

    public static float Distance(GameObject a, GameObject b)
    {
        Vector3 vecA = a.transform.position;
        Vector3 vecB = b.transform.position;
        vecA.y = 0;
        vecB.y = 0;
        return Vector3.Distance(vecA, vecB);
    }

    public static float Distance(Vector3 a, Vector3 b)
    {
        a.y = 0;
        b.y = 0;
        return Vector3.Distance(a, b);
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
            case Command.WORK:
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
            case Command.CHECK_ROOM:
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
            case Command.BARRICADE_DOOR:
                task.Limit.DoorToOpen = memory.CurrentRoom?.Door;
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
        if (memory.MyRoomIsAboveMe())
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
            if (requiredTask == null)
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

    public static bool CanSee(GameObject a, GameObject b)
    {
        Vector3 aHead;
        Vector3 bHead;
        if (a.CompareTag("ActiveShooter"))
        {
            aHead = a.transform.GetChild(0).position;
        }
        else
        {
            if (a.GetComponent<Person>().MyState.IsHiding)
            {
                aHead = a.transform.GetChild(2).position;
            }
            else
            {
                aHead = a.transform.GetChild(1).position;
            }
        }

        if (b.CompareTag("ActiveShooter"))
        {
            bHead = b.transform.GetChild(0).position;
        }
        else
        {
            if (b.GetComponent<Person>().MyState.IsHiding)
            {
                bHead = b.transform.GetChild(2).position;
            }
            else
            {
                bHead = b.transform.GetChild(1).position;
            }
        }

        LayerMask layerMask = LayerMask.GetMask("Wall", "Door", "ObstacleCollider");
        return !Physics.Linecast(aHead, bHead, layerMask);
    }

    public static bool AlreadyBarricaded(GameObject door)
    {
        return door.GetComponent<DoorController>().Obstacles == 2;
    }

    public static Node GetNodeToRunAway(GameObject person, GameObject shooter, bool update = false)
    {
        PersonMemory memory = person.GetComponent<Person>().PersonMemory;
        Node node = null;
        LayerMask layerMask = LayerMask.GetMask("Wall", "Door");
        node = GetVisibleNodeByPersonOnly(person, shooter, memory, node, layerMask);
        if (node == null)
        {
            if (update) { return null; }
            node = GetVisibleNodeByBoth(person, shooter, memory, node, layerMask);
        }
        if (node == null)
        {
            Debug.Log(person.name + " run away is null");
        }
        if (person.gameObject.name == "employee 32")
        {
            //Debug.Log(node.Name);
        }
        return node;
    }

    private static Node GetVisibleNodeByBoth(GameObject person, GameObject shooter, PersonMemory memory, Node node, LayerMask layerMask)
    {
        foreach (KeyValuePair<string, Node> entry in memory.Graph[memory.CurrentFloor].AllNodes)
        {
            if (!Physics.Linecast(person.transform.position, entry.Value.Position, layerMask))
            {
                Vector3 nodePerson = entry.Value.Position - person.transform.position;
                Vector3 personShooter = shooter.transform.position - person.transform.position;
                float angleBetween = Vector3.Angle(nodePerson, personShooter);
                if (node == null) node = entry.Value;
                else if (angleBetween > 60f &&
                    Utils.Distance(entry.Value.Position, shooter.transform.position) > Utils.Distance(shooter.transform.position, node.Position))
                {
                    if (memory.GetBlockedNodes().Length == 0)
                    {
                        node = entry.Value;
                    }
                    else if (!memory.GetBlockedNodes().Select(n => n.Name).Contains(entry.Value.Name))
                    {
                        node = entry.Value;
                    }

                }
            }
        }

        return node;
    }

    private static Node GetVisibleNodeByPersonOnly(GameObject person, GameObject shooter, PersonMemory memory, Node node, LayerMask layerMask)
    {
        foreach (KeyValuePair<string, Node> entry in memory.Graph[memory.CurrentFloor].AllNodes)
        {
            if (!Physics.Linecast(person.transform.position, entry.Value.Position, layerMask)
                && Physics.Linecast(shooter.transform.position, entry.Value.Position, layerMask))
            {
                Vector3 nodePerson = entry.Value.Position - person.transform.position;
                Vector3 personShooter = shooter.transform.position - person.transform.position;
                float angleBetween = Vector3.Angle(nodePerson, personShooter);
                if (node == null) node = entry.Value;
                else if (angleBetween > 60f &&
                    Utils.Distance(entry.Value.Position, shooter.transform.position) > Utils.Distance(shooter.transform.position, node.Position))
                {
                    if (memory.GetBlockedNodes().Length == 0)
                    {
                        node = entry.Value;
                    }
                    else if (!memory.GetBlockedNodes().Select(n => n.Name).Contains(entry.Value.Name))
                    {
                        node = entry.Value;
                    }

                }
            }
        }
        if (node == null)
        {
            return null;
        }
        Vector3 finalNodePerson = node.Position - person.transform.position;
        Vector3 finalPersonShooter = shooter.transform.position - person.transform.position;
        float finalAngleBetween = Vector3.Angle(finalNodePerson, finalPersonShooter);
        if (finalAngleBetween >= 60f)
        {
            return node;
        }
        else
        {
            return null;
        }
    }

    public static bool BlockedByShooter(GameObject pathLocation, PersonMemory memory)
    {
        if (memory.BlockedByShooter == null || memory.BlockedByShooter.Count == 0) return false;
        return memory.BlockedByShooter.Select(n => n.Name).Contains(pathLocation.name);
    }

    public static GameObject GetMyWorkplace(PersonMemory memory)
    {
        return memory.MyRoom.Reference.GetComponent<RoomManager>().RoomLocations.Find(r => r.GetComponent<RoomLocation>().Workplace
        && r.GetComponent<RoomLocation>().WorkEmployee != null
        && r.GetComponent<RoomLocation>().WorkEmployee.name == memory.transform.name);
    }

    public static GameObject[] GetEmployeesInRoom(GameObject room)
    {
        //repair
        return GameObject.FindGameObjectsWithTag("Employee").Where(e => !e.name.EndsWith("Origin")
        && e.GetComponent<Person>().PersonMemory.CurrentRoom?.Id == room.name).ToArray();
    }

    public static GameObject GetNearestEmployee(GameObject respawn, int floor)
    {
        float nearestDist = 999f;
        GameObject nearestEmployee = null;
        foreach (Transform employee in GameObject.Find("Employees " + floor).transform)
        {
            float distance = Utils.Distance(respawn.transform.position, employee.transform.position);
            if (distance < nearestDist)
            {
                nearestDist = distance;
                nearestEmployee = employee.gameObject;
            }
        }
        return nearestEmployee;
    }
}
