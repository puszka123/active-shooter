﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BehaviourExecutor
{
    Walking movementExecutor;
    Finder finderExecutor;
    Behaviour behaviourToExecute;
    Transform transform;
    PersonMemory memory;

    public BehaviourExecutor(Walking walkingModule, PersonMemory memory, Transform transform, Finder finderExecutor)
    {
        movementExecutor = walkingModule;
        this.finderExecutor = finderExecutor;
        this.transform = transform;
        this.memory = memory;
    }

    public void ExecuteBehaviour(ref Behaviour behaviourToExecute)
    {
        if (behaviourToExecute.Actions.Where(a => a.IsDone).ToArray().Length == behaviourToExecute.Actions.Count)
        {
            //to do select a new behaviour
            //now we select the same behaviour all the time
            behaviourToExecute.Actions.ForEach(a => a.IsDone = false);
        }
        //if (memory.CurrentFloor == 0 && !(behaviourToExecute is ImplementedBehaviours.HideInMyRoom))
        //{
        //    behaviourToExecute = new ImplementedBehaviours.HideInMyRoom(memory.MyRoom.Id);
        //}

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
                        break;
                    case ActionType.OTHER:
                        break;
                    case ActionType.FINDER:
                        finderExecutor.ExecuteAction(item, transform);
                        break;
                    case ActionType.DOOR:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
