using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType {
    WORK, FIGHT, EVACUATE, HIDE, FIND_AND_KILL
}

public abstract class Action {
    public List<Task> Tasks;
    public ActionType Type;

    public abstract void TasksCleaner(PersonMemory memory);
    public abstract void UpdateLimit(PersonMemory memory);
}
