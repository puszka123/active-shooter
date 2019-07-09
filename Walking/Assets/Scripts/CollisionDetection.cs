using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection {

    public float rightDist;
    public float veryRightDist;
    public float frontDist;
    public float leftDist;
    public float veryLeftDist;
    public bool blocked = false;
    public bool isStatic = false;


    public void UpdateCollisions(Transform transform, Vector3 goalLocation)
    {
        RaycastHit hit;

        float angle;
        Vector3 direction;
        LayerMask layerMask = LayerMask.GetMask("Wall", "Employee", "Obstacle");

        //front----------------------------
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            if (hit.transform.tag == "Employee")
            {
                //Debug.Log(Vector3.Angle(transform.forward, hit.transform.forward));
                if (Vector3.Angle(transform.forward, hit.transform.forward) < 90f) hit.distance = 30f;
            }

            frontDist = hit.distance;
            isStatic = hit.collider.gameObject.layer == 9;
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
        }
        angle = Mathf.Deg2Rad * 75;
        direction = new Vector3(-Mathf.Cos(angle), 0, Mathf.Sin(angle));
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity, layerMask))
        {
            if (hit.transform.tag == "Employee")
            {
                if (Vector3.Angle(transform.forward, hit.transform.forward) < 90f) hit.distance = 30f;
            }
            if (frontDist < hit.distance) isStatic = hit.collider.gameObject.layer == 9;
            frontDist = Mathf.Min(frontDist, hit.distance);
            //Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.yellow);
        }
        angle = Mathf.Deg2Rad * 75;
        direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity, layerMask))
        {
            if (hit.transform.tag == "Employee")
            {
                if (Vector3.Angle(transform.forward, hit.transform.forward) < 90f) hit.distance = 30f;
            }
            if (frontDist < hit.distance) isStatic = hit.collider.gameObject.layer == 9;
            frontDist = Mathf.Min(frontDist, hit.distance);
            //Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.black);
        }

        //left----------------------------
        angle = Mathf.Deg2Rad * 60;
        direction = new Vector3(-Mathf.Cos(angle), 0, Mathf.Sin(angle));
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity, layerMask))
        {
            if (hit.transform.tag == "Employee")
            {
                if (Vector3.Angle(transform.forward, hit.transform.forward) < 90f) hit.distance = 30f;
            }
            leftDist = hit.distance;
            //Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.yellow);
        }

        angle = Mathf.Deg2Rad * 45;
        direction = new Vector3(-Mathf.Cos(angle), 0, Mathf.Sin(angle));
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity, layerMask))
        {
            if (hit.transform.tag == "Employee")
            {
                if (Vector3.Angle(transform.forward, hit.transform.forward) < 90f) hit.distance = 30f;
            }
            leftDist = Mathf.Min(leftDist, hit.distance);
            //Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.yellow);
        }
        angle = Mathf.Deg2Rad * 25;
        direction = new Vector3(-Mathf.Cos(angle), 0, Mathf.Sin(angle));
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity, layerMask))
        {
            if (hit.transform.tag == "Employee")
            {
                if (Vector3.Angle(transform.forward, hit.transform.forward) < 90f) hit.distance = 30f;
            }
            veryLeftDist = hit.distance;
            //Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.black);
        }
        angle = Mathf.Deg2Rad * 5;
        direction = new Vector3(-Mathf.Cos(angle), 0, Mathf.Sin(angle));
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity, layerMask))
        {
            if (hit.transform.tag == "Employee")
            {
                if (Vector3.Angle(transform.forward, hit.transform.forward) < 90f) hit.distance = 30f;
            }
            veryLeftDist = Mathf.Min(veryLeftDist, hit.distance);
            //Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.yellow);
        }

        //right----------------------------
        angle = Mathf.Deg2Rad * 60;
        direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity, layerMask))
        {
            if (hit.transform.tag == "Employee")
            {
                if (Vector3.Angle(transform.forward, hit.transform.forward) < 90f) hit.distance = 30f;
            }
            rightDist = hit.distance;
            //Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.yellow);
        }
        angle = Mathf.Deg2Rad * 45;
        direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity, layerMask))
        {
            if (hit.transform.tag == "Employee")
            {
                if (Vector3.Angle(transform.forward, hit.transform.forward) < 90f) hit.distance = 30f;
            }
            rightDist = Mathf.Min(rightDist, hit.distance);
            //Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.red);
        }
        angle = Mathf.Deg2Rad * 25;
        direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity, layerMask))
        {
            if (hit.transform.tag == "Employee")
            {
                if (Vector3.Angle(transform.forward, hit.transform.forward) < 90f) hit.distance = 30f;
            }
            veryRightDist = hit.distance;
            //Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.black);
        }
        angle = Mathf.Deg2Rad * 5;
        direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity, layerMask))
        {
            if (hit.transform.tag == "Employee")
            {
                if (Vector3.Angle(transform.forward, hit.transform.forward) < 90f) hit.distance = 30f;
            }
            veryRightDist = Mathf.Min(veryRightDist, hit.distance);
            //Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.black);
        }

        frontDist = frontDist > 30 ? 30 : frontDist;
        rightDist = rightDist > 30 ? 30 : rightDist;
        veryRightDist = veryRightDist > 30 ? 30 : veryRightDist;
        leftDist = leftDist > 30 ? 30 : leftDist;
        veryLeftDist = veryLeftDist > 30 ? 30 : veryLeftDist;

        int layerMask1 = 1 << 9;
        blocked = Physics.Linecast(transform.position, goalLocation, layerMask1);
    }
}
