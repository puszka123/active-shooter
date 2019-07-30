using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StaircaseManager : MonoBehaviour {
    public Dictionary<string, List<Person>> People;


	// Use this for initialization
	void Start () {
        People = new Dictionary<string, List<Person>>();
	}
	
	public void Join(object[] args)
    {
        Person person = (Person)args[0];
        string staircase = (string)args[1];
        if(person.CompareTag("Employee"))
        {
            AddPerson(staircase, person);
        }
    }

    public void Leave(object[] args)
    {
        Person person = (Person)args[0];
        string staircase = (string)args[1];
        Person found = People[staircase].Find(p => p.name == person.name);
        if(found != null)
        {
            People[staircase].Remove(found);
        }
    }

    public void AddPerson(string key, Person value)
    {
        if(People.ContainsKey(key))
        {
            People[key].Add(value);
        }
        else
        {
            People.Add(key, new List<Person> { value });
        }
    }
}
