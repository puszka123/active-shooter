using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonBehaviours  {
    public List<Behaviour> MyBehaviours { get; private set; }

    public PersonBehaviours()
    {
        MyBehaviours = new List<Behaviour>();
    }

    public void AddBehaviour(Behaviour behaviour)
    {
        MyBehaviours.Add(behaviour);
    }

    public Behaviour GetBehaviourByIndex(int index)
    {
        return MyBehaviours[index];
    }
}
