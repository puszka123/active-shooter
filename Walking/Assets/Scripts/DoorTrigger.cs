using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    float frequency = 1f;
    public string Side;

    private void Start()
    {
        Side = gameObject.name.Split(' ')[1];          
    }

    private void OnTriggerStay(Collider other)
    {
        frequency += Time.deltaTime;
        if (frequency >= 1f)
        {
            frequency = 0f;
            other.SendMessage("DoorMet", transform.parent.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        other.SendMessage("DoorMet", transform.parent.gameObject);
        if(Side == "In")
        {
            other.SendMessage("YouEnterRoom", transform.parent.gameObject);
        }
        if(Side == "Out")
        {
            other.SendMessage("YouExitRoom", transform.parent.gameObject);
        }
    }

}
