using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.Fuzzy.Library;
using System;
using System.Linq;

public class Walking
{
    public List<Node> Path;
    public int currentNodeIndex;
    public float Speed;
    public float RotationSpeed;
    public float CurrentSpeed;

    AvoidanceSystem avoidanceSystem;

    Pathfinder pathfinder;
    CollisionDetection collisionDetection;
    public PersonGoal goal;


    private Rigidbody m_Rigidbody;
    private float blockedWayTimeout = 0f; //3f 3 sec

    private float _finalAngle = 0.0f;

    public bool Executing;
    public Action ActionToExecute;

    public Walking(Rigidbody rigidbody)
    {
        RotationSpeed = 7.5f;
        Speed = 0f;
        CurrentSpeed = 0f;
        m_Rigidbody = rigidbody;
        collisionDetection = new CollisionDetection();
        CurrentSpeed = Resources.Run;
        pathfinder = new Pathfinder();
        avoidanceSystem = new AvoidanceSystem();
        avoidanceSystem.initAvoidanceSystem();
        ActionToExecute = null;
    }

    public void InitPath(PersonMemory memory)
    {
        Path = pathfinder.FindWay(memory.Graph[memory.CurrentFloor], memory.StartPosition, memory.TargetPosition, memory);
        currentNodeIndex = 0;
    }

    public void InitPath(GameObject gameObject)
    {
        Path = new List<Node>() { new Node() { Name = gameObject.name, Position = gameObject.transform.position } };
        currentNodeIndex = 0;
    }

    public void InitPath(Room room)
    {
        Path = new List<Node>() { new Node() { Name = room.Id, Position = room.Reference.transform.position } };
        currentNodeIndex = 0;
    }

    public void ExecuteAction(Action action, PersonMemory memory, Transform transform)
    {
        if (IsActionExecuting(action)) return; //don't do that again!

        switch (action.Command)
        {
            case Command.GO_UP:
                memory.FindNearestLocation(transform.position);
                memory.setTargetPosition(NearestStairs("UP", transform, memory));
                InitPath(memory);
                Executing = true;
                break;
            case Command.GO_DOWN:
                memory.FindNearestLocation(transform.position);
                memory.setTargetPosition(NearestStairs("DOWN", transform, memory));
                InitPath(memory);
                Executing = true;
                break;
            case Command.EXIT_BUILDING:
                memory.FindNearestLocation(transform.position);
                memory.setTargetPosition(NearestExit(transform, memory));
                InitPath(memory);
                Executing = true;
                break;
            case Command.GO_TO_ROOM:
                memory.FindNearestLocation(transform.position);
                memory.setTargetPosition(action.Limits.
                    Select(limit => limit.FoundRoom.Id).
                    Where(id => !String.IsNullOrEmpty(id)).ToArray()[0]);
                Executing = true;
                InitPath(memory);
                break;
            case Command.GO_TO_DOOR:
                GameObject doorToOpen = action.Limits.
                    Select(limit => limit.DoorToOpen).
                    Where(door => door != null).ToArray()[0];
                InitPath(doorToOpen);
                CurrentSpeed = Resources.Walk;
                Executing = true;
                break;
            case Command.ENTER_ROOM:
                Room room = action.Limits.
                    Select(limit => limit.FoundRoom).
                    Where(r => r != null).ToArray()[0];
                InitPath(room);
                CurrentSpeed = Resources.Walk;
                Executing = true;
                break;
            case Command.STAY:
                Path = new List<Node>();
                memory.setStartPosition("");
                memory.setTargetPosition("");
                Executing = false;
                CurrentSpeed = Resources.Stay;
                break;
        }
        ActionToExecute = action;
    }

    public bool IsActionExecuting(Action action)
    {
        return (ActionToExecute != null && action.Command == ActionToExecute.Command && Executing);
    }

