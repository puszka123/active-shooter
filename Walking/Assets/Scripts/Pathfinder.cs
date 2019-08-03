using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System.Linq;

public class Pathfinder {

    public const int MAX_NODES_IN_QUEUE = 100;
    public const float MIN_DISTANCE = 0.175f;
    public const float MIN_DISTANCE_DOOR = 0.3f;
    public const float MIN_DISTANCE_HIDE = 0.095f;
    public const float MIN_DISTANCE_ROOM = 0.1f;
    public const float MIN_DISTANCE_ROOM_WALK = 0.15f;
    public const float MIN_DISTANCE_OBSTACLE = 0.1f;
    public const float MIN_DISTANCE_STAIRS = 0.1f;
    public const float MIN_DISTANCE_WORKPLACE = 0.025f;

    public float GetGoalAngle(GameObject person, Vector3 destination)
    {
        return Vector3.SignedAngle(destination - person.transform.position, person.transform.forward, Vector3.up)*(-1.0f);
    }

    private float Heuristic(Node a, Node b)
    {
        return Utils.Distance(a.Position, b.Position);
    }

    public bool CheckDistance(GameObject agent, Node target)
    {
        if (Utils.IsInAnyRoom(agent.GetComponent<Person>().PersonMemory))
        {
            return Utils.Distance(agent.transform.position, target.Position) <= MIN_DISTANCE_ROOM_WALK;
        }
        return Utils.Distance(agent.transform.position, target.Position) <= MIN_DISTANCE;
    }

    public bool CheckDistance(GameObject agent, Node target, Command cmd)
    {
        switch (cmd)
        {
            case Command.PICK_NEAREST_OBSTACLE:
                return Utils.Distance(agent.transform.position, target.Position) <= MIN_DISTANCE_OBSTACLE;
            case Command.GO_TO_WORKPLACE:
                return Utils.Distance(agent.transform.position, target.Position) <= MIN_DISTANCE_WORKPLACE;
            case Command.HIDE_IN_CURRENT_ROOM:
                return Utils.Distance(agent.transform.position, target.Position) <= MIN_DISTANCE_HIDE;
            case Command.GO_TO_DOOR:
                return Utils.Distance(agent.transform.position, target.Position) <= MIN_DISTANCE_DOOR;
            case Command.GO_DOWN:
            case Command.GO_UP:
                return Utils.Distance(agent.transform.position, target.Position) <= MIN_DISTANCE_STAIRS;
            case Command.ENTER_ROOM:
                return Utils.Distance(agent.transform.position, target.Position) <= MIN_DISTANCE_ROOM;
            default:
                return Utils.Distance(agent.transform.position, target.Position) <= MIN_DISTANCE;
        }
    }

    public bool CheckDistanceRoom(GameObject agent, Node target)
    {
        return Utils.Distance(agent.transform.position, target.Position) <= MIN_DISTANCE_ROOM;
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
                float newCost = costSoFar[current.Name] + Utils.Distance(current.Position, next.Position);
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
    public List<Node> FindWay(GameObject person, GameObject targetPosition, PersonMemory personMemory)
    {
        if (!CheckInputs(person, targetPosition)) return null;
        PersonGraph personGraph = new PersonGraph(person);
        Node[] personNeighbours = personGraph.GetRoomNeighbours();
        //RoomLocation start = startPosition.GetComponent<RoomLocation>();
        RoomLocation target = targetPosition.GetComponent<RoomLocation>();  
        if(target == null)
        {
            Debug.Log("Bug: " + personMemory.transform.name + " " +targetPosition);
            return new List<Node>();
        }
        frontierRoom.Clear();
        frontierRoom.Enqueue(new Node(personGraph.MeAsNode), 0);
        Dictionary<string, Node> cameFrom = new Dictionary<string, Node>();
        Dictionary<string, float> costSoFar = new Dictionary<string, float>();
        cameFrom.Add(personGraph.MeAsNode.Name, null);
        costSoFar.Add(personGraph.MeAsNode.Name, 0);

        while (frontierRoom.Count > 0)
        {
            Node current = frontierRoom.Dequeue();
            if (current.Name == target.MeAsNode.Name)
            {
                break;
            }
            List<Node> neighbours = null;
            if(current.Name == person.name)
            {
                neighbours = personNeighbours.ToList();
            }
            else
            {
                neighbours = GameObject.Find(current.Name).GetComponent<RoomLocation>().NeighbourNodes;
            }
            foreach (Node next in neighbours)
            {
                float newCost = costSoFar[current.Name] + Utils.Distance(current.Position, next.Position);
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


    FastPriorityQueue<Node> frontierPerson = new FastPriorityQueue<Node>(MAX_NODES_IN_QUEUE);
    public List<Node> FindWay(Graph graph, GameObject person, Node targetPosition, PersonMemory personMemory)
    {
        //first check if blocked nodes are still blocked!!!
        personMemory.UpdateBlockedNodes();
        PersonGraph personGraph = new PersonGraph(person);
        Node[] personNeighbours = personGraph.GetNeighbours();
        if (!CheckInputs(graph, personGraph.MeAsNode, targetPosition)) return null;
        frontierPerson.Clear();
        frontierPerson.Enqueue(new Node(personGraph.MeAsNode), 0);
        Dictionary<string, Node> cameFrom = new Dictionary<string, Node>();
        Dictionary<string, float> costSoFar = new Dictionary<string, float>();
        cameFrom.Add(personGraph.MeAsNode.Name, null);
        costSoFar.Add(personGraph.MeAsNode.Name, 0);

        while (frontierPerson.Count > 0)
        {
            Node current = frontierPerson.Dequeue();
            if (current.Name == targetPosition.Name)
            {
                break;
            }
            Node[] neighbours = null;
            if (current.Name == person.name)
            {
                neighbours = personGraph.GetNeighbours();
            }
            else {
                neighbours = graph.GetNeighbours(current);
            }
            foreach (Node next in neighbours)
            {
                //if a node is blocked by a door (a person doesn't have a keys) then d not consider the node and find another way
                //if blocked by shooter, too
                if (personMemory.GetBlockedNodes().Select(node => node.Name).Contains(next.Name)
                    || (personMemory.BlockedByShooter != null && personMemory.BlockedByShooter.Select(node => node.Name).Contains(next.Name)))
                {
                    continue;
                }
                float newCost = costSoFar[current.Name] + Utils.Distance(current.Position, next.Position);
                if (!costSoFar.ContainsKey(next.Name) || newCost < costSoFar[next.Name])
                {
                    costSoFar[next.Name] = newCost;
                    float priority = newCost + Heuristic(targetPosition, next);
                    frontierPerson.Enqueue(new Node(next), priority);
                    if (cameFrom.ContainsKey(next.Name)) cameFrom[next.Name] = current;
                    else cameFrom.Add(next.Name, current);
                }
            }
        }

        List<Node> desiredPath = new List<Node>();
        desiredPath.Add(targetPosition);
        if (!cameFrom.ContainsKey(targetPosition.Name))
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
}
