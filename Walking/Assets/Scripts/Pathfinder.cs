using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System.Linq;

public class Pathfinder {

    public const int MAX_NODES_IN_QUEUE = 100;
    public const float MIN_DISTANCE = 0.2f;
    public const float MIN_DISTANCE_ROOM = 0.1f;
    public const float MIN_DISTANCE_OBSTACLE = 0.1f;

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

    public bool CheckDistance(GameObject agent, Node target, Command cmd)
    {
        switch (cmd)
        {
            case Command.PICK_NEAREST_OBSTACLE:
                return Vector3.Distance(agent.transform.position, target.Position) <= MIN_DISTANCE_OBSTACLE;
            default:
                return Vector3.Distance(agent.transform.position, target.Position) <= MIN_DISTANCE;
        }
    }

    public bool CheckDistanceRoom(GameObject agent, Node target)
    {
        //Debug.Log(Vector3.Distance(agent.transform.position, target.Position));
        return Vector3.Distance(agent.transform.position, target.Position) <= MIN_DISTANCE_ROOM;
    }

    FastPriorityQueue<Node> frontier = new FastPriorityQueue<Node>(MAX_NODES_IN_QUEUE);
    public List<Node> FindWay(Graph graph, Node startPosition, Node targetPosition, PersonMemory personMemory)
    {
        //first check if blocked nodes are still blocked!!!
        personMemory.UpdateBlockedNodes();

        if (!CheckInputs(graph, startPosition, targetPosition)) return null;
        frontier.Clear();
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
                //if a node is blocked by a door (a person doesn't have a keys) then d not consider the node and find another way
                if(personMemory.GetBlockedNodes().Select(node => node.Name).Contains(next.Name))
                {
                    //Debug.Log("Pathfinder: " + next.Name + " " + startPosition.Name);
                    continue;
                }
                float newCost = costSoFar[current.Name] + Vector3.Distance(current.Position, next.Position);
                if (!costSoFar.ContainsKey(next.Name) || newCost < costSoFar[next.Name])
                {
                    costSoFar[next.Name] = newCost;
                    float priority = newCost + Heuristic(targetPosition, next);
                    frontier.Enqueue(next, priority);
                    if (cameFrom.ContainsKey(next.Name)) cameFrom[next.Name] = current;
                    else cameFrom.Add(next.Name, current);
                }
            }
        }

        List<Node> desiredPath = new List<Node>();
        desiredPath.Add(targetPosition);
        if(!cameFrom.ContainsKey(targetPosition.Name))
        {
            return new List<Node>();
        }
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

    private bool CheckInputs(Graph graph, Node startPosition, Node targetPosition)
    {
        return !(graph == null || startPosition == null || targetPosition == null);
    }

    private bool CheckInputs(GameObject startPosition, GameObject targetPosition)
    {
        return !(startPosition == null || targetPosition == null);
    }

    //for room
    FastPriorityQueue<Node> frontierRoom = new FastPriorityQueue<Node>(MAX_NODES_IN_QUEUE);
    public List<Node> FindWay(GameObject startPosition, GameObject targetPosition, PersonMemory personMemory)
    {
        if (!CheckInputs(startPosition, targetPosition)) return null;
        RoomLocation start = startPosition.GetComponent<RoomLocation>();
        RoomLocation target = targetPosition.GetComponent<RoomLocation>();     
        frontierRoom.Clear();
        frontierRoom.Enqueue(new Node(start.MeAsNode), 0);
        Dictionary<string, Node> cameFrom = new Dictionary<string, Node>();
        Dictionary<string, float> costSoFar = new Dictionary<string, float>();
        cameFrom.Add(start.MeAsNode.Name, null);
        costSoFar.Add(start.MeAsNode.Name, 0);

        while (frontierRoom.Count > 0)
        {
            Node current = frontierRoom.Dequeue();
            if (current.Name == target.MeAsNode.Name)
            {
                break;
            }
            
            foreach (Node next in GameObject.Find(current.Name).GetComponent<RoomLocation>().NeighbourNodes)
            {
                float newCost = costSoFar[current.Name] + Vector3.Distance(current.Position, next.Position);
                if (!costSoFar.ContainsKey(next.Name) || newCost < costSoFar[next.Name])
                {
                    costSoFar[next.Name] = newCost;
                    float priority = newCost + Heuristic(target.MeAsNode, next);
                    frontierRoom.Enqueue(new Node(next), priority);
                    if (cameFrom.ContainsKey(next.Name)) cameFrom[next.Name] = current;
                    else cameFrom.Add(next.Name, current);
                }
            }
        }

        List<Node> desiredPath = new List<Node>();
        desiredPath.Add(target.MeAsNode);
        if (!cameFrom.ContainsKey(target.MeAsNode.Name))
        {
            return new List<Node>();
        }
        Node startNode = cameFrom[target.MeAsNode.Name];
        while (startNode != null)
        {
            desiredPath.Add(startNode);
            startNode = cameFrom[startNode.Name];
        }
        desiredPath.Reverse();
        return desiredPath;
    }
}
