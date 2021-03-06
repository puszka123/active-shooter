﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    //public static System.Random Random = new System.Random();
    public float horizontalDeviation;
    public float verticalUpDeviation;
    public float verticalUpDeviationBasic;
    public float verticalDownDeviation;
    public float[] ShooterHorizontalAccuracy;
    public float[] ShooterVerticalAccuracy;
    Rigidbody myRigidBody;

    public float RotationSpeed;
    public float firingRate;
    public float basicFiringRate;
    public float ShootStrength;
    public float ReloadTime;
    float reloadTimer;
    public bool CanShoot;

    public GameObject DoorToDestroy;

    float timer;
    float shootTimer;

    float allHits = 0;
    float hits = 0;

    float soundTimer = 1f;
    float soundMax = 1f;
    float numberOfShots = 0f;

    public ParameterSetter setter;

    private void Start()
    {
        setter = GameObject.FindGameObjectWithTag("ParameterSetter").GetComponent<ParameterSetter>();
        UpdateStats();
        CanShoot = true;
        timer = 0f;
        reloadTimer = 0f;
        RotationSpeed = 30f;
        myRigidBody = GetComponent<Rigidbody>();
        ShooterHorizontalAccuracy = new float[] { (90f - horizontalDeviation), (90f + horizontalDeviation) };
        ShooterVerticalAccuracy = new float[] { (90f - verticalUpDeviation), (90f + verticalDownDeviation) };
    }

    public Transform victim = null;
    Transform lastVictim = null;
    float start = 10f;
    float stop = 170f;
    float increase = 1f;
    public Vector3 DetectPosition;
    private void FixedUpdate()
    {

        myRigidBody.isKinematic = true;
        myRigidBody.isKinematic = false;
        float angleHorizontal;
        Vector3 direction;
        RaycastHit hit;
        shootTimer += Time.deltaTime;
        soundTimer += Time.deltaTime;
        if (soundTimer >= soundMax && numberOfShots > 0)
        {
            ShotSound(numberOfShots);
            soundTimer = 0f;
            numberOfShots = 0f;
        }

        if (victim != null)
        {
            RotateTo(DetectPosition);
        }
        else if (DoorToDestroy != null)
        {
            if (!Utils.ToFar(DoorToDestroy, gameObject, 0.5f))
            {
                RotateTo(DoorToDestroy.transform);
            }
            else
            {
                DoorDestroyed(DoorToDestroy);
            }
        }

        //timer += Time.deltaTime;


        LayerMask layerMask = LayerMask.GetMask("Wall", "Door", "Employee", "ObstacleCollider"); //test add obstacle
        float distance = 999f;
        bool noOneInSight = true;
        for (float i = start; i < stop; i += increase)
        {
            angleHorizontal = Mathf.Deg2Rad * i;
            direction = new Vector3(-Mathf.Cos(angleHorizontal), 0, Mathf.Sin(angleHorizontal));
            if (Physics.Raycast(transform.GetChild(0).position, transform.TransformDirection(direction), out hit, Mathf.Infinity, layerMask))
            {
                //Debug.DrawRay(transform.GetChild(0).position, transform.TransformDirection(direction) * hit.distance, Color.red);
                if (hit.transform.CompareTag("Employee")
                    && hit.transform.GetComponent<PersonStats>().GetHealth() > 0)
                {
                    hit.transform.SendMessage("ISeeYou");
                    noOneInSight = false;
                    float dist = Utils.Distance(gameObject, hit.transform.gameObject);
                    if (dist < distance)
                    {
                        distance = dist;
                        victim = hit.transform;
                        DetectPosition = hit.point;
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

            if (DoorToDestroy != null)
            {
                if (shootTimer >= firingRate)
                {
                    shootTimer = 0f;
                    Shoot();
                }
            }
        }
    }

    public void RotateTo(Transform thing)
    {
        Vector3 direction;
        direction = (thing.position - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);
    }

    public void RotateTo(Vector3 position)
    {
        Vector3 direction;
        direction = (position - transform.position).normalized;
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);
    }


    public void Shoot()
    {
        ++numberOfShots;

        RaycastHit hit;
        float angleHorizontal;
        float angleVertical;
        Vector3 direction;
        angleHorizontal = Mathf.Deg2Rad * Random.Range(ShooterHorizontalAccuracy[0], ShooterHorizontalAccuracy[1]);
        angleVertical = Mathf.Deg2Rad * Random.Range(ShooterVerticalAccuracy[0], ShooterVerticalAccuracy[1]);
        LayerMask layerMask = LayerMask.GetMask("Wall", "Door", "Employee", "ObstacleCollider"); //test add obstacle
        direction = new Vector3(-Mathf.Cos(angleHorizontal), Mathf.Cos(angleVertical), Mathf.Sin(angleHorizontal));
        if (Physics.Raycast(transform.position, transform.TransformDirection(direction), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(direction) * hit.distance, Color.red);
            if (hit.transform.CompareTag("Employee"))
            {
                hit.transform.SendMessage("YouAreHit", new object[] { hit, transform });
            }

            if (hit.transform.CompareTag("Door"))
            {
                hit.transform.SendMessage("YouAreHit", new object[] { hit, transform });
            }
        }
    }

    public void UpdateFiringRate()
    {
        if (victim == null)
        {
            firingRate = basicFiringRate;
            return;
        }

        if(!setter.FiringRateSpeedUpEnabled)
        {
            return;
        }

        float distance = Utils.Distance(victim.position, transform.position);
        if (distance > 20f * Resources.scale)
        {
            firingRate = basicFiringRate;
        }
        else if (distance > 10f * Resources.scale)
        {
            firingRate = 0.75f * basicFiringRate;
        }
        else
        {
            firingRate = 0.5f * basicFiringRate;
        }
    }

    public void UpdateVerticalDeviations()
    {
        if (victim == null)
        {
            verticalUpDeviation = verticalUpDeviationBasic;
            return;
        }
        float distance = Utils.Distance(victim.position, transform.position);
        if (distance > 10f * Resources.scale)
        {
            verticalUpDeviation = verticalUpDeviationBasic;
        }
        else if (distance > 5f * Resources.scale)
        {
            verticalUpDeviation = 2f;
        }
        else if (distance > 2.5f * Resources.scale)
        {
            verticalUpDeviation = 4f;
        }
        else
        {
            verticalUpDeviation = 6f;
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

    public void ShootDoor(GameObject doorToDestroy)
    {
        DoorController doorController = doorToDestroy.GetComponent<DoorController>();
        DoorToDestroy = doorToDestroy;
        RotateTo(doorToDestroy.transform);

        bool doorBlocked = !doorController.Destroyed() && (doorController.IsLocked || doorController.IsBarricaded());
        if (!doorBlocked)
        {
            GetComponent<Person>().PersonMemory.ClearBlockedRooms();
        }
        else
        {
            GetComponent<Person>().PersonMemory.AddBlockedRoom();
            if(!WantDestroyDoor())
            {
                GameObject room = doorToDestroy.GetComponent<DoorController>().MyRoom;
                Room checkedRoom = new Room
                {
                    Id = room.name,
                    Door = doorToDestroy,
                    Employees = room.GetComponent<PathLocation>().RoomEmployees.ToArray(),
                    Reference = room,
                };
                GetComponent<Person>().PersonMemory.AddCheckedRoom(checkedRoom);
                GetComponent<Person>().PersonMemory.ClearFoundRoom();
                DoorDestroyed(doorToDestroy);
                gameObject.SendMessage("SelectAction");
                return;
            }
        }

        if (doorController.Destroyed() || !doorBlocked)
        {
            DoorDestroyed(doorToDestroy);
        }
    }

    public void DoorDestroyed(GameObject door)
    {
        if (DoorToDestroy == null)
        {
            return;
        }
        if (door.name == DoorToDestroy?.name)
        {
            ClearDoorToDestroy();
            GetComponent<Person>().destroyerExecutor.OnDoorDestroyed(door);
        }
    }

    public void ClearDoorToDestroy()
    {
        DoorToDestroy = null;
    }

    public void ShotSound(float numberOfShots)
    {
        foreach (var item in GameObject.FindGameObjectsWithTag("Employee"))
        {
            item.SendMessage("ShotSound", numberOfShots);
        }
    }

    public bool WantDestroyDoor()
    {
        PersonStats stats = GetComponent<PersonStats>();
        PersonMemory memory = GetComponent<Person>().PersonMemory;
        float random = Random.Range(0f, 1f);
        return stats.ForceDoorOpen * memory.BlockedRooms > random;
    }

    public void UpdateStats()
    {
        setter = GameObject.FindGameObjectWithTag("ParameterSetter").GetComponent<ParameterSetter>();
        ShootStrength = setter.ShootStrength;
        firingRate = setter.firingRate;
        basicFiringRate = setter.firingRate;
        horizontalDeviation = setter.horizontalDeviation;
        verticalUpDeviation = setter.verticalUpDeviation;
        verticalUpDeviationBasic = setter.verticalUpDeviation;
        verticalDownDeviation = setter.verticalDownDeviation;
    }
}
