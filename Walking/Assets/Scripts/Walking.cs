using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.Fuzzy.Library;
using System;
using System.Linq;

public class Walking : MonoBehaviour
{
    public List<Node> Path;
    public int currentNodeIndex;
    public float Speed;
    public float RotationSpeed;

    public GameObject StartPosition;
    public GameObject TargetPosition;
    public float CurrentSpeed;

    AvoidanceSystem avoidanceSystem;

    Pathfinder pathfinder;
    PersonMemory memory;
    public PersonMemory PersonMemory { get { return memory; } }
    CollisionDetection collisionDetection;
    public PersonGoal goal;


    private Rigidbody m_Rigidbody;
    private float timer = 0.1f;
    private float timerEdge = 0.1f;
    private float blockedWayTimeout = 0f; //3f 3 sec

    private float _finalAngle = 0.0f;

    //private List<GameObject> pathLocations;
    //private int next = 0;

    // Use this for initialization
    void Start()
    {
        
    }

    public void Init(int floor)
    {
        goal = new PersonGoal();
        goal.SetCurrentGoal(Goal.GO_UP);
        CurrentSpeed = Resources.Walk;
        pathfinder = new Pathfinder();
        avoidanceSystem = new AvoidanceSystem();
        avoidanceSystem.initAvoidanceSystem();
        m_Rigidbody = GetComponent<Rigidbody>();
        collisionDetection = new CollisionDetection();
        memory = new PersonMemory();
        memory.Init(floor, transform.position);
        GetTargetByGoal();
        Path = pathfinder.FindWay(memory.Graph[memory.CurrentFloor], memory.StartPosition, memory.TargetPosition, memory);
        currentNodeIndex = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (CheckGoal() || memory == null)
        {
            if (memory != null)
            {
                switch (goal.MyCurrentGoal)
                {
                    case Goal.GO_UP:
                        TryToGoUp();
                        break;
                    case Goal.GO_DOWN:
                        TryToGoDown();
                        break;
                    default:
                        memory.FindNearestLocation(transform.position);
                        memory.setTargetPosition(memory.RandomTarget().Name);
                        Path = pathfinder.FindWay(memory.Graph[memory.CurrentFloor], memory.StartPosition, memory.TargetPosition, memory);
                        currentNodeIndex = 0;
                        break;
                }
            }
            return;
        }

        Vector3 m_EulerAngleVelocity;
        Quaternion deltaRotation;
        if (timer < timerEdge)
        {
            m_Rigidbody.MovePosition(transform.position + transform.forward * Speed * Time.deltaTime);
            m_EulerAngleVelocity = new Vector3(0, _finalAngle, 0);
            deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * RotationSpeed * Time.deltaTime);
            m_Rigidbody.MoveRotation(m_Rigidbody.rotation * deltaRotation);
            timer += Time.deltaTime;
            return;
        }
        else
        {
            timer = 0.0f;
        }

        collisionDetection.UpdateCollisions(transform, Path[currentNodeIndex].Position);

        //Debug.Log(String.Format("front: {0} left: {1} veryLeft: {2}, right: {3} veryRight: {4}", frontDist, littleLeftDist, veryLeftDist, littleRightDist, veryRightDist));

        avoidanceSystem.Calculate(collisionDetection.rightDist, collisionDetection.veryRightDist, collisionDetection.leftDist,
            collisionDetection.veryLeftDist, collisionDetection.frontDist, Speed, CurrentSpeed, collisionDetection.isStatic);

   
        float aCS = avoidanceSystem.GetOutputValue("SpeedChange");


        if (aCS != -999.0f)
        {
            Speed += aCS;
        }
        if (Speed < 0.0f) Speed = 0.0f;
        if (Speed > 2.0f) Speed = 2.0f;
        float goalAngle = pathfinder.GetGoalAngle(gameObject, Path[currentNodeIndex].Position);
        float avoidanceAngle = avoidanceSystem.GetOutputValue("Angle");
        float ratio = 0.4f;
        float goalWeight = 0.5f;
        float avoidanceWeight = 0.5f;
        if (avoidanceAngle != -999.0f)
        {
            //if ((avoidanceAngle > 1f || avoidanceAngle < -1f)) Debug.Log(avoidanceAngle);
            _finalAngle = avoidanceWeight * avoidanceAngle + goalWeight * goalAngle;
        }
        m_Rigidbody.MovePosition(transform.position + transform.forward * Speed * Time.deltaTime);
        m_EulerAngleVelocity = new Vector3(0, _finalAngle, 0);
        deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * RotationSpeed * Time.deltaTime);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * deltaRotation);
        m_Rigidbody.isKinematic = true;
        m_Rigidbody.isKinematic = false;
    }

    private bool CheckGoal()
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
        if (pathfinder.CheckDistance(gameObject, Path[currentNodeIndex]))
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

    public void UpdatePathAfterBlockedNode()
    {
        blockedWayTimeout = 0f;
        memory.FindNearestLocation(transform.position);
        if (memory.StartPosition != null)
        {
            Path = pathfinder.FindWay(memory.Graph[memory.CurrentFloor], memory.StartPosition, memory.TargetPosition, memory);
            currentNodeIndex = 0;
        }
    }

    public void TryToGoUp()
    {
        GameObject.Find(memory.TargetPosition.Name).SendMessage("TeleportMePls", gameObject);
    }

    public void TryToGoDown()
    {
        GameObject.Find(memory.TargetPosition.Name).SendMessage("TeleportMePls", gameObject);
    }

    public void GetTargetByGoal()
    {
        switch (goal.MyCurrentGoal)
        {
            case Goal.GO_UP:
                memory.FindNearestLocation(transform.position);
                memory.setTargetPosition(NearestUpStairs());
                Path = pathfinder.FindWay(memory.Graph[memory.CurrentFloor], memory.StartPosition, memory.TargetPosition, memory);
                currentNodeIndex = 0;
                break;
            case Goal.GO_DOWN:
                memory.FindNearestLocation(transform.position);
                memory.setTargetPosition(NearestDownStairs());
                Path = pathfinder.FindWay(memory.Graph[memory.CurrentFloor], memory.StartPosition, memory.TargetPosition, memory);
                currentNodeIndex = 0;
                break;
            default:
                break;
        }
    }

    public string NearestUpStairs()
    {
        GameObject location = GameObject.Find("Checkpoints "+memory.CurrentFloor);
        List<GameObject> stairs = new List<GameObject>();
        foreach (Transform item in location.transform)
        {
            if(item.GetComponent<Stairs>() != null && item.GetComponent<PathLocation>().StairsDirection == "UP")
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

    public string NearestDownStairs()
    {
        GameObject location = GameObject.Find("Checkpoints " + memory.CurrentFloor);
        List<GameObject> stairs = new List<GameObject>();
        foreach (Transform item in location.transform)
        {
            if (item.GetComponent<Stairs>() != null && item.GetComponent<PathLocation>().StairsDirection == "DOWN")
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
}
