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
    public Command? CommandToExecute;

    public Walking(Rigidbody rigidbody)
    {
        RotationSpeed = 10f;
        Speed = 0f;
        CurrentSpeed = 0f;
        m_Rigidbody = rigidbody;
        collisionDetection = new CollisionDetection();
        CurrentSpeed = Resources.Walk;
        pathfinder = new Pathfinder();
        avoidanceSystem = new AvoidanceSystem();
        avoidanceSystem.initAvoidanceSystem();
        CommandToExecute = null;
    }

    public void InitPath(PersonMemory memory)
    {
        Path = pathfinder.FindWay(memory.Graph[memory.CurrentFloor], memory.StartPosition, memory.TargetPosition, memory);
        currentNodeIndex = 0;
    }

    public void ExecuteCommand(Command cmd, PersonMemory memory, Transform transform)
    {
        CommandToExecute = cmd;
        switch (cmd)
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
        }
    }

    public void MakeMove(Transform transform, PersonMemory memory)
    {
        Executing = !CheckGoal(transform, memory);
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
            switch (CommandToExecute)
            {
                case Command.GO_UP:
                    TryToGoStairs(transform.gameObject, memory);
                    break;
                case Command.GO_DOWN:
                    TryToGoStairs(transform.gameObject, memory);
                    break;
                case Command.EXIT_BUILDING:
                    transform.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }

    public void CalculateMovement(Transform transform)
    {
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
        float avoidanceWeight = 0.5f;
        if (avoidanceAngle != -999.0f)
        {
            _finalAngle = avoidanceWeight * avoidanceAngle + goalWeight * goalAngle;
        }
    }

    private bool CheckGoal(Transform transform, PersonMemory memory)
    {
        if (Path == null) return true;
        if (currentNodeIndex >= Path.Count) return true;
        int layerMask = 1 << 9;
        RaycastHit hit;
        if (Physics.Linecast(transform.position, Path[currentNodeIndex].Position, out hit, layerMask) && blockedWayTimeout >= 3f)
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
            blockedWayTimeout += Time.deltaTime;
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
