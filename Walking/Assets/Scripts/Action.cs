using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType {
    WORK, FIGHT, EVACUATE, HIDE
}

public abstract class Action {
    public List<Task> Tasks;
    public ActionType Type;

    public abstract void TasksCleaner(PersonMemory memory);
    public abstract void UpdateLimit(PersonMemory memory);
}
