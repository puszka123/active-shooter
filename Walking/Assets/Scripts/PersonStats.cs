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
    public PanicMode Panic;
    public float ShooterLocationDistance;
    public float ForceDoorOpen;

    private void Start()
    {
        Health = 100f;
        Strength = 1f;
        LockDoorChance = 0.9f;
        BarricadeDoorChance = 0.1f;
        HideChance = 0.9f;
        ShotDetection = Random.Range(0.5f, 1f);
        FirstDecisionTime = Random.Range(2f, 10f);
        Panic = PanicMode.LOW;
        ShooterLocationDistance = 15f * Resources.scale;
        ForceDoorOpen = 0.1f;
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
