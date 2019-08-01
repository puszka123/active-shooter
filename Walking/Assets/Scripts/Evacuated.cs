using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evacuated : MonoBehaviour {
    public List<GameObject> EvacuatedPeople;

	public void Add(GameObject person)
    {
        if(EvacuatedPeople == null)
        {
            EvacuatedPeople = new List<GameObject> { person };
        } 
        else
        {
            EvacuatedPeople.Add(person);
        }
    }

    public void DestroyEvacuated()
    {
        foreach (var item in EvacuatedPeople)
        {
            Destroy(item);
        }
        EvacuatedPeople = null;
    }
}
