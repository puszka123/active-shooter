using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonActions  {
    public List<Action> MyActions { get; private set; }

    public PersonActions()
    {
        MyActions = new List<Action>();
    }

    public void AddAction(Action action)
    {
        MyActions.Add(action);
    }

    public Action GetActionByIndex(int index)
    {
        return MyActions[index];
    }
}
