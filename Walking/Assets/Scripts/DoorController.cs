using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DoorController : MonoBehaviour
{

    private bool isOpen = false;
    private string doorKey;

    private float closeTime = 1f;

    public bool IsOpen { get { return isOpen; } }

    BoxCollider myCollider;

    private

    // Use this for initialization
    void Start()
    {
        doorKey = transform.name;
        BoxCollider[] res = GetComponents<BoxCollider>();
        foreach (var item in res)
        {
            if(!item.isTrigger)
            {
                myCollider = item;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isOpen)
        {

            if (closeTime <= 0f)
            {
                isOpen = false;
                myCollider.enabled = true;
            }
            else
            {
                closeTime -= Time.deltaTime;
                myCollider.enabled = false;
            }
        }
    }

    public void TryToOpenDoor(string[] keys = null)
    {
        if((keys != null && keys.Contains(doorKey)) || doorKey == null)
        {
            closeTime = 1f;
            isOpen = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        other.SendMessage("DoorMet", gameObject);
    }
}
