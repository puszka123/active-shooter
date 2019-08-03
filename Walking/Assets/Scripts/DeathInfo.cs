using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathInfo {
    public int SimulationId;
    public float DeathTime;
    public string BehaviourName;
    public string ActionName;
    public int Floor;
    public bool InRoom;

    public string SaveToString()
    {
        return JsonUtility.ToJson(this);
    }
}
