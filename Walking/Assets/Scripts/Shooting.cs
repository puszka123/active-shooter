using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    //public static System.Random Random = new System.Random();
    public float horizontalDeviation;
    public float verticalUpDeviation;
    public float verticalDownDeviation;
    public float[] ShooterHorizontalAccuracy;
    public float[] ShooterVerticalAccuracy;
    Rigidbody myRigidBody;

    public float RotationSpeed;
    public float ReactionSpeed;
    public float firingRate;
    public bool CanShoot;

    float timer;
    float shootTimer;

    float allHits = 0;
    float hits = 0;

    private void Start()
    {
        CanShoot = true;
        firingRate = 1f;
        timer = 0f;
        RotationSpeed = 30f;
        ReactionSpeed = 0.0f;
        myRigidBody = GetComponent<Rigidbody>();
        horizontalDeviation = 1f;
        verticalUpDeviation =1f;
        verticalDownDeviation = 0.5f;
        ShooterHorizontalAccuracy = new float[] { (90f - horizontalDeviation), (90f + horizontalDeviation) };
        ShooterVerticalAccuracy = new float[] { (90f - verticalUpDeviation), (90f + verticalDownDeviation) };
    }

    private void Update()
    {
        //RaycastHit hit;
        //LayerMask layerMask = LayerMask.GetMask("Wall", "Door", "Employee"); //test add obstacle
        //if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        //{
        //    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        //}

    }

    Transform victim = null;
    Transform lastVictim = null;
    float start = 10f;
    float stop = 170f;
    float increase = 0.1f;

    private void FixedUpdate()
    {

        myRigidBody.isKinematic = true;
        myRigidBody.isKinematic = false;
        float angleHorizontal;
        Vector3 direction;
        RaycastHit hit;
        shootTimer += Time.deltaTime;

        if (victim != null)
        {
            RotateToVictim(victim);
        }

        timer += Time.deltaTime;
        if (timer >= ReactionSpeed)
        {
            timer = 0f;
        }
        else
        {
            return;
        }


        LayerMask layerMask = LayerMask.GetMask("Wall", "Door", "Employee"); //test add obstacle
        float distance = 999f;
        bool noOneInSight = true;
        for (float i = start; i < stop; i += increase)
        {
            angleHorizontal = Mathf.Deg2Rad * i;
            direction = new Vector3(-Mathf.Cos(angleHorizontal), 0, Mathf.Sin(angleHorizontal));
            if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity, layerMask))
            {
                if (hit.transform.CompareTag("Employee") 
                    && hit.transform.GetComponent<PersonStats>().GetHealth() > 0)
                {
                    //Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.yellow);
                    noOneInSight = false;
                    float dist = Utils.Distance(gameObject, hit.transform.gameObject);
                    if (dist < distance)
                    {
                        distance = dist;
                        victim = hit.transform;
                        UpdateFiringRate();
                        UpdateVerticalDeviations();
                    }
                    if (shootTimer >= firingRate)
                    {
                        shootTimer = 0f;
                        Shoot();
                    }
                }
            }
        }
        if (noOneInSight)
        {
            if (victim != null
                && victim.GetComponent<PersonStats>().GetHealth() > 0)
            {
                lastVictim = victim;
                GetComponent<FollowVictim>().SetLastVictim(lastVictim);
            }
            else
            {
                lastVictim = null;
            }
            victim = null;
        }
    }

    public void RotateToVictim(Transform victim)
    {
        Vector3 direction;
        direction = (victim.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);
    }

    public void Shoot()
    {
        RaycastHit hit;
        float angleHorizontal;
        float angleVertical;
        Vector3 direction;
        angleHorizontal = Mathf.Deg2Rad * Random.Range(ShooterHorizontalAccuracy[0], ShooterHorizontalAccuracy[1]);
        angleVertical = Mathf.Deg2Rad * Random.Range(ShooterVerticalAccuracy[0], ShooterVerticalAccuracy[1]);
        LayerMask layerMask = LayerMask.GetMask("Wall", "Door", "Employee"); //test add obstacle
        direction = new Vector3(-Mathf.Cos(angleHorizontal), Mathf.Cos(angleVertical), Mathf.Sin(angleHorizontal));
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.red);
            if (hit.transform.CompareTag("Employee"))
            {
                hit.transform.SendMessage("YouAreHit", new object[] { hit, transform });
            }
        }
    }

    public void UpdateFiringRate()
    {
        if (victim == null)
        {
            firingRate = 1f;
            return;
        }
        float distance = Vector3.Distance(victim.position, transform.position);
        if (distance > 20f*Resources.scale)
        {
            firingRate = 1f;
        }
        else if(distance > 15f * Resources.scale)
        {
            firingRate = 0.75f;
        }
        else
        {
            firingRate = 0.5f;
        }
    }

    public void UpdateVerticalDeviations()
    {
        if (victim == null)
        {
            verticalUpDeviation = 1f;
            return;
        }
        float distance = Vector3.Distance(victim.position, transform.position);
        if (distance > 20f * Resources.scale)
        {
            verticalUpDeviation = 1f;
        }
        else if (distance > 5f * Resources.scale)
        {
            verticalUpDeviation = 2f;
        }
        else if (distance > 2.5f * Resources.scale)
        {
            verticalUpDeviation = 4f;
        }
        else if (distance > 2.0f * Resources.scale)
        {
            verticalUpDeviation = 8f;
        }
        else
        {
            verticalUpDeviation = 12f;
        }
        ShooterVerticalAccuracy = new float[] { (90f - verticalUpDeviation), (90f + verticalDownDeviation) };
    }

    public bool ShootToVictim()
    {
        return victim != null;
    }

    public GameObject LastVictim()
    {
        return lastVictim.gameObject;
    }
}
