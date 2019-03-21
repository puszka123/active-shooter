using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class Pathfinder {

    public const int MAX_NODES_IN_QUEUE = 100;
    public const float MIN_DISTANCE = 1.5f;

    public float GetGoalAngle(GameObject person, Vector3 destination)
    {
        return Vector3.SignedAngle(destination - person.transform.position, person.transform.forward, Vector3.up)*(-1.0f);
    }

    private float Heuristic(Node a, Node b)
    {
        return Vector3.Distance(a.Position, b.Position);
    }

    public bool CheckDistance(GameObject agent, Node target)
    {
        //Debug.Log(Vector3.Distance(agent.transform.position, target.Position));
        return Vector3.Distance(agent.transform.position, target.Position) <= MIN_DISTANCE;
    }

    public List<Node> FindWay(Graph graph, Node startPosition, Node targetPosition)
    {
        FastPriorityQueue<Node> frontier = new FastPriorityQueue<Node>(MAX_NODES_IN_QUEUE);
        frontier.Enqueue(startPosition, 0);
        Dictionary<string, Node> cameFrom = new Dictionary<string, Node>();
        Dictionary<string, float> costSoFar = new Dictionary<string, float>();
        cameFrom.Add(startPosition.Name, null);
        costSoFar.Add(startPosition.Name, 0);

        while(frontier.Count > 0)
        {
            Node current = frontier.Dequeue();
            if(current.Name == targetPosition.Name)
            {
                break;
            }

            foreach(Node next in graph.GetNeighbours(current))
            {
                float newCost = costSoFar[current.Name] + Vector3.Distance(current.Position, next.Position);
                if (!costSoFar.ContainsKey(next.Name) || newCost < costSoFar[next.Name])
                {
                    costSoFar[next.Name] = newCost;
                    float priority = newCost + Heuristic(targetPosition, next);
                    frontier.Enqueue(next, priority);
                    if (cameFrom.ContainsKey(next.Name)) cameFrom[next.Name] = current;
                    else cameFrom.Add(next.Name, current);
                    //Debug.Log(next.Name + " " + cameFrom[next].Name);
                }
            }
        }

        List<Node> desiredPath = new List<Node>();
        desiredPath.Add(targetPosition);
        Node start = cameFrom[targetPosition.Name];
        while (start != null)
        {
            desiredPath.Add(start);
            //Debug.Log(start.Name);
            start = cameFrom[start.Name];
        }
        desiredPath.Reverse();
        return desiredPath;
    }
}
