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
    public PersonBehaviours MyBehaviours;

    List<Node> blockedByDoor;

    public PersonMemory()
    {
        rand = new System.Random();
        Graph = new Dictionary<int, Graph>();
        CurrentFloor = 1;

        StartPosition = TargetPosition = null;

        MyBehaviours = new PersonBehaviours();
        Behaviour behaviour = new Behaviour();
        behaviour.RunToExitBehaviour();
        MyBehaviours.AddBehaviour(behaviour);
    }

    public void Init(int floor, Vector3 position)
    {

        CurrentFloor = floor;
        foreach (var item in GameObject.FindGameObjectsWithTag("Floor"))
        {
            int f = int.Parse(item.name.Split(' ')[1]);
            Graph.Add(f, new Graph(f));
        }
        FindNearestLocation(position);
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
            //Debug.Log("StartPosition is null");
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
}
