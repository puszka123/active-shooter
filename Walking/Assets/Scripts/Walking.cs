﻿using System.Collections;
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
    public GameObject Me;


    private Rigidbody m_Rigidbody;
    private float blockedWayTimeout = 0f; //3f 3 sec

    private float _finalAngle = 0.0f;

    public bool Executing;
    public Task TaskToExecute;

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
        TaskToExecute = null;
        Me = rigidbody.gameObject;
    }

    public void InitPath(PersonMemory memory)
    {
        Path = pathfinder.FindWay(memory.Graph[memory.CurrentFloor], memory.StartPosition, memory.TargetPosition, memory);
        currentNodeIndex = 0;
    }

    public void InitPathWithRoomPath(PersonMemory memory)
    {
        List<Node> longPath = pathfinder.FindWay(memory.Graph[memory.CurrentFloor], memory.StartPosition, memory.TargetPosition, memory);
        Path.AddRange(longPath);
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

    public void InitRoomPath(Room room, Transform transform, PersonMemory memory, GameObject targetPosition)
    {
        GameObject startPosition = Utils.NearestRoomLocation(room.Reference, transform.gameObject);
        Path = pathfinder.FindWay(startPosition, targetPosition, memory);
        currentNodeIndex = 0;
    }

    public void ExecuteTask(Task task, PersonMemory memory, Transform transform)
    {
        if (IsTaskExecuting(task)) return; //don't do that again!
        TaskToExecute = task;
        switch (task.Command)
        {
            case Command.GO_UP:
                memory.FindNearestLocation(transform.position);
                memory.setTargetPosition(Utils.NearestStairs("UP", transform, memory));
                if (Utils.IsInAnyRoom(memory))
                {
                    GameObject roomUp = Utils.GetRoom(memory).Door;
                    InitRoomPath(memory.CurrentRoom, transform, memory, roomUp);
                    InitPathWithRoomPath(memory);
                }
                else
                {
                    InitPath(memory);
                }
                Executing = true;
                break;
            case Command.GO_DOWN:
                memory.FindNearestLocation(transform.position);
                memory.setTargetPosition(Utils.NearestStairs("DOWN", transform, memory));
                if (Utils.IsInAnyRoom(memory))
                {
                    GameObject roomDown = Utils.GetRoom(memory).Door;
                    InitRoomPath(memory.CurrentRoom, transform, memory, roomDown);
                    InitPathWithRoomPath(memory);
                }
                else
                {
                    InitPath(memory);
                }
                
                Executing = true;
                break;
            case Command.EXIT_BUILDING:
                memory.FindNearestLocation(transform.position);
                memory.setTargetPosition(Utils.NearestExit(transform, memory));
                InitPath(memory);
                Executing = true;
                break;
            case Command.GO_TO_ROOM:
                memory.FindNearestLocation(transform.position);
                Room room1 = Utils.GetRoom(task);
                if(room1 == null || room1.Id == memory.CurrentRoom?.Id)
                {
                    FinishWalking();
                    return;
                }
                memory.setTargetPosition(room1.Id);
                InitPath(memory);
                Executing = true;
                break;
            case Command.GO_TO_DOOR:
                GameObject doorToOpen = Utils.GetDoor(task);
                if(doorToOpen == null)
                {
                    FinishWalking();
                    return;
                }
                if (Utils.IsInAnyRoom(memory))
                {
                    InitRoomPath(memory.CurrentRoom, transform, memory, doorToOpen);
                }
                else
                {
                    InitPath(doorToOpen);
                }
                CurrentSpeed = Resources.Walk;
                Executing = true;
                break;
            case Command.ENTER_ROOM:
                Room room2 = Utils.GetRoom(task);
                if (room2 == null || room2.Id == memory.CurrentRoom?.Id)
                {
                    FinishWalking();
                    return;
                }
                InitPath(room2);
                CurrentSpeed = Resources.Walk;
                Executing = true;
                break;
            case Command.HIDE_IN_CURRENT_ROOM:
                Room room3 = Utils.GetRoom(task);
                if (room3 == null 
                    || transform.GetComponent<Person>().MyState.IsHiding 
                    || !Utils.IsInAnyRoom(memory))
                {
                    FinishWalking();
                    return;
                }
                InitRoomPath(room3, transform, memory, 
                    Utils.TheFarthestHidingPlaceInRoom(transform.gameObject, room3.Reference));
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
    }

    public bool IsTaskExecuting(Task task)
    {
        return (TaskToExecute != null && task.Command == TaskToExecute.Command && Executing);
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
            switch (TaskToExecute.Command)
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
                    if (Utils.NearestExit(transform, memory) != null)
                    {
                        transform.gameObject.SetActive(false);
                    }
                    break;
                default:
                    break;
            }
            FinishWalking();
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
        float avoidanceWeight = 2f;
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
                InitPath(memory);
                return false;
            }
            else
            {
                return true;
            }
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
            InitPath(memory);
            if(Path.Count == 0)
            {
                FinishWalking();
            }
        }
    }

    public void TryToGoStairs(GameObject gameObject, PersonMemory memory)
    {
        GameObject.Find(memory.TargetPosition.Name).SendMessage("TeleportMePls", gameObject);
    }


    

    public void FinishWalking()
    {
        Person person = Me.GetComponent<Person>();
        if (TaskToExecute.Command == Command.HIDE_IN_CURRENT_ROOM && Utils.IsHiding(Me, person.PersonMemory))
        {
            person.MyState.IsHiding = true;
        }
        TaskToExecute.IsDone = true;
        Executing = false;
        Path = null;
    }
}
