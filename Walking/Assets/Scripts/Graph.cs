using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Graph
{
    Dictionary<string, Node> allNodes;
    Dictionary<string, List<Node>> nodes;
    public Dictionary<string, Node> AllNodes { get { return allNodes; } }
    public Dictionary<string, List<Node>> Nodes { get { return nodes; } }

    public Graph(int floor)
    {
        allNodes = new Dictionary<string, Node>();
        nodes = new Dictionary<string, List<Node>>();
        GameObject[] pathLocations = GameObject.FindGameObjectsWithTag("PathLocation")
            .Where(e => e.GetComponent<PathLocation>().Floor == floor).ToArray();
        foreach (GameObject pathLocation in pathLocations)
        {
            pathLocation.GetComponent<PathLocation>().FindMyNeighbours();
            nodes.Add(pathLocation.name, GetNeighbours(pathLocation.GetComponent<PathLocation>().NearestNeighbours));
        }
    }

    List<Node> GetNeighbours(List<GameObject> gNeighbours)
    {
        List<Node> neighbours = new List<Node>();
        foreach (var item in gNeighbours)
        {
            Node neighbour = new Node { Name = item.name, Position = item.transform.position };
            neighbours.Add(neighbour);
            if (!allNodes.ContainsKey(neighbour.Name)) allNodes.Add(neighbour.Name, neighbour);
        }

        return neighbours;
    }

    public Node[] GetNeighbours(Node node)
    {
        return nodes[node.Name].ToArray();
    }

    public Node GetNode(string key)
    {
        return allNodes[key];
    }
}
