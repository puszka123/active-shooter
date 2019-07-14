using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerExecutor  {
    public bool Executing;
    public Task TaskToExecute;
    public GameObject Me;
    public Shooting Shooting;

    public DestroyerExecutor(GameObject me)
    {
        Me = me;
        Shooting = Me.GetComponent<Shooting>();
    }

    public void ExecuteTask(Task task)
    {
        if (IsTaskExecuting(task)) return; //don't do that again!
        if (task.IsDone) return;
        Executing = true;
        TaskToExecute = task;
        switch (task.Command)
        {
            case Command.DESTROY_DOOR:
                GameObject doorToDestroy = Utils.GetRoom(task)?.Door;
                if(doorToDestroy == null
                    || Utils.ToFar(Me, doorToDestroy, 0.5f))
                {
                    FinishDestroyDoor();
                    return;
                }
                Shooting.ShootDoor(doorToDestroy);
                break;
        }
    }

    public bool IsTaskExecuting(Task task)
    {
        return (TaskToExecute != null && task.Command == TaskToExecute.Command && Executing);
    }

    public void OnDoorDestroyed(GameObject door)
    {
        FinishDestroyDoor();
    }

    public void FinishDestroyDoor()
    {
        TaskToExecute.IsDone = true;
        Executing = false;
    }
}
