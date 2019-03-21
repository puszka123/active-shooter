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
            if (item == "Door 1")
            {
                StartPosition = graph.GetNode(item);
            }
            if (item == "Door 6")
            {
                TargetPosition = graph.GetNode(item);
            }
        }
	}
}
