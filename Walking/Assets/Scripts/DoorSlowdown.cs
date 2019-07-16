using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSlowdown : MonoBehaviour {
    public float SlowdownFactor;

    private void Start()
    {
        SlowdownFactor = 2f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Employee" || other.tag == "ActiveShooter")
        {
            other.GetComponent<Person>().walkingModule.Slowdown(SlowdownFactor);
            DoorController dc = transform.parent.GetComponent<DoorController>();
            if(dc.IsOpen)
            {
                dc.OpenDoor();
            }
        }
    }
}
