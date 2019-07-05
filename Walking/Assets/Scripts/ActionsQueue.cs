using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionsQueue
{
    Queue<Action> actions;
    public Action ExecutingAction;
    public List<Action> Actions
    {
        get
        {
            return new List<Action>(actions);
        }
    }

    public ActionsQueue()
    {
        actions = new Queue<Action>();
    }

    public void AddAction(Action action)
    {
        actions.Enqueue(action);
    }

    public Action GetActionToExecute()
    {
        ExecutingAction = actions.Dequeue();
        return ExecutingAction;
    }

    public bool WaitingActionIsExecuted()
    {
        return ExecutingAction == null || ExecutingAction.IsDone;
    }
}
