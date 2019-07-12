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
        if (Me.name == "employee 43")
        {
            Debug.Log(TaskToExecute.Command);
        }
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

    public void InitPathToNearestObstacle(GameObject gameObject, PersonMemory memory)
    {
        GameObject obstacleToPick = memory.GetNearestObstacle(gameObject);
        if (obstacleToPick == null)
        {
            FinishWalking();
            return;
        }
        //memory.PickObstacle(obstacleToPick);
        Path = new List<Node>() { new Node() { Name = obstacleToPick.name, Position = obstacleToPick.transform.position } };
        currentNodeIndex = 0;
    }

    public void ExecuteTask(Task task, PersonMemory memory, Transform transform)
    {
        if (IsTaskExecuting(task)) return; //don't do that again!s 
        TaskToExecute = task;
        switch (task.Command)
        {
            case Command.GO_UP:
                Room upRoom = Utils.GetRoom(task);
                if (upRoom != null && !Utils.RoomIsAbove(upRoom, memory))
                {
                    FinishWalking();
                    return;
                }
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
                CurrentSpeed = Resources.Walk; //test
                Executing = true;
                break;
            case Command.GO_DOWN:
                Room downRoom = Utils.GetRoom(task);
                if (downRoom != null && !Utils.RoomIsBelow(downRoom, memory))
                {
                    FinishWalking();
                    return;
                }
                if (memory.CurrentFloor == 0)
                {
                    FinishWalking();
                    return;
                }
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
                CurrentSpeed = Resources.Walk; //test
                Executing = true;
                break;
            case Command.EXIT_BUILDING:
                if (memory.CurrentFloor != 0)
                {
                    FinishWalking();
                    return;
                }
                memory.FindNearestLocation(transform.position);
                memory.setTargetPosition(Utils.NearestExit(transform, memory));
                InitPath(memory);
                CurrentSpeed = Resources.Walk; //test
                Executing = true;
                break;
            case Command.GO_TO_ROOM:
                Room room1 = Utils.GetRoom(task);
                if (room1 == null
                    || room1.Id == memory.CurrentRoom?.Id
                    || !Utils.RoomIsAtMyLevel(room1, memory))
                {
                    FinishWalking();
                    return;
                }
                memory.FindNearestLocation(transform.position);
                memory.setTargetPosition(room1.Id);
                InitPath(memory);
                CurrentSpeed = Resources.Walk; //test
                Executing = true;
                break;
            case Command.GO_TO_DOOR:
                GameObject doorToOpen = Utils.GetDoor(task);
                if (doorToOpen == null || !Utils.ToFar(transform.gameObject, doorToOpen))
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
                if (room2 == null 
                    || room2.Id == memory.CurrentRoom?.Id
                    || Utils.DoorIsLocked(room2.Door))
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
            case Command.PICK_NEAREST_OBSTACLE:
                if (!Utils.IsInAnyRoom(memory))
                {
                    FinishWalking();
                    return;
                }
                InitPathToNearestObstacle(transform.gameObject, memory);
                CurrentSpeed = Resources.Walk;
                Executing = true;
                break;
            case Command.BLOCK_DOOR:
                if (!Utils.IsInAnyRoom(memory) || memory.PickedObstacle == null)
                {
                    FinishWalking();
                    return;
                }
                memory.PickedObstacle.GetComponent<Obstacle>().TryToPickUp(transform.gameObject);
                Executing = true;
                break;
            case Command.STAY:
                Path = new List<Node>();
                memory.setStartPosition("");
                memory.setTargetPosition("");
                CurrentSpeed = Resources.Stay;
                FinishWalking();
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
            if (memory.PickedObstacle == null) //test check if good
            {
                m_Rigidbody.MovePosition(transform.position + transform.forward * Speed * Time.deltaTime);
            }
            else
            {
                m_Rigidbody.MovePosition(transform.position + transform.forward * 0.025f * Time.deltaTime);
            }
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
                    if (Utils.NearestExit(transform, memory) != null
                        && pathfinder.CheckDistance(transform.gameObject, memory.TargetPosition))
                    {
                        transform.gameObject.SetActive(false);
                    }
                    break;
                case Command.BLOCK_DOOR:
                    GameObject obstacle = memory.PickedObstacle;
                    if (obstacle != null
                        && Utils.Distance(memory.CurrentRoom.Door, transform.gameObject) < Pathfinder.MIN_DISTANCE)
                    {
                        memory.CurrentRoom.Door.SendMessage("AddObstacle", obstacle);
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
        if (pathfinder.CheckDistance(transform.gameObject, Path[currentNodeIndex], TaskToExecute.Command))
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
            if (Path.Count == 0)
            {
                FinishWalking();
            }
        }
    }

    public void TryToGoStairs(GameObject gameObject, PersonMemory memory)
    {
        GameObject.Find(memory.TargetPosition.Name).SendMessage("TeleportMePls", gameObject);
    }

    public void OnObstaclePick(GameObject obstacle, GameObject person)
    {
        if (TaskToExecute.Command == Command.BLOCK_DOOR)
        {
            if (person.GetComponent<Person>().PersonMemory.PickedObstacle == null)
            {
                FinishWalking();
            }
            person.GetComponent<Person>().PersonMemory.PickedObstacle.SetActive(false);
            person.GetComponent<Person>().PersonMemory.CurrentRoom.Reference.SendMessage("PickedObstacle",
                person.GetComponent<Person>().PersonMemory.PickedObstacle);
            InitPath(person.GetComponent<Person>().PersonMemory.CurrentRoom.Door);
            CurrentSpeed = obstacle.GetComponent<Obstacle>().TargetSpeed;
        }
    }

    public void OnBeNearObstacle(GameObject obstacle, GameObject person)
    {
        if (TaskToExecute != null && TaskToExecute.Command == Command.PICK_NEAREST_OBSTACLE)
        {
            person.GetComponent<Person>().PersonMemory.PickObstacle(obstacle);
            FinishWalking();
        }
    }


    public void FinishWalking()
    {
        Person person = Me.GetComponent<Person>();
        if (TaskToExecute.Command == Command.HIDE_IN_CURRENT_ROOM && Utils.IsHiding(Me, person.PersonMemory))
        {
            person.MyState.IsHiding = true;
        }
        if (TaskToExecute.Command == Command.BLOCK_DOOR)
        {
            person.PersonMemory.PutObstacle();
        }
        TaskToExecute.IsDone = true;
        Executing = false;
        Path = null;
    }
}
