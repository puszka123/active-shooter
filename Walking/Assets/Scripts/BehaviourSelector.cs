using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourSelector {
    public List<Behaviour> AvailableBehaviours;

    public BehaviourSelector(List<Behaviour> behaviours)
    {
        AvailableBehaviours = behaviours;
    }

    public Behaviour SelectBehaviour()
    {
        return null;
    }
}
