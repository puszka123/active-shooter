using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Negotiation
{
    //public List<GameObject> members;

    public GameObject PersonNearestDoor(GameObject door, GameObject[] members)
    {
        GameObject selectedMember = members[0];
        float minDistance = GetDistance(selectedMember, door);
        foreach (var member in members)
        {
            float distance = GetDistance(member, door);
            if (minDistance > distance)
            {
                minDistance = distance;
                selectedMember = member;
            }
        }
        return selectedMember;
    }

    public float GetDistance(GameObject a, GameObject b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }
}
