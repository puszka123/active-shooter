using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PersonMemory
{
    public Node StartPosition;
    public Node TargetPosition;
    public Dictionary<int, Graph> Graph { get; private set; }
    System.Random rand;
    public int CurrentFloor;
    public Room CurrentRoom;
    public Room MyRoom;
    public Room FoundRoom;
    private List<Room> InformedRooms;
    public List<Room> CheckedRooms;
    public Dictionary<int, List<Room>> CheckedRoomsByFloor;
    public Transform transform;
    public ShooterInfo ShooterInfo;
    public List<GameObject> InformedPeople;
    public GameObject PickedObstacle;
    public GameObject CurrentStaircase;
    public bool IsAtStaircase;
    public List<int> AreAllCheckedRooms; //floor is an index

    //Active shooter
    public int BlockedRooms;

    Dictionary<string, List<Node>> blockedByDoor;
    List<Node> blockedByShooter;
    public List<Node> BlockedByShooter { get { return blockedByShooter != null ? new List<Node>(blockedByShooter) : null; } }

    public PersonMemory()
    {
        rand = new System.Random();
        Graph = new Dictionary<int, Graph>();
        CurrentFloor = 1;
        BlockedRooms = 0;
        StartPosition = TargetPosition = null;
    }

    public void Init(int floor, Transform transform) //it was vector3 position instead of transform, checkwhat better will be
    {
        this.transform = transform;
        CurrentFloor = floor;

        CheckedRoomsByFloor = new Dictionary<int, List<Room>>();
        foreach (var item in GameObject.FindGameObjectWithTag("SimulationManager").GetComponent<SimulationManager>().graphs)
        {
            Graph.Add(item.Key, item.Value);
        }
        //FindNearestLocation(transform.position);
        //setTargetPosition(RandomTarget().Name);
    }

    public void setStartPosition(string name)
    {
        foreach (var item in Graph[CurrentFloor].Nodes.Keys)
        {
            if (item == name)
            {
                StartPosition = Graph[CurrentFloor].GetNode(item);
            }
        }
    }

    public void setTargetPosition(string name)
    {
        foreach (var item in Graph[CurrentFloor].Nodes.Keys)
        {
            if (item == name)
            {
                TargetPosition = Graph[CurrentFloor].GetNode(item);
            }
        }
    }

    public Node FindNearestLocation(Vector3 position)
    {
        Node node = null;
        foreach (KeyValuePair<string, Node> entry in Graph[CurrentFloor].AllNodes)
        {
            int layerMask = 1 << 9;
            if (!Physics.Linecast(position, entry.Value.Position, layerMask))
            {
                if (node == null) node = entry.Value;
                else if (Utils.Distance(entry.Value.Position, position) < Utils.Distance(position, node.Position))
                {
                    if (blockedByDoor == null)
                    {
                        node = entry.Value;
                    }
                    else if (!blockedByDoor.Values.SelectMany(l => l).Select(n => n.Name).Contains(entry.Value.Name))
                    {
                        node = entry.Value;
                    }

                }
            }
        }
        if (StartPosition == null)
        {
            //Debug.Log(transform.name + " StartPosition is null");
        }
        return node;
    }

    public Node[] GetBlockedNodes()
    {
        return blockedByDoor?.Values?.SelectMany(l => l).ToArray() ?? new Node[0];
    }

    public void AddBlockedNode(GameObject door, Node node)
    {
        if (blockedByDoor == null)
        {
            blockedByDoor = new Dictionary<string, List<Node>>();
            blockedByDoor.Add(door.name, new List<Node>() { node });
        }
        else if (!blockedByDoor.Values.SelectMany(l => l).ToArray().Select(n => n.Name).Contains(node.Name))
        {
            if (blockedByDoor.ContainsKey(door.name))
            {
                blockedByDoor[door.name].Add(node);
            }
            else
            {
                blockedByDoor.Add(door.name, new List<Node>() { node });
            }
        }
    }

    public void clearBlockedByDoors()
    {
        blockedByDoor = null;
    }

    public Node RandomTarget()
    {
        int nodeIndex = rand.Next(Graph[CurrentFloor].AllNodes.Count);
        return Graph[CurrentFloor].AllNodes.ElementAt(nodeIndex).Value;
    }

    public void SetMyRoom(string roomId)
    {
        if (transform.gameObject.tag == "ActiveShooter")
        {
            return;
        }

        GameObject myRoom = GameObject.Find(roomId);
        MyRoom = new Room
        {
            Id = roomId,
            Door = myRoom.GetComponent<PathLocation>().RoomDoor,
            Employees = myRoom.GetComponent<PathLocation>().RoomEmployees.ToArray(),
            Reference = myRoom,
        };
        CurrentRoom = new Room
        {
            Id = roomId,
            Door = myRoom.GetComponent<PathLocation>().RoomDoor,
            Employees = myRoom.GetComponent<PathLocation>().RoomEmployees.ToArray(),
            Reference = myRoom,
        };
    }

    public bool MyRoomIsAboveMe()
    {
        return CurrentFloor < GetRoomFloor(MyRoom.Id);
    }

    public bool MyRoomIsBelowMe()
    {
        return CurrentFloor > GetRoomFloor(MyRoom.Id);
    }

    public int GetRoomFloor(string roomId)
    {
        GameObject room = GameObject.Find(roomId);
        return int.Parse(room.transform.parent.name.Split(' ')[1]);
    }

    public bool IsInMyRoom()
    {
        if (CurrentRoom == null) return false;
        return CurrentRoom.Id == MyRoom.Id;
    }

    public bool IsOnTheFloor(int floor)
    {
        return CurrentFloor == floor;
    }

    public void AddInformedRoom(Room room)
    {
        if (room == null)
        {
            return;
        }
        if (InformedRooms == null)
        {
            InformedRooms = new List<Room>() { room };
        }
        else if (!InformedRooms.Select(r => r.Id).Contains(room.Id))
        {
            InformedRooms.Add(room);
        }
    }

    public Room DeleteInformedRoomById(string roomId)
    {
        List<Room> rooms = InformedRooms.Where(room => room.Id == roomId).Select(room => room).ToList();
        if (rooms.Count == 0) return null;
        Room deletedRoom = rooms[0];
        InformedRooms = InformedRooms.Where(room => room.Id != roomId).Select(room => room).ToList();
        return deletedRoom;
    }

    public void DeleteInformedRooms()
    {
        InformedRooms = null;
    }

    public List<Room> GetInformedRooms()
    {
        List<Room> informedRooms = new List<Room>();
        if (InformedRooms == null) return informedRooms;
        foreach (var item in InformedRooms)
        {
            informedRooms.Add(new Room { Id = item.Id });
        }

        return informedRooms;
    }

    public void SaveFoundRoom(Room room)
    {
        FoundRoom = room;
    }

    public bool InformedRoom(GameObject room)
    {
        if (InformedRooms == null) return false;
        return InformedRooms.Select(r => r.Id).Contains(room.name);
    }

    public bool CheckedRoom(GameObject room)
    {
        if (CheckedRooms == null) return false;
        return CheckedRooms.Select(r => r.Id).Contains(room.name);
    }

    public void ClearFoundRoom()
    {
        FoundRoom = null;
    }

    public void SaveCurrentRoom(GameObject room)
    {
        bool wasNull = false;
        if (CurrentRoom == null)
        {
            wasNull = true;
        }
        CurrentRoom = new Room
        {
            Id = room.name,
            Door = room.GetComponent<PathLocation>().RoomDoor,
            Employees = room.GetComponent<PathLocation>().RoomEmployees.ToArray(),
            Reference = room,
        };
        ClearRoomBlockedNode(CurrentRoom);
        if (wasNull && !transform.CompareTag("ActiveShooter"))
        {
            transform.SendMessage("SelectBehaviour");
        }
    }

    public void ClearCurrentRoom()
    {
        CurrentRoom = null;
    }

    public void ClearRoomBlockedNode(Room room)
    {
        string key = room.Door.name;
        if (blockedByDoor != null && blockedByDoor.ContainsKey(key))
        {
            blockedByDoor[key] = blockedByDoor[key].Where(n => n.Name != room.Reference.name).ToList();
        }
    }

    public void ClearRoomBlockedNode(GameObject door)
    {
        string key = door.name;
        string roomName = door.GetComponent<DoorController>().MyRoom.name;
        if (blockedByDoor != null && blockedByDoor.ContainsKey(key))
        {
            blockedByDoor[key] = blockedByDoor[key].Where(n => n.Name != roomName).ToList();
        }
    }

    public void UpdateBlockedNodes()
    {
        if (blockedByDoor == null) return;

        List<string> keysToClear = new List<string>();
        Dictionary<string, List<Node>> temp = new Dictionary<string, List<Node>>(blockedByDoor);
        foreach (var key in temp.Keys)
        {
            if (!GameObject.Find(key).GetComponent<DoorController>().IsLocked)
            {
                keysToClear.Add(key);
            }
        }

        foreach (var key in keysToClear)
        {
            blockedByDoor[key] = new List<Node>();
        }
    }

    public void AddInformedPerson(GameObject person)
    {
        if (InformedPeople == null)
        {
            InformedPeople = new List<GameObject> { person };
        }
        else
        {
            InformedPeople.Add(person);
        }
    }

    public void AddInformedPeople(IEnumerable<GameObject> people)
    {
        if (InformedPeople == null)
        {
            InformedPeople = new List<GameObject>(people);
        }
        else
        {
            InformedPeople.AddRange(people);
        }
    }

    public bool AreInformed(IEnumerable<GameObject> people)
    {
        if (InformedPeople == null) return false;

        foreach (var person in people)
        {
            if (!InformedPeople.Select(p => p.name).Contains(person.name))
            {
                return false;
            }
        }
        return true;
    }

    public GameObject GetNearestObstacle(GameObject person)
    {
        if (CurrentRoom.Obstacles.Count == 0) return null;
        GameObject nearestObstacle = CurrentRoom.Obstacles[0];
        float nearestDistance = Utils.Distance(nearestObstacle, person);
        foreach (var item in CurrentRoom.Obstacles)
        {
            float distance = Utils.Distance(item, person);
            if (distance < nearestDistance)
            {
                nearestObstacle = item;
                nearestDistance = distance;
            }
        }

        return nearestObstacle;
    }

    public void PickObstacle(GameObject obstacle)
    {
        PickedObstacle = obstacle;
    }

    public void PutObstacle()
    {
        PickedObstacle = null;
    }

    public GameObject GetCurrentStaircase()
    {
        return CurrentStaircase;
    }

    public void AddCurrentStaircase(GameObject staircase)
    {
        CurrentStaircase = staircase;
    }

    public void ClearCurrentStaircase()
    {
        CurrentStaircase = null;
    }

    public void ToggleIsAtStaircase()
    {
        IsAtStaircase = !IsAtStaircase;
    }

    public void UpdateNodesBlockedByShooter()
    {
        if (transform.CompareTag("ActiveShooter")
            || ShooterInfo == null
            || Utils.IsNullVector(ShooterInfo.Position)) return;
        ClearBlockedNodesByShooter();


        Vector3 shooterPosition = ShooterInfo.Position;
        int shooterFloor = ShooterInfo.Floor;

        LayerMask layerMask = LayerMask.GetMask("Wall");
        foreach (KeyValuePair<string, Node> entry in Graph[CurrentFloor].AllNodes)
        {
            if (!Physics.Linecast(shooterPosition, entry.Value.Position, layerMask))
            {
                AddNodeBlockedByShooter(entry.Value);
            }
        }
    }

    public void AddNodeBlockedByShooter(Node node)
    {
        if (transform.CompareTag("ActiveShooter"))
        {
            return;
        }
        if (blockedByShooter == null)
        {
            blockedByShooter = new List<Node> { node };
        }
        else if (!blockedByShooter.Select(n => n.Name).Contains(node.Name))
        {
            blockedByShooter.Add(node);
        }
    }

    public void UpdateActiveShooterInfo(GameObject activeShooter, bool canSee)
    {
        bool shooterNull = false;
        if (ShooterInfo == null)
        {
            shooterNull = true;
        }
        Vector3 shooterPos = activeShooter.transform.position;
        ShooterInfo = new ShooterInfo
        {
            Position = canSee ? new Vector3(shooterPos.x, shooterPos.y, shooterPos.z) : Resources.NullVector,
            Name = activeShooter.name,
            Floor = activeShooter.GetComponent<Person>().PersonMemory.CurrentFloor
        };
    }

    public bool UpdateActiveShooterInfo(ShooterInfo activeShooter)
    {
        bool firstTime = false;
        if (ShooterInfo == null)
        {
            ShooterInfo = new ShooterInfo
            {
                Position = activeShooter.Position,
                Name = activeShooter.Name,
                Floor = activeShooter.Floor
            };
            firstTime = true;
        }
        return firstTime;
    }

    public void ClearBlockedNodesByShooter()
    {
        blockedByShooter = null;
    }

    public void AddCheckedRoom(Room room)
    {
        if (room == null)
        {
            return;
        }
        if (CheckedRooms == null)
        {
            CheckedRooms = new List<Room>() { room };
        }
        else if (!CheckedRooms.Select(r => r.Id).Contains(room.Id))
        {
            CheckedRooms.Add(room);
        }

        if (CheckedRoomsByFloor.ContainsKey(CurrentFloor))
        {
            CheckedRoomsByFloor[CurrentFloor].Add(room);
        }
        else
        {
            CheckedRoomsByFloor.Add(CurrentFloor, new List<Room> { room });
        }
    }

    public void ClearBlockedRooms()
    {
        BlockedRooms = 0;
    }

    public void AddBlockedRoom()
    {
        BlockedRooms++;
    }

    public void SetCurrentFloor(int value)
    {
        CurrentFloor = value;
        if (transform.CompareTag("ActiveShooter"))
        {
            transform.SendMessage("SelectAction");
        }
    }

    public void SetAllChecked()
    {
        if (AreAllCheckedRooms == null)
        {
            AreAllCheckedRooms = new List<int> { CurrentFloor };
        }
        else
        {
            AreAllCheckedRooms.Add(CurrentFloor);
        }
    }
}

