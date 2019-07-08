using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public Walking walkingModule;
    public Finder finderModule;
    public DoorExecutor doorExecutor;
    PersonMemory memory;
    public PersonMemory PersonMemory { get { return memory; } }
    private float timer = 0.1f;
    private float timerEdge = 0.1f;
    public Action CurrentAction;
    ActionExecutor actionExecutor;
    public TalkExecutor talkExecutor;

    public TasksQueue waitingTasks;

    bool init = false;

    public void Init(int floor, string myRoomId)
    {
        memory = new PersonMemory();
        memory.Init(floor, transform);
        memory.SetMyRoom(myRoomId);
        walkingModule = new Walking(GetComponent<Rigidbody>());
        finderModule = new Finder(memory);
        doorExecutor = new DoorExecutor(GetComponent<PersonDoor>(), GetComponent<MyChat>(), gameObject);
        talkExecutor = new TalkExecutor(this);
        actionExecutor = new ActionExecutor(walkingModule, memory, transform, finderModule, doorExecutor, talkExecutor);
        CurrentAction = memory.MyActions.GetActionByIndex(0);
        actionExecutor.ExecuteAction(ref CurrentAction);
        init = true;
        waitingTasks = new TasksQueue();
    }

    private void FixedUpdate()
    {
        if (!init) return;

        timer += Time.deltaTime;
        doorExecutor.UpdateTimer(Time.deltaTime);
        talkExecutor.UpdateTalkingTimer(Time.deltaTime);
        if (timer >= timerEdge && walkingModule.Executing)
        {
            timer = 0f;
            walkingModule.CalculateMovement(transform);
        }
        if (walkingModule.Executing)
        {
            walkingModule.MakeMove(transform, memory);
        }
        if (waitingTasks.Tasks.Count > 0 && waitingTasks.WaitingTaskIsExecuted()) //if single task is executed but queue still full
        {
            actionExecutor.ExecuteSingleTask(waitingTasks.GetTaskToExecute());
        }
        else if (!waitingTasks.WaitingTaskIsExecuted()) //if single task is not done
        {
            actionExecutor.ExecuteSingleTask(waitingTasks.ExecutingTask);
        }
        else if(waitingTasks.Tasks.Count == 0 && waitingTasks.WaitingTaskIsExecuted()) //if single task is done and queue is empty
        {
            actionExecutor.ExecuteAction(ref CurrentAction);
        }
        
    }
}
