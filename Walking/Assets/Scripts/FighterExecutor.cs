using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterExecutor {
    public bool Executing;
    public Task TaskToExecute;
    public GameObject Me;
    public GameObject Shooter;

    public FighterExecutor(GameObject me)
    {
        Me = me;
    }

    public void ExecuteTask(Task task)
    {
        if (IsTaskExecuting(task)) return; //don't do that again!
        if (task.IsDone) return;
        Executing = true;
        TaskToExecute = task;

        if (Shooter == null)
        {
            Shooter = GameObject.FindGameObjectWithTag("ActiveShooter");
        }

        switch (task.Command)
        {
            case Command.FIGHT:
                if (Utils.ToFar(Me, Shooter, 0.2f)
                    || Shooter.GetComponent<Fight>().AlreadyFighting(Me))
                {
                    FinishFighting();
                    return;
                }
                Shooter.GetComponent<Fight>().JoinFight(Me);
                break;
        }
    }

    public bool IsTaskExecuting(Task task)
    {
        return (TaskToExecute != null && task.Command == TaskToExecute.Command && Executing);
    }

    public void FinishFighting()
    {
        TaskToExecute.IsDone = true;
        Executing = false;
    }
}
