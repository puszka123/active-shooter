using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonStats : MonoBehaviour {
    public float Health;

    private void Start()
    {
        Health = 100f;
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
