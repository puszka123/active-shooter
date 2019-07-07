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
    public PersonBehaviours MyBehaviours;
    private List<Room> InformedRooms;
    Transform transform; //for testing!!!
    public ShooterInfo ShooterInfo;

    List<Node> blockedByDoor;

    public PersonMemory()
    {
        rand = new System.Random();
        Graph = new Dictionary<int, Graph>();
        CurrentFloor = 1;

        StartPosition = TargetPosition = null;
        ShooterInfo = new ShooterInfo(); //test
        //MyBehaviours = new PersonBehaviours();
        //Behaviour behaviour = new Behaviour();
        //behaviour.WorkBehaviour(MyRoom.Id);
        //MyBehaviours.AddBehaviour(behaviour);
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

    public void FindNearestLocation(Vector3 position)
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
                    //if(blockedByDoor != null)
                    //    foreach (var item in blockedByDoor.Select(n => n.Name))
                    //    {
                    //        Debug.Log(item);
                    //    }

                    if (blockedByDoor == null)
                    {
                        node = entry.Value;
                    }
                    else if (!blockedByDoor.Select(n => n.Name).Contains(entry.Value.Name))
                    {
                        node = entry.Value;
                    }

                }
            }
        }
        StartPosition = node;
        if (StartPosition == null)
        {
            Debug.Log("StartPosition is null");
        }
    }

    public Node[] GetBlockedNodes()
    {
        return blockedByDoor?.ToArray() ?? new Node[0];
    }

    public void AddBlockedNode(Node node)
    {
        if (blockedByDoor == null)
        {
            blockedByDoor = new List<Node>(new Node[] { node });
        }
        else if (!blockedByDoor.Select(n => n.Name).Contains(node.Name))
        {
            blockedByDoor.Add(node);
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
        MyRoom = new Room { Id = roomId };
        CurrentRoom = new Room { Id = roomId };
        MyBehaviours = new PersonBehaviours();
        Behaviour behaviour = null;
        if (transform.name == "Informer")
        {
            behaviour = new ImplementedBehaviours.InformRoom();
        }
        else
        {
            behaviour = new ImplementedBehaviours.Work(MyRoom.Id);
        }
        //Behaviour behaviour = new ImplementedBehaviours.RunToExit();
        MyBehaviours.AddBehaviour(behaviour);
    }

    public bool MyRoomIsAboveMe()
    {
        return CurrentFloor < GetRoomFloor(MyRoom.Id);
    }

    public int GetRoomFloor(string roomId)
    {
        GameObject room = GameObject.Find(roomId);
        return int.Parse(room.transform.parent.name.Split(' ')[1]);
    }

    public bool IsInMyRoom()
    {
        return CurrentRoom.Id == MyRoom.Id;
    }

    public bool IsOnTheFloor(int floor)
    {
        return CurrentFloor == floor;
    }

    public void AddInformedRoom(Room room)
    {
        if(room == null)
        {
            return;
        }
        if(InformedRooms == null)
        {
            InformedRooms = new List<Room>() { room };
        }
        else
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
            Reference = room
        };
        Debug.Log(CurrentRoom.Id);
    }

    public void ClearCurrentRoom()
    {
        CurrentRoom = null;
        Debug.Log(CurrentRoom);
    }
}
