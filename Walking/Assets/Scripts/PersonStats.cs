﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonStats : MonoBehaviour {
    public float Health;
    public float Strength;
    public float LockDoorChance;
    public float BarricadeDoorChance;
    public float HideChance;

    private void Start()
    {
        Health = 100f;
        Strength = 1f;
        LockDoorChance = 0.9f;
        BarricadeDoorChance = 0.1f;
        HideChance = 0.9f;
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
