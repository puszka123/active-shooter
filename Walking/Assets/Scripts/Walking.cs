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

    private float _finalAngle = 0.0f;

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
        RaycastHit hitFront;
        RaycastHit hitLeftFront;
        RaycastHit hitRightFront;
        RaycastHit hitRight;
        RaycastHit hitVeryRight;
        RaycastHit hitLittleRight;
        RaycastHit hitLeft;
        RaycastHit hitVeryLeft;
        RaycastHit hitLittleLeft;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitFront, Mathf.Infinity))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hitFront.distance, Color.blue);
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(-0.2f, 0, 1f)), out hitLeftFront, Mathf.Infinity))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(-0.05f, 0, 1f)) * hitLeftFront.distance, Color.blue);
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0.2f, 0, 1f)), out hitRightFront, Mathf.Infinity))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(0.05f, 0, 1f)) * hitRightFront.distance, Color.blue);
        }
        float veryShift = 25.0f / 90.0f;
        float shift = 60.0f / 90.0f;
        float littleShift = 140.0f / 90.0f;
        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(-1.0f, 0, shift)), out hitLeft, Mathf.Infinity))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(-1.0f, 0, shift)) * hitLeft.distance, Color.yellow);
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(-1.0f, 0, littleShift)), out hitLittleLeft, Mathf.Infinity))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(-1.0f, 0, littleShift)) * hitLittleLeft.distance, Color.yellow);
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(1.0f, 0, shift)), out hitRight, Mathf.Infinity))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(1.0f, 0, shift)) * hitRight.distance, Color.red);
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(-1.0f, 0, veryShift)), out hitVeryLeft, Mathf.Infinity))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(-1.0f, 0, veryShift)) * hitVeryLeft.distance, Color.black);
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(1.0f, 0, veryShift)), out hitVeryRight, Mathf.Infinity))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(1.0f, 0, veryShift)) * hitVeryRight.distance, Color.black);
        }

        if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(1.0f, 0, littleShift)), out hitLittleRight, Mathf.Infinity))
        {
            //Debug.DrawRay(transform.position, transform.TransformDirection(new Vector3(1.0f, 0, littleShift)) * hitLittleRight.distance, Color.yellow);
        }

        float rightDist = hitRight.distance < 30.0f ? hitRight.distance : 25.0f;
        float veryRightDist = hitVeryRight.distance < 30.0f ? hitVeryRight.distance : 25.0f;
        float leftDist = hitLeft.distance < 30.0f ? hitLeft.distance : 25.0f;
        float veryLeftDist = hitVeryLeft.distance < 30.0f ? hitVeryLeft.distance : 25.0f;
        float frontDist = hitFront.distance < 30.0f ? hitFront.distance : 25.0f;
        float leftFrontDist = hitLeftFront.distance < 30.0f ? hitLeftFront.distance : 25.0f;
        float rightFrontDist = hitRightFront.distance < 30.0f ? hitRightFront.distance : 25.0f;
        float littleRightDist = hitLittleRight.distance < 30.0f ? hitLittleRight.distance : 25.0f;
        float littleLeftDist = hitLittleLeft.distance < 30.0f ? hitLittleLeft.distance : 25.0f;

        frontDist = Mathf.Min(frontDist, leftFrontDist, rightFrontDist);

        //to do 
        veryLeftDist = Mathf.Min(leftDist, veryLeftDist);
        veryRightDist = Mathf.Min(rightDist, veryRightDist);


        //Debug.Log(String.Format("front: {0} left: {1} veryLeft: {2}, right: {3} veryRight: {4}", frontDist, littleLeftDist, veryLeftDist, littleRightDist, veryRightDist));

        avoidanceSystem.Calculate(littleRightDist, veryRightDist, littleLeftDist, veryLeftDist, frontDist, Speed, MovementTargets.Run);

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
        ratio = 0.3f;
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
