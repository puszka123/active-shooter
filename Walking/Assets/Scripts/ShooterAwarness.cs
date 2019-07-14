using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterAwarness : MonoBehaviour
{
    float timer = 0f;
    public GameObject FoundVictim;
    public float RotationSpeed;

    private void Start()
    {
        RotationSpeed = 10f;
    }

    private void Update()
    {
        //LayerMask layerMask = LayerMask.GetMask("Wall", "Door", "Employee");
        //float angleHorizontal;
        //Vector3 direction;
        //RaycastHit hit;
        //for (float i = 180; i < 360f; i += 1f)
        //{
        //    angleHorizontal = Mathf.Deg2Rad * i;
        //    direction = new Vector3(-Mathf.Cos(angleHorizontal), 0, Mathf.Sin(angleHorizontal));
        //    if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity, layerMask))
        //    {
        //        Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.yellow);
        //    }
        //}
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= 0.5f)
        {
            timer = 0f;
        }
        else
        {
            return;
        }

        if (!GetComponent<Shooting>().ShootToVictim())
        {
            Scan();
        }
    }

    public void Scan()
    {
        float angleHorizontal;
        Vector3 direction;
        RaycastHit hit;

        LayerMask layerMask = LayerMask.GetMask("Wall", "Door", "Employee"); //test add obstacle
        float distance = 999f;
        bool noOneInSight = true;
        for (float i = 180; i < 360f; i += 1f)
        {
            angleHorizontal = Mathf.Deg2Rad * i;
            direction = new Vector3(-Mathf.Cos(angleHorizontal), 0, Mathf.Sin(angleHorizontal));
            if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity, layerMask))
            {
                //Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.yellow);
                if (hit.transform.CompareTag("Employee")
                    && hit.transform.GetComponent<PersonStats>().GetHealth() > 0)
                {
                    if (IsMoving(hit)) //moving
                    {
                        noOneInSight = false;
                        float dist = Utils.Distance(gameObject, hit.transform.gameObject);
                        if (dist < distance)
                        {
                            distance = dist;
                            FoundVictim = hit.transform.gameObject;
                        }
                    }
                }
            }
        }
        if (noOneInSight)
        {
            if (FoundVictim != null
                && FoundVictim.GetComponent<PersonStats>().GetHealth() > 0)
            {
                GetComponent<FollowVictim>().SetLastVictim(FoundVictim.transform);
            }
            FoundVictim = null;
        }
        else
        {
            RotateToVictim();
        }
    }

    private static bool IsMoving(RaycastHit hit)
    {
        if(hit.transform.name.StartsWith("Shoot Tester"))
        {
            return true;
        }
        return hit.transform.GetComponent<Person>().walkingModule.Executing;
    }

    public void RotateToVictim()
    {
        Vector3 direction;
        direction = (FoundVictim.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);
    }
}
