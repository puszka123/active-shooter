using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PanicMode
{
    LOW, MEDIUM, HIGH
}

public class PersonStats : MonoBehaviour {
    public float Health;
    public float Strength;
    public float LockDoorChance;
    public float BarricadeDoorChance;
    public float HideChance;
    public float ShotDetection;
    public float FirstDecisionTime;
    public float ShooterLocationDistance;
    public float ForceDoorOpen;
    public float Altruism;

    //selection hebaviour
    public float aboveMeWeight = 0.9f;
    public float distanceWeight = 0.3f;
    public float notAboveMeWeight = 0.9f;
    public float altruismWeight = 1f;
    public float floorWeight = 0.5f;
    public float familiarityWeight = 0.3f;

    private void Start()
    {
        Health = 100f;
        Strength = 1f;
        LockDoorChance = 0.9f;
        BarricadeDoorChance = 0.1f;
        HideChance = 0.9f;
        ShotDetection = Random.Range(0.5f, 1f);
        FirstDecisionTime = Random.Range(2f, 10f);
        ShooterLocationDistance = 15f * Resources.scale;
        ForceDoorOpen = 0.1f;
        Altruism = 0.5f;

        aboveMeWeight = 0.9f;
        distanceWeight = 0.3f;
        notAboveMeWeight = 0.9f;
        altruismWeight = 1f;
        floorWeight = 0.5f;
        familiarityWeight = 0.3f;
    }

    public void GetDamage(float damage, Transform activeShooter, Vector3 hitPoint)
    {
        Health -= damage;
        if (Health <= 0f)
        {
            GetComponent<Person>().Die(activeShooter, hitPoint);
        }
    }

    public float GetHealth()
    {
        return Health;
    }
}
