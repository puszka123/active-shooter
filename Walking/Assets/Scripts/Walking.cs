using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.Fuzzy.Library;
using System;

public static class MovementTargets
{
    public const float Stay = 0.0f;
    public const float SlowWalk = 1.0f;
    public const float Walk = 2f;
    public const float Run = 5.0f;
    public const float Sprint = 7.5f;
}

public class Walking : MonoBehaviour
{
    public GameObject TestLocation;
    public float Speed;
    public float RotationSpeed;

    AvoidanceSystem avoidanceSystem;

    Pathfinder pathfinder;


    private Rigidbody m_Rigidbody;
    private float timer = 0.2f;
    private float timerEdge = 0.2f;

    private float timerRay = 0.1f;
    private float timerRayEdge = 0.1f;

    private float _finalAngle = 0.0f;

    float rightDist;
    float veryRightDist;
    float frontDist;
    float leftDist;
    float veryLeftDist;
    bool blocked = false;

    //private List<GameObject> pathLocations;
    //private int next = 0;

    // Use this for initialization
    void Start()
    {
        pathfinder = new Pathfinder();
        avoidanceSystem = new AvoidanceSystem();
        avoidanceSystem.initAvoidanceSystem();
        m_Rigidbody = GetComponent<Rigidbody>();
        //pathLocations = new List<GameObject>(GameObject.FindGameObjectsWithTag("PathLocation"));
    }

    private void Update()
    {
        timerRay += Time.deltaTime;
        if (timerRay < timerRayEdge) return;
        else timerRay = 0;
        RaycastHit hit;

        float angle;
        Vector3 direction;

        //front----------------------------
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            frontDist = hit.distance;
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
        }
        angle = Mathf.Deg2Rad * 85;
        direction = new Vector3(-Mathf.Cos(angle), 0, Mathf.Sin(angle));
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity))
        {
            frontDist = Mathf.Min(frontDist, hit.distance);
            //Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.yellow);
        }
        angle = Mathf.Deg2Rad * 85;
        direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity))
        {
            frontDist = Mathf.Min(frontDist, hit.distance);
            //Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.black);
        }

        //left----------------------------
        angle = Mathf.Deg2Rad * 60;
        direction = new Vector3(-Mathf.Cos(angle), 0, Mathf.Sin(angle));
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity))
        {
            leftDist = hit.distance;
            //Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.yellow);
        }

        angle = Mathf.Deg2Rad * 45;
        direction = new Vector3(-Mathf.Cos(angle), 0, Mathf.Sin(angle));
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity))
        {
            leftDist = Mathf.Min(leftDist, hit.distance);
            //Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.yellow);
        }
        angle = Mathf.Deg2Rad * 25;
        direction = new Vector3(-Mathf.Cos(angle), 0, Mathf.Sin(angle));
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity))
        {
            veryLeftDist = hit.distance;
            //Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.black);
        }
        angle = Mathf.Deg2Rad * 5;
        direction = new Vector3(-Mathf.Cos(angle), 0, Mathf.Sin(angle));
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity))
        {
            veryLeftDist = Mathf.Min(veryLeftDist, hit.distance);
            //Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.yellow);
        }

        //right----------------------------
        angle = Mathf.Deg2Rad * 60;
        direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity))
        {
            rightDist = hit.distance;
            //Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.yellow);
        }
        angle = Mathf.Deg2Rad * 45;
        direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity))
        {
            rightDist = Mathf.Min(rightDist, hit.distance);
            //Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.red);
        }
        angle = Mathf.Deg2Rad * 25;
        direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity))
        {
            veryRightDist = hit.distance;
            //Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.black);
        }
        angle = Mathf.Deg2Rad * 5;
        direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity))
        {
            veryRightDist = Mathf.Min(veryRightDist, hit.distance);
            //Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.black);
        }

        frontDist = frontDist > 30 ? 30 : frontDist;
        rightDist = rightDist > 30 ? 30 : rightDist;
        veryRightDist = veryRightDist > 30 ? 30 : veryRightDist;
        leftDist = leftDist > 30 ? 30 : leftDist;
        veryLeftDist = veryLeftDist > 30 ? 30 : veryLeftDist;

        int layerMask = 1 << 9;
        blocked = Physics.Linecast(transform.position, TestLocation.transform.position, layerMask);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
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

        //Debug.Log(String.Format("front: {0} left: {1} veryLeft: {2}, right: {3} veryRight: {4}", frontDist, littleLeftDist, veryLeftDist, littleRightDist, veryRightDist));

        avoidanceSystem.Calculate(rightDist, veryRightDist, leftDist, veryLeftDist, frontDist, Speed, MovementTargets.Run);

        Speed = Speed < 10.0f ? Speed : 10.0f;
        Speed = Speed > 0.0f ? Speed : 0.0f;
        float aCS = avoidanceSystem.GetOutputValue("SpeedChange");


        if (aCS != -999.0f)
        {
            Speed += aCS;
        }
        if (Speed < 0.0f) Speed = 0.0f;
        float goalAngle = pathfinder.GetGoalAngle(gameObject, TestLocation.transform.position);
        goalAngle = goalAngle > -5f && goalAngle < 5f ? 0f : goalAngle;
        float avoidanceAngle = avoidanceSystem.GetOutputValue("Angle");
        float avoidanceDist = frontDist;
        float goalDist = Vector3.Distance(gameObject.transform.position, TestLocation.transform.position);
        float ratio = avoidanceDist / goalDist;
        if (ratio > 1f) ratio = 1f;
        if (ratio < 0f) ratio = 0f;
        //Debug.Log(ratio);
        if (blocked) ratio = 0.1f;
        else ratio = 0.3f;
        float goalWeight = ratio;
        float avoidanceWeight = 1 - ratio;
        if (avoidanceAngle != -999.0f)
        {
            _finalAngle = avoidanceWeight * avoidanceAngle + goalWeight * goalAngle;
        }
        m_Rigidbody.MovePosition(transform.position + transform.forward * Speed * Time.deltaTime);
        m_EulerAngleVelocity = new Vector3(0, _finalAngle, 0);
        deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * RotationSpeed * Time.deltaTime);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * deltaRotation);
        m_Rigidbody.isKinematic = true;
        m_Rigidbody.isKinematic = false;
    }
}
