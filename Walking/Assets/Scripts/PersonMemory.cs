using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PersonMemory
{
    public Node StartPosition;
    public Node TargetPosition;
    public Graph Graph { get; private set; }

    List<Node> blockedByDoor;

    public PersonMemory()
    {
        Graph = new Graph();

        StartPosition = TargetPosition = null;
    }

    public void setStartPosition(string name)
    {
        foreach (var item in Graph.Nodes.Keys)
        {
            if (item == name)
            {
                StartPosition = Graph.GetNode(item);
            }
        }
    }

    public void setTargetPosition(string name)
    {
        foreach (var item in Graph.Nodes.Keys)
        {
            if (item == name)
            {
                TargetPosition = Graph.GetNode(item);
            }
        }
    }

    public void FindNearestLocation(Vector3 position)
    {
        Node node = null;
        foreach (KeyValuePair<string, Node> entry in Graph.AllNodes)
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
}
