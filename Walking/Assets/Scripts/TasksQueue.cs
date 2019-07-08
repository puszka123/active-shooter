using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TasksQueue
{
    Queue<Task> tasks;
    public Task ExecutingTask;
    public List<Task> Tasks
    {
        get
        {
            return new List<Task>(tasks);
        }
    }

    public TasksQueue()
    {
        tasks = new Queue<Task>();
    }

    public void AddTask(Task task)
    {
        tasks.Enqueue(task);
    }

    public Task GetTaskToExecute()
    {
        ExecutingTask = tasks.Dequeue();
        return ExecutingTask;
    }

    public bool WaitingTaskIsExecuted()
    {
        return ExecutingTask == null || ExecutingTask.IsDone;
    }
}
