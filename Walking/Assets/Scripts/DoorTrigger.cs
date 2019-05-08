using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        other.SendMessage("DoorMet", transform.parent.gameObject);
    }
}