    public void MakeMove(Transform transform, PersonMemory memory)
    {
        Executing = !CheckGoal(transform, memory);
        if (transform.name == "Informer")
        {
           // Debug.Log(Speed);
           // Debug.Log(Executing);
        }
        if (Executing)
        {
            Vector3 m_EulerAngleVelocity;
            Quaternion deltaRotation;
            m_Rigidbody.MovePosition(transform.position + transform.forward * Speed * Time.deltaTime);
            m_EulerAngleVelocity = new Vector3(0, _finalAngle, 0);
            deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * RotationSpeed * Time.deltaTime);
            m_Rigidbody.MoveRotation(m_Rigidbody.rotation * deltaRotation);
            m_Rigidbody.isKinematic = true;
            m_Rigidbody.isKinematic = false;
        }
        else
        {
            switch (ActionToExecute.Command)
            {
                case Command.GO_UP:
                    if (pathfinder.CheckDistance(transform.gameObject, memory.TargetPosition))
                    {
                        TryToGoStairs(transform.gameObject, memory);
                    }
                    break;
                case Command.GO_DOWN:
                    if (pathfinder.CheckDistance(transform.gameObject, memory.TargetPosition))
                    {
                        TryToGoStairs(transform.gameObject, memory);
                    }
                    break;
                case Command.EXIT_BUILDING:
                    if (NearestExit(transform, memory) != null)
                    {
                        transform.gameObject.SetActive(false);
                    }
                    break;
                default:
                    break;
            }
            ActionToExecute.IsDone = true;
        }
    }

    public void CalculateMovement(Transform transform)
    {
        if (Path == null || currentNodeIndex >= Path.Count) return;
        collisionDetection.UpdateCollisions(transform, Path[currentNodeIndex].Position);

        avoidanceSystem.Calculate(collisionDetection.rightDist, collisionDetection.veryRightDist, collisionDetection.leftDist,
            collisionDetection.veryLeftDist, collisionDetection.frontDist, Speed, CurrentSpeed, collisionDetection.isStatic);


        float aCS = avoidanceSystem.GetOutputValue("SpeedChange");


        if (aCS != -999.0f)
        {
            Speed += aCS;
        }
        if (Speed < 0.0f) Speed = 0.0f;
        if (Speed > 2.0f) Speed = 2.0f;
        float goalAngle = pathfinder.GetGoalAngle(transform.gameObject, Path[currentNodeIndex].Position);
        float avoidanceAngle = avoidanceSystem.GetOutputValue("Angle");
        float ratio = 0.4f;
        float goalWeight = 0.5f;
        float avoidanceWeight = 0.7f;
        if (avoidanceAngle != -999.0f)
        {
            _finalAngle = avoidanceWeight * avoidanceAngle + goalWeight * goalAngle;
        }
    }

    private bool CheckGoal(Transform transform, PersonMemory memory)
    {
        if (Path == null || Path.Count == 0) return true;
        if (currentNodeIndex >= Path.Count) return true;
        int layerMask = 1 << 9;
        RaycastHit hit;
        if (Physics.Linecast(transform.position, Path[currentNodeIndex].Position, out hit, layerMask) && blockedWayTimeout >= 2f)
        {
            blockedWayTimeout = 0f;
            memory.FindNearestLocation(transform.position);
            if (memory.StartPosition != null)
            {
                Path = pathfinder.FindWay(memory.Graph[memory.CurrentFloor], memory.StartPosition, memory.TargetPosition, memory);
                currentNodeIndex = 0;
                return false;
            }
            else return true;
        }
        else
        {
            blockedWayTimeout += Time.deltaTime + 0.1f;
        }
        if (pathfinder.CheckDistance(transform.gameObject, Path[currentNodeIndex]))
        {
            if (++currentNodeIndex >= Path.Count) return true;
            else return false;
        }

        return false;
    }

    public bool IsPositive(float number)
    {
        return number > 0;
    }

    public bool IsNegative(float number)
    {
        return number < 0;
    }

    public void UpdatePathAfterBlockedNode(Transform transform, PersonMemory memory)
    {
        blockedWayTimeout = 0f;
        memory.FindNearestLocation(transform.position);
        if (memory.StartPosition != null)
        {
            Path = pathfinder.FindWay(memory.Graph[memory.CurrentFloor], memory.StartPosition, memory.TargetPosition, memory);
            currentNodeIndex = 0;
            if(Path.Count == 0)
            {
                ActionToExecute.IsDone = true;
                Executing = false;
            }
        }
    }

    public void TryToGoStairs(GameObject gameObject, PersonMemory memory)
    {
        GameObject.Find(memory.TargetPosition.Name).SendMessage("TeleportMePls", gameObject);
    }


    public string NearestStairs(string stairsType, Transform transform, PersonMemory memory)
    {
        GameObject location = GameObject.Find("Checkpoints " + memory.CurrentFloor);
        List<GameObject> stairs = new List<GameObject>();
        foreach (Transform item in location.transform)
        {
            if (item.GetComponent<Stairs>() != null && item.GetComponent<PathLocation>().StairsDirection == stairsType)
            {
                stairs.Add(item.gameObject);
            }
            if (stairs.Count >= 2) break;
        }
        GameObject nearestStairs = stairs[0];
        foreach (var item in stairs)
        {
            if (Vector3.Distance(transform.position, nearestStairs.transform.position)
                >
                Vector3.Distance(transform.position, item.transform.position))
            {
                nearestStairs = item;
            }
        }
        return nearestStairs.name;
    }

    public string NearestExit(Transform transform, PersonMemory memory)
    {
        GameObject location = GameObject.Find("Checkpoints " + memory.CurrentFloor);
        List<GameObject> exits = new List<GameObject>();
        foreach (Transform item in location.transform)
        {
            if (item.GetComponent<PathLocation>().IsExit)
            {
                exits.Add(item.gameObject);
            }
            if (exits.Count >= 2) break;
        }
        if (exits.Count == 0) return null;
        GameObject nearestExits = exits[0];
        foreach (var item in exits)
        {
            if (Vector3.Distance(transform.position, nearestExits.transform.position)
                >
                Vector3.Distance(transform.position, item.transform.position))
            {
                nearestExits = item;
            }
        }
        return nearestExits.name;
    }
}
