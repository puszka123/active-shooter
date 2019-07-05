using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorExecutor {
    public PersonDoor PersonDoor;
    public bool Executing;
    public Action ActionToExecute;

    public DoorExecutor(PersonDoor personDoor)
    {
        PersonDoor = personDoor;
    }

    public void ExecuteAction(Action action, Transform transform)
    {
        if (IsActionExecuting(action)) return; //don't do that again!
        Executing = true;
        switch (action.Command)
        {
            case Command.KNOCK:
                
                break;
        }
    }

    public bool IsActionExecuting(Action action)
    {
        return (ActionToExecute != null && action.Command == ActionToExecute.Command && Executing);
    }
}
