using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Obstacle : MonoBehaviour {
    public float Weight;
    public float TargetSpeed;
    public List<GameObject> Pickers;
    public List<GameObject> PossiblePickers;
    public bool InBlock;

	// Use this for initialization
	void Start () {
        Weight = 10;
        TargetSpeed = Resources.SlowWalk;
        Pickers = new List<GameObject>();
        PossiblePickers = new List<GameObject>();
        InBlock = false;
	}

    private void Update()
    {
        
    }

    public void TryToPickUp(GameObject person)
    {
        if (!Pickers.Select(p => p.name).Contains(person.name))
        {
            Pickers.Add(person);
        }
        if(PersonCanPick(person))
        {
            person.SendMessage("PickMe", gameObject);
        }
    }

    public bool PersonCanPick(GameObject person)
    {
        //to do add person strength parameter
        //return person.IsStrongEnough(gameObject);
        return true; //test
    }
}
