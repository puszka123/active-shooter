using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSelector {
    public List<Action> AvailableActions;

    public ActionSelector(Behaviour behaviour)
    {
        AvailableActions = behaviour.Actions;
    }

    public Action SelectAction()
    {
        return null;
    }
}
