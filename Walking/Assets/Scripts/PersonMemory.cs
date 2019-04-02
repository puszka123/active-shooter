using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonMemory {

    private Graph graph;
    public Node StartPosition;
    public Node TargetPosition;
    public Graph Graph { get { return graph; } }

	public PersonMemory() {
        graph = new Graph();

        StartPosition = TargetPosition = null;
        foreach (var item in graph.Nodes.Keys)
        {
            if (item == "Check Point (6)")
            {
                StartPosition = graph.GetNode(item);
            }
            if (item == "Check Point (19)")
            {
                TargetPosition = graph.GetNode(item);
            }
        }
	}

    public void setStartPosition(string name)
    {
        foreach (var item in graph.Nodes.Keys)
        {
            if (item == name)
            {
                StartPosition = graph.GetNode(item);
            }
        }
    }

    public void setTargetPosition(string name)
    {
        foreach (var item in graph.Nodes.Keys)
        {
            if (item == name)
            {
                TargetPosition = graph.GetNode(item);
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
                else if (Vector3.Distance(entry.Value.Position, position) < Vector3.Distance(position, node.Position)) node = entry.Value;
            }
        }
        StartPosition = node;
    }
}
