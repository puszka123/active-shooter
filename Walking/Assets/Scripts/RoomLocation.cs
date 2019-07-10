using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLocation : MonoBehaviour
{
    public GameObject MyRoom;
    public bool HidingPlace;
    public List<GameObject> Neighbours;
    public List<Node> NeighbourNodes;
    public Node MeAsNode;


    public void InitMyNeighbours(List<GameObject> roomLocations)
    {
        MeAsNode = new Node
        {
            Name = gameObject.name,
            Position = gameObject.transform.position
        };
        Neighbours = new List<GameObject>();
        NeighbourNodes = new List<Node>();
        LayerMask layerMask = LayerMask.GetMask("Wall", "Door", "Obstacle");
        foreach (var item in roomLocations)
        {
            if (item.name != gameObject.name
                && !Physics.Linecast(transform.position, item.transform.position, layerMask))
            {
                Neighbours.Add(item);
                NeighbourNodes.Add(new Node
                {
                    Name = item.name,
                    Position = item.transform.position
                });
            }
        }
    }

    public void InitDoorNeighbours(List<GameObject> roomLocations, GameObject room)
    {
        MeAsNode = new Node
        {
            Name = gameObject.name,
            Position = gameObject.transform.position
        };
        Neighbours = new List<GameObject>();
        NeighbourNodes = new List<Node>();
        LayerMask layerMask = LayerMask.GetMask("Wall", "Obstacle");
        foreach (var item in roomLocations)
        {
            if (item.name != gameObject.name
                && !Physics.Linecast(transform.position, item.transform.position, layerMask)
                && item.GetComponent<RoomLocation>().MyRoom.name == room.name)
            {
                Neighbours.Add(item);
                NeighbourNodes.Add(new Node
                {
                    Name = item.name,
                    Position = item.transform.position
                });
            }
        }
    }

    public void UpdateNode(string name)
    {
        MeAsNode = new Node
        {
            Name = name,
            Position = gameObject.transform.position
        };
        foreach (var item in Neighbours)
        {
            item.GetComponent<RoomLocation>().Neighbours.Add(gameObject);
            item.GetComponent<RoomLocation>().NeighbourNodes.Add(MeAsNode);
        }

    }
}
