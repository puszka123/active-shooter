using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Behaviour {
    public List<Action> Actions;

    public abstract float BehaviourHappenProbability(Person person);
}
