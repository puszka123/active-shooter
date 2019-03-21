using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonMemory : MonoBehaviour {
    public List<Node> knownLocations;

    Graph graph;
	// Use this for initialization
	void Start () {
        graph = new Graph();
        Pathfinder pathfinder = new Pathfinder();
        Node startPosition, targetPosition;
        startPosition = targetPosition = null;
        foreach (var item in graph.Nodes.Keys)
        {
            if (item == "Door 1")
            {
                startPosition = graph.GetNode(item);
            }
            if (item == "Door 6")
            {
                targetPosition = graph.GetNode(item);
            }
        }
        pathfinder.FindWay(graph, startPosition, targetPosition);
	}
}
