using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PersonGraph {
    public List<Node> Neighbours;
    public GameObject Me;
    public Node MeAsNode;

    public PersonGraph(GameObject me)
    {
        Me = me;
        MeAsNode = new Node
        {
            Name = Me.name,
            Position = Me.transform.position
        };
    }

    public Node[] GetNeighbours()
    {
        Neighbours = new List<Node>();
        PersonMemory memory = Me.GetComponent<Person>().PersonMemory;
        Node[] allNodes = memory.Graph[memory.CurrentFloor].AllNodes.Select(entry => entry.Value).ToArray();
        LayerMask layerMask = LayerMask.GetMask("Wall");
        foreach (var node in allNodes)
        {
            if(!Physics.Linecast(Me.transform.position, node.Position, layerMask))
            {
                Neighbours.Add(node);
            }
        }
        return Neighbours.ToArray();
    }

    public Node[] GetRoomNeighbours()
    {
        Neighbours = new List<Node>();
        PersonMemory memory = Me.GetComponent<Person>().PersonMemory;
        Node[] allNodes = memory.CurrentRoom.Reference.GetComponent<RoomManager>().RoomLocations.Select(rl => rl.GetComponent<RoomLocation>().MeAsNode).ToArray();
        LayerMask layerMask = LayerMask.GetMask("Wall", "Door", "Obstacle");
        foreach (var node in allNodes)
        {
            if (!Physics.Linecast(Me.transform.position, node.Position, layerMask))
            {
                Neighbours.Add(node);
            }
        }
        return Neighbours.ToArray();
    }
}
