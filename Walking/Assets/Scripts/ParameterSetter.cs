using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParameterSetter : MonoBehaviour {
    public float HealthStart;
    public float HealthStop;
    public float StrengthStart;
    public float StrengthStop;
    public float LockDoorChanceStart;
    public float LockDoorChanceStop;
    public float BarricadeDoorChanceStart;
    public float BarricadeDoorChanceStop;
    public float HideChanceStart;
    public float HideChanceStop;
    public float ShotDetectionStart;
    public float ShotDetectionStop;
    public float FirstDecisionTimeStart;
    public float FirstDecisionTimeStop;
    public float ShooterLocationDistanceStart;
    public float ShooterLocationDistanceStop;
    public float ForceDoorOpenStart;
    public float ForceDoorOpenStop;
    public float AltruismStart;
    public float AltruismStop;

    public float horizontalDeviation;
    public float verticalUpDeviation;
    public float verticalDownDeviation;
    public float firingRate;
    public float ShootStrength;

    //selection hebaviour
    public float aboveMeWeight;
    public float distanceWeight;
    public float notAboveMeWeight;
    public float altruismWeight;
    public float floorWeight;
    public float familiarityWeight;
    public float shooterCheckRoomWeight;

    public float shooterSearchFloor;
    public float shooterGoUp;
    public float shooterGoDown;
    public float victimStairsWeight;

    public float LawEnforcementArrival;
    public float BarricadingTime;
    public bool FiringRateSpeedUpEnabled;

    public int SimulationsCount;

    public float basicEvacuationChanceStart;
    public float basicEvacuationChanceStop;
    public float basicHideChanceStart;
    public float basicHideChanceStop;
    public float basicFightChanceStart;
    public float basicFightChanceStop;

    public float myRoomChanceStart;
    public float myRoomChanceStop;
    public float anyRoomChanceStart;
    public float anyRoomChanceStop;
    public float exitChanceStart;
    public float exitChanceStop;

    public float DropGunProbability;
    public float ShootProbability;

    public float freezeStart;
    public float freezeStop;

    public float freezeChanceStart;
    public float freezeChanceStop;

    public void SetStrengthStart(string value)
    {
        value = value.Trim();
        StrengthStart = float.Parse(value);
    }

    public void SetStrengthStop(string value)
    {
        value = value.Trim();
        StrengthStop = float.Parse(value);
    }

    public void SetLockDoorChanceStart(string value)
    {
        value = value.Trim();
        LockDoorChanceStart = float.Parse(value);
    }

    public void SetLockDoorChanceStop(string value)
    {
        value = value.Trim();
        LockDoorChanceStop = float.Parse(value);
    }

    public void SetBarricadeDoorChanceStart(string value)
    {
        value = value.Trim();
        BarricadeDoorChanceStart = float.Parse(value);
    }

    public void SetBarricadeDoorChanceStop(string value)
    {
        value = value.Trim();
        BarricadeDoorChanceStop = float.Parse(value);
    }

    public void SetHideChanceStart(string value)
    {
        value = value.Trim();
        HideChanceStart = float.Parse(value);
    }

    public void SetHideChanceStop(string value)
    {
        value = value.Trim();
        HideChanceStop = float.Parse(value);
    }

    public void SetShotDetectionStart(string value)
    {
        value = value.Trim();
        ShotDetectionStart = float.Parse(value);
    }

    public void SetShotDetectionStop(string value)
    {
        value = value.Trim();
        ShotDetectionStop = float.Parse(value);
    }

    public void SetFirstDecisionTimeStart(string value)
    {
        value = value.Trim();
        FirstDecisionTimeStart = float.Parse(value);
    }

    public void SetFirstDecisionTimeStop(string value)
    {
        value = value.Trim();
        FirstDecisionTimeStop = float.Parse(value);
    }

    public void SetShooterLocationDistanceStart(string value)
    {
        value = value.Trim();
        ShooterLocationDistanceStart = float.Parse(value);
    }

    public void SetShooterLocationDistanceStop(string value)
    {
        value = value.Trim();
        ShooterLocationDistanceStop = float.Parse(value);
    }

    public void SetForceDoorOpenStart(string value)
    {
        value = value.Trim();
        ForceDoorOpenStart = float.Parse(value);
    }

    public void SetForceDoorOpenStop(string value)
    {
        value = value.Trim();
        ForceDoorOpenStop = float.Parse(value);
    }

    public void SetAltruismStart(string value)
    {
        value = value.Trim();
        AltruismStart = float.Parse(value);
    }

    public void SetAltruismStop(string value)
    {
        value = value.Trim();
        AltruismStop = float.Parse(value);
    }

    public void SetAboveMeWeight(string value)
    {
        value = value.Trim();
        aboveMeWeight = float.Parse(value);
    }

    public void SetDistanceWeight(string value)
    {
        value = value.Trim();
        distanceWeight = float.Parse(value);
    }

    public void SetNotAboveMeWeight(string value)
    {
        value = value.Trim();
        notAboveMeWeight = float.Parse(value);
    }

    public void SetAltruismWeight(string value)
    {
        value = value.Trim();
        altruismWeight = float.Parse(value);
    }

    public void SetFloorWeight(string value)
    {
        value = value.Trim();
        floorWeight = float.Parse(value);
    }

    public void SetFamiliarityWeight(string value)
    {
        value = value.Trim();
        familiarityWeight = float.Parse(value);
    }

    public void SetHorizontalDeviation(string value)
    {
        value = value.Trim();
        horizontalDeviation = float.Parse(value);
    }

    public void SetVerticalUpDeviation(string value)
    {
        value = value.Trim();
        verticalUpDeviation = float.Parse(value);
    }

    public void SetVerticalDownDeviation(string value)
    {
        value = value.Trim();
        verticalDownDeviation = float.Parse(value);
    }

    public void SetFiringRate(string value)
    {
        value = value.Trim();
        firingRate = float.Parse(value);
    }

    public void ShotDestroyStrength(string value)
    {
        value = value.Trim();
        ShootStrength = float.Parse(value);
    }

    public void SetLawEnforcementArrival(string value)
    {
        value = value.Trim();
        LawEnforcementArrival = float.Parse(value);
    }

    public void SetBarricadingTime(string value)
    {
        value = value.Trim();
        BarricadingTime = float.Parse(value);
    }

    public void SetFiringRateSpeedUp(bool value)
    {
        FiringRateSpeedUpEnabled = GameObject.Find("FiringRateToggle").GetComponent<Toggle>().isOn;
    }

    public void SetShooterSearchFloor(string value)
    {
        value = value.Trim();
        shooterSearchFloor = float.Parse(value);
    }

    public void SetShooterGoUp(string value)
    {
        value = value.Trim();
        shooterGoUp = float.Parse(value);
    }

    public void SetShooterGoDown(string value)
    {
        value = value.Trim();
        shooterGoDown = float.Parse(value);
    }

    public void SetVictimStairsWeight(string value)
    {
        value = value.Trim();
        victimStairsWeight = float.Parse(value);
    }

    public void SetSimulationsCount(string value)
    {
        value = value.Trim();
        SimulationsCount = int.Parse(value);
    }

    public void SetShooterCheckRoomWeight(string value)
    {
        value = value.Trim();
        shooterCheckRoomWeight = float.Parse(value);
    }

    public void SetEvacuationChanceStart(string value)
    {
        value = value.Trim();
        basicEvacuationChanceStart = float.Parse(value);
    }

    public void SetEvacuationChanceStop(string value)
    {
        value = value.Trim();
        basicEvacuationChanceStop = float.Parse(value);
    }

    public void SetBasicHideChanceStart(string value)
    {
        value = value.Trim();
        basicHideChanceStart = float.Parse(value);
    }

    public void SetBasicHideChanceStop(string value)
    {
        value = value.Trim();
        basicHideChanceStop = float.Parse(value);
    }


    public void SetFightChanceStart(string value)
    {
        value = value.Trim();
        basicFightChanceStart = float.Parse(value);
    }

    public void SetFightChanceStop(string value)
    {
        value = value.Trim();
        basicFightChanceStop = float.Parse(value);
    }

    public void SetMyRoomChanceStart(string value)
    {
        value = value.Trim();
        myRoomChanceStart = float.Parse(value);
    }

    public void SetMyRoomChanceStop(string value)
    {
        value = value.Trim();
        myRoomChanceStop = float.Parse(value);
    }

    public void SetAnyRoomChanceStart(string value)
    {
        value = value.Trim();
        anyRoomChanceStart = float.Parse(value);
    }

    public void SetAnyRoomChanceStop(string value)
    {
        value = value.Trim();
        anyRoomChanceStop = float.Parse(value);
    }

    public void SetDropGunProbability(string value)
    {
        value = value.Trim();
        DropGunProbability = float.Parse(value);
    }

    public void SetShootProbability(string value)
    {
        value = value.Trim();
        ShootProbability = float.Parse(value);
    }

    public void SetFreezeStart(string value)
    {
        value = value.Trim();
        freezeStart = float.Parse(value);
    }

    public void SetFreezeStop(string value)
    {
        value = value.Trim();
        freezeStop = float.Parse(value);
    }

    public void SetFreezeChanceStart(string value)
    {
        value = value.Trim();
        freezeChanceStart = float.Parse(value);
    }

    public void SetFreezeChanceStop(string value)
    {
        value = value.Trim();
        freezeChanceStop = float.Parse(value);
    }

    public void SaveChanges()
    {
        foreach (var item in GameObject.FindGameObjectsWithTag("Employee"))
        {
            item.GetComponent<PersonStats>().UpdateStats();
        }

        GameObject.FindGameObjectWithTag("ActiveShooter").GetComponent<Shooting>().UpdateStats();
        GameObject.FindGameObjectWithTag("ActiveShooter").GetComponent<PersonStats>().UpdateStats();
        foreach (var item in GameObject.FindGameObjectsWithTag("Door")) 
        {
            item.GetComponent<DoorBarricade>().UpdateParams();
        }
        GameObject.FindGameObjectWithTag("SimulationManager").GetComponent<FPSDisplayScript>().UpdateParams();
        GameObject.FindGameObjectWithTag("SimulationManager").GetComponent<SimulationManager>().UpdateParams();
    }

    public string SaveToString()
    {
        return JsonUtility.ToJson(this);
    }
}
