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
    public PersonActions MyActions;
    private List<Room> InformedRooms;
    public Transform transform;
    public ShooterInfo ShooterInfo;
    public List<GameObject> InformedPeople;
    public GameObject PickedObstacle;
    public GameObject CurrentStaircase;
    public bool IsAtStaircase;

    Dictionary<string, List<Node>> blockedByDoor;
    List<Node> blockedByShooter;
    public List<Node> BlockedByShooter { get { return new List<Node>(blockedByShooter); } }

    public PersonMemory()
    {
        rand = new System.Random();
        Graph = new Dictionary<int, Graph>();
        CurrentFloor = 1;

        StartPosition = TargetPosition = null;
        //ShooterInfo = new ShooterInfo(); //test
        //MyActions = new PersonActions();
        //Action action = new Action();
        //action.WorkAction(MyRoom.Id);
        //MyActions.AddAction(action);
    }

    public void Init(int floor, Transform transform) //it was vector3 position instead of transform, checkwhat better will be
    {
        this.transform = transform;
        CurrentFloor = floor;
        foreach (var item in GameObject.FindGameObjectsWithTag("Floor"))
        {
            int f = int.Parse(item.name.Split(' ')[1]);
            Graph.Add(f, new Graph(f));
        }
        FindNearestLocation(transform.position);
        setTargetPosition(RandomTarget().Name);
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
                else if (Vector3.Distance(entry.Value.Position, position) < Vector3.Distance(position, node.Position))
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
            MyActions = new PersonActions();
            Action actionShooter = new ShooterActions.GoToAnyRoom();
            MyActions.AddAction(actionShooter);
            return;
        }
        if (transform.gameObject.name.StartsWith("Fighter"))
        {
            MyActions = new PersonActions();
            Action actionFighter = new EvacuationActions.RunAway();
            MyActions.AddAction(actionFighter);
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
        if (transform.name != "Informer") //test
            CurrentRoom = new Room
            {
                Id = roomId,
                Door = myRoom.GetComponent<PathLocation>().RoomDoor,
                Employees = myRoom.GetComponent<PathLocation>().RoomEmployees.ToArray(),
                Reference = myRoom,
            };
        MyActions = new PersonActions();
        Action action = null;
        if (transform.name == "Informer")
        {
            action = new EvacuationActions.RunToExit();
        }
        else if (transform.name.StartsWith("Fighter"))
        {
            action = new FightActions.Fight();
        }
        else
        {
            //action = new WorkActions.GoToWork(MyRoom);
            //action = new WorkActions.LeaveWork();
            //action = new EvacuationActions.RunToExit();
            //action = new HideActions.HideInCurrentRoom();
            //action = new HideActions.LockCurrentRoom();
            //action = new FightActions.Fight();
            action = new HideActions.BarricadeDoor();
        }
        //Action action = new ImplementedActions.RunToExit();
        MyActions.AddAction(action);
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

    public void ClearFoundRoom()
    {
        FoundRoom = null;
    }

    public void SaveCurrentRoom(GameObject room)
    {
        CurrentRoom = new Room
        {
            Id = room.name,
            Door = room.GetComponent<PathLocation>().RoomDoor,
            Employees = room.GetComponent<PathLocation>().RoomEmployees.ToArray(),
            Reference = room,
        };
        ClearRoomBlockedNode(CurrentRoom);
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
        Debug.Log(transform.name + " " + IsAtStaircase);
    }

    public void UpdateNodesBlockedByShooter()
    {
        if (ShooterInfo == null) return;

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
        if(blockedByShooter == null)
        {
            blockedByShooter = new List<Node> { node };
        }
        else if(!blockedByShooter.Select(n => n.Name).Contains(node.Name))
        {
            blockedByShooter.Add(node);
        }
    }

    public void UpdateActiveShooterInfo(GameObject activeShooter)
    {
        Vector3 shooterPos = activeShooter.transform.position;
        ShooterInfo = new ShooterInfo
        {
            Position = new Vector3(shooterPos.x, shooterPos.y, shooterPos.z),
            Name = activeShooter.name,
        };
    }
}

