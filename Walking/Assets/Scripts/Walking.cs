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
    public GameObject Me;


    private Rigidbody m_Rigidbody;
    private float blockedWayTimeout = 0f; //3f 3 sec

    private float _finalAngle = 0.0f;

    public bool Executing;
    public Task TaskToExecute;

    public GameObject Shooter;

    public Walking(Rigidbody rigidbody)
    {
        RotationSpeed = 10f;
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

        Shooter = GameObject.FindGameObjectWithTag("ActiveShooter");
    }

    public void InitPath(PersonMemory memory)
    {
        //Path = pathfinder.FindWay(memory.Graph[memory.CurrentFloor], memory.StartPosition, memory.TargetPosition, memory);
        Path = pathfinder.FindWay(memory.Graph[memory.CurrentFloor], Me, memory.TargetPosition, memory);
        currentNodeIndex = 0;
    }

    public void InitPathWithRoomPath(PersonMemory memory)
    {
        List<Node> longPath = pathfinder.FindWay(memory.Graph[memory.CurrentFloor], Me, memory.TargetPosition, memory);
        longPath = longPath.Where(n => n.Name != Me.name).ToList();
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
        //GameObject startPosition = Utils.NearestRoomLocation(room.Reference, transform.gameObject);
        Path = pathfinder.FindWay(Me, targetPosition, memory);
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

    public void InitPathToEnemy(PersonMemory memory)
    {
        LayerMask layerMask = LayerMask.GetMask("Wall", "Door", "Obstacle");
        bool canRunTowards = !Physics.Linecast(Me.transform.position, Shooter.transform.position, layerMask);
        if (canRunTowards)
        {
            InitPath(Shooter);
        }
        else
        {
            memory.setTargetPosition(memory.FindNearestLocation(Shooter.transform.position).Name);
            if (memory.CurrentRoom == null)
            {
                InitPath(memory);
            }
            else
            {
                InitRoomPath(memory.CurrentRoom, Me.transform, memory, memory.CurrentRoom.Door);
                InitPathWithRoomPath(memory);
            }
        }
    }

    public void ExecuteTask(Task task, PersonMemory memory, Transform transform)
    {
        if (IsTaskExecuting(task)) return; //don't do that again!s 
        TaskToExecute = task;
        if (Me.CompareTag("ActiveShooter"))
        {
            //Debug.Log(task.Command);
        }
        switch (task.Command)
        {
            case Command.GO_UP:
                Room upRoom = Utils.GetRoom(task);
                if (upRoom != null && !Utils.RoomIsAbove(upRoom, memory))
                {
                    FinishWalking();
                    return;
                }
                string nearestStairs = Utils.NearestStairs("UP", transform, memory);
                if (nearestStairs == null)
                {
                    Me.GetComponent<Person>().MyState.CanGoUp = false;
                    FinishWalking();
                    Me.SendMessage("SelectBehaviour");
                }
                memory.setTargetPosition(nearestStairs);
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
                string nearestStairs2 = Utils.NearestStairs("DOWN", transform, memory);
                if (nearestStairs2 == null)
                {
                    Me.GetComponent<Person>().MyState.CanGoDown = false;
                    FinishWalking();
                    Me.SendMessage("SelectBehaviour");
                }
                memory.setTargetPosition(nearestStairs2);
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
                memory.setTargetPosition(room1.Id);
                InitPath(memory);
                if (!transform.CompareTag("ActiveShooter") && Path.Count == 0 && room1.Id == memory.MyRoom.Id)
                {
                    Me.GetComponent<Person>().MyState.CanRunToMyRoom = false;
                    FinishWalking();
                    Me.SendMessage("SelectBehaviour");
                    return;
                }
                CurrentSpeed = Resources.Walk; //test
                Executing = true;
                break;
            case Command.GO_TO_DOOR:
                GameObject doorToOpen = Utils.GetDoor(task);
                if (doorToOpen == null || !Utils.ToFar(transform.gameObject, doorToOpen, Pathfinder.MIN_DISTANCE_DOOR))
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
                    || Utils.DoorIsLocked(room2.Door)
                    || !Utils.ToFar(Me, room2.Reference, Pathfinder.MIN_DISTANCE_ROOM)
                    || Utils.ToFar(Me, room2.Door, Pathfinder.MIN_DISTANCE_DOOR))
                {
                    //if (Me.CompareTag("ActiveShooter"))
                    //{
                    //    Debug.Log(String.Format("{0} {1} {2} {3} {4}",
                    //        room2 == null,
                    //        room2.Id == memory.CurrentRoom?.Id,
                    //        Utils.DoorIsLocked(room2.Door),
                    //        !Utils.ToFar(Me, room2.Reference, Pathfinder.MIN_DISTANCE_ROOM),
                    //        Utils.ToFar(Me, room2.Door, Pathfinder.MIN_DISTANCE_DOOR)));
                    //}
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
                    || !Utils.IsInAnyRoom(memory)
                    || !Utils.ToFar(transform.gameObject, Utils.NearestHidingPlaceInRoom(transform.gameObject, room3.Reference), Pathfinder.MIN_DISTANCE_HIDE))
                {
                    FinishWalking();
                    return;
                }
                InitRoomPath(room3, transform, memory,
                    Utils.NearestHidingPlaceInRoom(transform.gameObject, room3.Reference));
                CurrentSpeed = Resources.Walk;
                Executing = true;
                break;
            case Command.CHECK_ROOM:
                Room room7 = Utils.GetRoom(task);
                if (room7 == null)
                {
                    FinishWalking();
                    return;
                }
                GameObject hide = Utils.NearestHidingPlaceInRoom(transform.gameObject, room7.Reference);
                if (hide == null)
                {
                    memory.ClearFoundRoom();
                    FinishWalking();
                    return;
                }
                if (!Utils.IsInAnyRoom(memory)
                    || !Utils.ToFar(transform.gameObject, hide, Pathfinder.MIN_DISTANCE))
                {
                    FinishWalking();
                    return;
                }
                InitRoomPath(room7, transform, memory, hide);
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
            case Command.RUN_TO_ENEMY:
                if (!Utils.ToFar(Shooter, Me))
                {
                    FinishWalking();
                    return;
                }
                InitPathToEnemy(memory);
                CurrentSpeed = Resources.Sprint;
                Executing = true;
                break;
            case Command.RUN_AWAY:
                if (!Utils.CanSee(Me, Shooter))
                {
                    FinishWalking();
                    return;
                }
                memory.UpdateActiveShooterInfo(Shooter);
                memory.ClearBlockedNodesByShooter(); //if we see shooter then previous locations are clear probably
                memory.setTargetPosition(Utils.GetNodeToRunAway(Me, Shooter).Name);
                InitPath(memory);
                CurrentSpeed = Resources.Sprint;
                Executing = true;
                break;
            case Command.GO_TO_WORKPLACE:
                Room room5 = Utils.GetRoom(task);
                if (room5 == null
                    || room5.Id != memory.MyRoom.Id
                    || memory.CurrentRoom == null
                    || memory.CurrentRoom.Id != memory.MyRoom.Id
                    || !Utils.ToFar(transform.gameObject, Utils.GetMyWorkplace(memory), Pathfinder.MIN_DISTANCE_WORKPLACE))
                {
                    FinishWalking();
                    return;
                }
                InitRoomPath(room5, transform, memory, Utils.GetMyWorkplace(memory));
                CurrentSpeed = Resources.Walk;
                Executing = true;
                break;
            case Command.WORK:
                if (memory.CurrentRoom == null
                    || memory.CurrentRoom.Id != memory.MyRoom.Id
                    || Utils.ToFar(transform.gameObject, Utils.GetMyWorkplace(memory), Pathfinder.MIN_DISTANCE_WORKPLACE))
                {
                    FinishWalking();
                    return;
                }
                Path = new List<Node>();
                memory.setStartPosition("");
                memory.setTargetPosition("");
                CurrentSpeed = Resources.Stay;
                FinishWalking();
                break;
        }
        if (task.Command != Command.HIDE_IN_CURRENT_ROOM
            && Path != null && Path.Count > 0
            && Me.GetComponent<Person>().MyState.IsHiding)
        {
            Me.GetComponent<PersonHide>().GetUp();
        }
    }

    public bool IsTaskExecuting(Task task)
    {
        return (TaskToExecute != null && task.Command == TaskToExecute.Command && Executing);
    }

    public void MakeMove(Transform transform, PersonMemory memory)
    {
        if (TaskToExecute.Command == Command.RUN_AWAY)
        {
            if (!Utils.CanSee(Me, Shooter))
            {
                FinishWalking();
                return;
            }
            Node foundNode = Utils.GetNodeToRunAway(Me, Shooter, true);
            if (foundNode != null)
            {
                memory.setTargetPosition(foundNode.Name);
                InitPath(Me.GetComponent<Person>().PersonMemory);
            }
        }
        Executing = !CheckGoal(transform, memory);

        if (Executing)
        {
            Vector3 m_EulerAngleVelocity;
            Quaternion deltaRotation;
            if (memory.PickedObstacle == null) //test check if good
            {
                m_Rigidbody.MovePosition(transform.position + transform.forward * GetSpeedAtStaircase(Speed) * Time.deltaTime);
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
                case Command.RUN_AWAY:
                    memory.UpdateNodesBlockedByShooter();
                    break;
                case Command.CHECK_ROOM:
                    memory.ClearFoundRoom();
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
        Room currentRoom = Me.GetComponent<Person>().PersonMemory.CurrentRoom;
        if (currentRoom == null)
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
            float avoidanceWeight = 2f;
            if (avoidanceAngle != -999.0f)
            {
                _finalAngle = avoidanceWeight * avoidanceAngle + goalWeight * goalAngle;
            }
        }
        else
        {
            float goalAngle = pathfinder.GetGoalAngle(transform.gameObject, Path[currentNodeIndex].Position);
            float goalWeight = 1f;
            _finalAngle = goalWeight * goalAngle;
            if (Speed < CurrentSpeed)
            {
                Speed += 0.03f;
            }
            if (Speed > CurrentSpeed)
            {
                Speed -= 0.05f;
            }
            //Speed = CurrentSpeed;
        }

    }

    private bool CheckGoal(Transform transform, PersonMemory memory)
    {
        if (Path == null || Path.Count == 0) return true;
        if (currentNodeIndex >= Path.Count) return true;
        LayerMask layerMask = LayerMask.GetMask("Wall");
        RaycastHit hit;
        if (Physics.Linecast(transform.position, Path[currentNodeIndex].Position, out hit, layerMask) && blockedWayTimeout >= 2f)
        {
            blockedWayTimeout = 0f;
            InitPath(memory);
            return false;
        }
        else
        {
            blockedWayTimeout += Time.deltaTime + 0.1f;
        }
        if (currentNodeIndex == Path.Count - 1)
        {
            if (pathfinder.CheckDistance(transform.gameObject, Path[currentNodeIndex], TaskToExecute.Command))
            {
                if (++currentNodeIndex >= Path.Count) return true;
                else return false;
            }
        }
        else
        {
            if (pathfinder.CheckDistance(transform.gameObject, Path[currentNodeIndex]))
            {
                if (++currentNodeIndex >= Path.Count) return true;
                else return false;
            }
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

        InitPath(memory);
        if (Path.Count == 0)
        {
            FinishWalking();
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

    public void CheckIfSeeShooter()
    {
        if (TaskToExecute == null) return;
        if (TaskToExecute.Command == Command.RUN_TO_ENEMY
            && Utils.CanSee(Me, Shooter))
        {
            InitPath(Shooter);
        }
    }


    public void FinishWalking()
    {
        Person person = Me.GetComponent<Person>();
        if (TaskToExecute.Command == Command.HIDE_IN_CURRENT_ROOM && Utils.IsHiding(Me, person.PersonMemory))
        {
            person.GetComponent<PersonHide>().Hide();
        }
        if (TaskToExecute.Command == Command.BLOCK_DOOR)
        {
            person.PersonMemory.PutObstacle();
        }
        if (TaskToExecute.Command == Command.RUN_AWAY)
        {
            person.PersonMemory.UpdateNodesBlockedByShooter();
        }
        TaskToExecute.IsDone = true;
        Executing = false;
        Path = null;
    }

    public void Slowdown(float factor)
    {
        Speed = Resources.SlowWalk;
    }

    public float GetSpeedAtStaircase(float currentSpeed)
    {
        if (Me.GetComponent<Person>().PersonMemory.GetCurrentStaircase() == null)
        {
            return currentSpeed;
        }

        float w = Resources.Walk;
        float s = Resources.Sprint;
        float a = (3 * w - 2 * s) / (6 * (w - s));
        float b = s / 3 - (3 * s * w - 2 * s * s) / (6 * (w - s));

        float res = a * currentSpeed + b;
        return res;
    }
}
