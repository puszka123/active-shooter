using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BehaviourExecutor
{
    Walking movementExecutor;
    Finder finderExecutor;
    DoorExecutor doorExecutor;
    Behaviour behaviourToExecute;
    Transform transform;
    PersonMemory memory;
    TalkExecutor talkExecutor;

    public BehaviourExecutor(Walking walkingModule, PersonMemory memory, Transform transform, 
        Finder finderExecutor, DoorExecutor doorExecutor, TalkExecutor talkExecutor)
    {
        movementExecutor = walkingModule;
        this.doorExecutor = doorExecutor;
        this.finderExecutor = finderExecutor;
        this.transform = transform;
        this.memory = memory;
        this.talkExecutor = talkExecutor;
    }

    public void ExecuteBehaviour(ref Behaviour behaviourToExecute)
    {
        //test
        if (behaviourToExecute.Actions.Where(a => a.IsDone).ToArray().Length == behaviourToExecute.Actions.Count)
        {
            behaviourToExecute.Actions.ForEach(a => a.IsDone = false);
        }

        behaviourToExecute.ActionsCleaner(memory);
        foreach (var item in behaviourToExecute.Actions)
        {
            if (item.CanBeExecuted(memory) && !item.IsDone)
            {
                behaviourToExecute.UpdateActionsLimits(memory);
                switch (item.Type)
                {
                    case ActionType.MOVEMENT:
                        movementExecutor.ExecuteAction(item, memory, transform);
                        break;
                    case ActionType.TALK:
                        talkExecutor.CheckTalking();
                        talkExecutor.ExecuteAction(item);
                        break;
                    case ActionType.OTHER:
                        break;
                    case ActionType.FINDER:
                        finderExecutor.ExecuteAction(item, transform);
                        break;
                    case ActionType.DOOR:
                       // doorExecutor.CheckDoor(doorExecutor.GetRoom(item)?.Door);
                        doorExecutor.CheckDoor(item);
                        doorExecutor.ExecuteAction(item, transform);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void ExecuteSingleAction(Action action)
    {
        if (action.CanBeExecuted(memory) && !action.IsDone)
        {
            switch (action.Type)
            {
                case ActionType.MOVEMENT:
                    movementExecutor.ExecuteAction(action, memory, transform);
                    break;
                case ActionType.TALK:
                    break;
                case ActionType.OTHER:
                    break;
                case ActionType.FINDER:
                    finderExecutor.ExecuteAction(action, transform);
                    break;
                case ActionType.DOOR:
                    //doorExecutor.CheckDoor(doorExecutor.GetRoom(action)?.Door);
                    doorExecutor.CheckDoor(action);
                    doorExecutor.ExecuteAction(action, transform);
                    break;
                default:
                    break;
            }
        }
    }
}
