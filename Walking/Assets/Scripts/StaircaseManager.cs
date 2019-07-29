using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaircaseManager : MonoBehaviour {
    public List<Person> People;


	// Use this for initialization
	void Start () {
        People = new List<Person>();
	}
	
	public void Join(Person person)
    {
        if(person.CompareTag("Employee"))
        {
            People.Add(person);
        }
    }

    public void Leave(Person person)
    {
        Person found = People.Find(p => p.name == person.name);
        if(found != null)
        {
            People.Remove(found);
        }
    }
}
