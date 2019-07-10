using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActionExecutor
{
    Walking movementExecutor;
    Finder finderExecutor;
    DoorExecutor doorExecutor;
    Action actionToExecute;
    Transform transform;
    PersonMemory memory;
    TalkExecutor talkExecutor;

    public ActionExecutor(Walking walkingModule, PersonMemory memory, Transform transform, 
        Finder finderExecutor, DoorExecutor doorExecutor, TalkExecutor talkExecutor)
    {
        movementExecutor = walkingModule;
        this.doorExecutor = doorExecutor;
        this.finderExecutor = finderExecutor;
        this.transform = transform;
        this.memory = memory;
        this.talkExecutor = talkExecutor;
    }

    public void ExecuteAction(ref Action actionToExecute)
    {
        //test
        if (actionToExecute.Tasks.Where(a => a.IsDone).ToArray().Length == actionToExecute.Tasks.Count)
        {
            actionToExecute.Tasks.ForEach(a => a.IsDone = false);
        }

        actionToExecute.TasksCleaner(memory);
        foreach (var item in actionToExecute.Tasks)
        {
            if (item.CanBeExecuted(memory) && !item.IsDone)
            {
                actionToExecute.UpdateLimit(memory);
                switch (item.Type)
                {
                    case TaskType.MOVEMENT:
                        movementExecutor.ExecuteTask(item, memory, transform);
                        break;
                    case TaskType.TALK:
                        talkExecutor.CheckTalking();
                        talkExecutor.ExecuteTask(item);
                        break;
                    case TaskType.OTHER:
                        break;
                    case TaskType.FINDER:
                        finderExecutor.ExecuteTask(item, transform);
                        break;
                    case TaskType.DOOR:
                       // doorExecutor.CheckDoor(doorExecutor.GetRoom(item)?.Door);
                        doorExecutor.CheckDoor(item);
                        doorExecutor.ExecuteTask(item, transform);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void ExecuteSingleTask(Task task)
    {
        if (task.CanBeExecuted(memory) && !task.IsDone)
        {
            switch (task.Type)
            {
                case TaskType.MOVEMENT:
                    movementExecutor.ExecuteTask(task, memory, transform);
                    break;
                case TaskType.TALK:
                    break;
                case TaskType.OTHER:
                    break;
                case TaskType.FINDER:
                    finderExecutor.ExecuteTask(task, transform);
                    break;
                case TaskType.DOOR:
                    //doorExecutor.CheckDoor(doorExecutor.GetRoom(task)?.Door);
                    doorExecutor.CheckDoor(task);
                    doorExecutor.ExecuteTask(task, transform);
                    break;
                default:
                    break;
            }
        }
    }
}
