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
    public Behaviour CurrentBehaviour;
    BehaviourExecutor behaviourExecutor;
    public TalkExecutor talkExecutor;

    public ActionsQueue waitingActions;

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
        behaviourExecutor = new BehaviourExecutor(walkingModule, memory, transform, finderModule, doorExecutor, talkExecutor);
        CurrentBehaviour = memory.MyBehaviours.GetBehaviourByIndex(0);
        behaviourExecutor.ExecuteBehaviour(ref CurrentBehaviour);
        init = true;
        waitingActions = new ActionsQueue();
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
        if (waitingActions.Actions.Count > 0 && waitingActions.WaitingActionIsExecuted()) //if single action is executed but queue still full
        {
            behaviourExecutor.ExecuteSingleAction(waitingActions.GetActionToExecute());
        }
        else if (!waitingActions.WaitingActionIsExecuted()) //if single action is not done
        {
            behaviourExecutor.ExecuteSingleAction(waitingActions.ExecutingAction);
        }
        else if(waitingActions.Actions.Count == 0 && waitingActions.WaitingActionIsExecuted()) //if single action is done and queue is empty
        {
            behaviourExecutor.ExecuteBehaviour(ref CurrentBehaviour);
        }
        
    }
}
