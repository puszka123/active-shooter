using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Task {
    public Command Command;
    //public Condition Condition;
    public List<Limit> Limits;
    public TaskType Type;
    public bool IsDone;
    public List<Task> RequiredTasks;

    public bool CanBeExecuted(PersonMemory memory)
    {
        //return Condition.ConditionMet(memory) && RequiredTasksCompleted();
        return RequiredTasksCompleted();
    }

    public bool RequiredTasksCompleted()
    {
        return RequiredTasks.Where(a => !a.IsDone).ToArray().Length == 0;
    }
}
