using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DoorBarricade : MonoBehaviour {
    public float barricadeTime;
    bool isBarricading;
    public bool IsBarricading { get { return IsBarricading; } }
    public List<GameObject> Members;
    public float time;

    // Use this for initialization
    void Start () {
        barricadeTime = 240f;
        time = 0f;
        isBarricading = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(isBarricading)
        {
            List<GameObject> membersToRemove = new List<GameObject>();
            foreach (var member in Members)
            {
                if(Utils.ToFar(member, gameObject))
                {
                    membersToRemove.Add(member);
                }
            }
            foreach (var item in membersToRemove)
            {
                Members.Remove(item);
            }
            time += Time.deltaTime;
            if(time >= GetBarricadeTime()/2)
            {
                GetComponent<DoorController>().AddFirstObstacle();
            }
            if(time >= GetBarricadeTime())
            {
                GetComponent<DoorController>().AddSecondObstacle();
                InformMembers();
                FinishBarricading();
            }
        }
	}

    public void JoinBarricading(GameObject member)
    {
        bool start = false;
        if(Members == null || Members.Count == 0)
        {
            Members = new List<GameObject> { member };
            start = true;
        }
        else if(!Members.Select(m => m.name).Contains(member.name))
        {
            Members.Add(member);
        }
        if (start)
        {
            StartBarricading();
        }
    }

    public void StartBarricading()
    {
        isBarricading = true;
        time = 0f;
    }

    public void FinishBarricading()
    {
        isBarricading = false;
        time = 0f;
        Members = null;
    }

    public float GetBarricadeTime()
    {
        float strengthSum = 0f;
        Members.ForEach(m => strengthSum += m.GetComponent<PersonStats>().Strength);

        return barricadeTime / strengthSum;
    }

    public void InformMembers()
    {
        Members.ForEach(m => m.SendMessage("FinishBarricade", gameObject));
    }
}
