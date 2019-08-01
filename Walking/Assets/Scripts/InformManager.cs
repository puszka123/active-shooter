using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformManager : MonoBehaviour {
    Dictionary<int, List<MyChat>> notInformed;

    public float time = 0.0f;
    public bool initied;
    public bool CanInit;

    // Use this for initialization
    void Start () {
        initied = false;
        CanInit = false;

    }
	
	// Update is called once per frame
	void Update () {    
        if(CanInit && !initied)
        {
            time += Time.deltaTime;
        }

		if(!initied && CanInit && time >= 1.5f)
        {
            initied = true;
            notInformed = new Dictionary<int, List<MyChat>>();
            GameObject[] employees = GameObject.FindGameObjectsWithTag("Employee");
            foreach (var employee in employees)
            {
                if(employee.GetComponent<Person>().PersonMemory == null)
                {
                    continue;
                }
                int floor = employee.GetComponent<Person>().PersonMemory.CurrentFloor;
                if(!notInformed.ContainsKey(floor))
                {
                    notInformed.Add(floor, new List<MyChat> { employee.GetComponent<MyChat>() });
                }
                else
                {
                    notInformed[floor].Add(employee.GetComponent<MyChat>());
                }
            }
        }
        else if(!initied)
        {
            time += Time.deltaTime;
        }
	}

    public void Leave(GameObject employee)
    {
        int floor = employee.GetComponent<Person>().PersonMemory.CurrentFloor;
        if(notInformed.ContainsKey(floor))
        {
            MyChat chat = employee.GetComponent<MyChat>();
            notInformed[floor].Remove(notInformed[floor].Find(ch => ch.name == chat.name));
        }
    }

    public List<MyChat> GetFloorEmployees(int floor)
    {
        return notInformed[floor];
    }

    public void StartInitialization()
    {
        CanInit = true;
        time = 0f;
    }
}
