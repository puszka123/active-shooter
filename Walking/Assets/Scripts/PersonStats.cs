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
    public float shooterCheckRoomWeight = 0.02f;

    public float victimStairsWeight = 0.7f;
    public float shooterSearchFloorChance = 0.8f;
    public float shooterGoUpChance = 0.1f;
    public float shooterGoDownChance = 0.1f;

    public float basicEvacuationChance = 1f;
    public float basicHideChance = 1f;
    public float basicFightChance = 1f;
    

    public ParameterSetter setter;

    private void Start()
    {
        setter = GameObject.FindGameObjectWithTag("ParameterSetter").GetComponent<ParameterSetter>();
        UpdateStats();
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

    public void UpdateStats()
    {
        setter = GameObject.FindGameObjectWithTag("ParameterSetter").GetComponent<ParameterSetter>();
        Health = 100f;
        Strength = Random.Range(setter.StrengthStart, setter.StrengthStop);
        LockDoorChance = Random.Range(setter.LockDoorChanceStart, setter.LockDoorChanceStop);
        BarricadeDoorChance = Random.Range(setter.BarricadeDoorChanceStart, setter.BarricadeDoorChanceStop);
        HideChance = Random.Range(setter.HideChanceStart, setter.HideChanceStop);
        ShotDetection = Random.Range(setter.ShotDetectionStart, setter.ShotDetectionStop);
        FirstDecisionTime = Random.Range(setter.FirstDecisionTimeStart, setter.FirstDecisionTimeStop);
        ShooterLocationDistance = Random.Range(setter.ShooterLocationDistanceStart, setter.ShooterLocationDistanceStop) * Resources.scale;
        ForceDoorOpen = setter.ForceDoorOpenStart;
        Altruism = Random.Range(setter.AltruismStart, setter.AltruismStop);
        aboveMeWeight = setter.aboveMeWeight;
        notAboveMeWeight = setter.notAboveMeWeight;
        distanceWeight = setter.distanceWeight;
        altruismWeight = setter.altruismWeight;
        floorWeight = setter.floorWeight;
        familiarityWeight = setter.familiarityWeight;
        shooterGoDownChance = setter.shooterGoDown;
        shooterGoUpChance = setter.shooterGoUp;
        shooterSearchFloorChance = setter.shooterSearchFloor;
        shooterCheckRoomWeight = setter.shooterCheckRoomWeight;
        basicEvacuationChance = Random.Range(setter.basicEvacuationChanceStart, setter.basicEvacuationChanceStop);
        basicHideChance = Random.Range(setter.basicHideChanceStart, setter.basicHideChanceStop);
        basicFightChance = Random.Range(setter.basicFightChanceStart, setter.basicFightChanceStop);
    }
}
