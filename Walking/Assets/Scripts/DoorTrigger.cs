using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DoorTrigger : MonoBehaviour
{
    float frequency = 1f;
    public string Side;
    public List<GameObject> People;

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
        AddPerson(other.gameObject);
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

    private void OnTriggerExit(Collider other)
    {
        RemovePerson(other.gameObject);
    }

    public void AddPerson(GameObject person)
    {
        if (People == null) People = new List<GameObject> { person };
        else People.Add(person);
    }

    public void RemovePerson(GameObject person)
    {
        People = People.Where(p => p.name != person.name).ToList();
    }

}
