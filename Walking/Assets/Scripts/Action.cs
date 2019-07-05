using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Action {
    public Command Command;
    //public Condition Condition;
    public List<Limit> Limits;
    public ActionType Type;
    public bool IsDone;
    public List<Action> RequiredActions;

    public bool CanBeExecuted(PersonMemory memory)
    {
        //return Condition.ConditionMet(memory) && RequiredActionsCompleted();
        return RequiredActionsCompleted();
    }

    public bool RequiredActionsCompleted()
    {
        return RequiredActions.Where(a => !a.IsDone).ToArray().Length == 0;
    }
}
