using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    float frequency = 1f;

    private void OnTriggerStay(Collider other)
    {
        frequency += Time.deltaTime;
        if (frequency >= 1f)
        {
            frequency = 0f;
            other.SendMessage("DoorMet", transform.parent.gameObject);
        }
    }

}
