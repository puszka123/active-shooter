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
    public Behaviour CurrentBehaviour;
    ActionExecutor actionExecutor;
    public TalkExecutor talkExecutor;
    public DestroyerExecutor destroyerExecutor;
    public FighterExecutor fighterExecutor;
    public PersonState MyState;
    public TasksQueue waitingTasks;
    public float actionTime = 0.5f;
    public float actionTimeEdge = 0.5f;
    public float simulationTime = 0f;
    public BehaviourSelector BehaviourSelector;
    public ActionSelector ActionSelector;
    public PersonPersonalAttributes PersonalAttributes;

    bool init = false;
    bool test = false;

    public void Init(int floor, string myRoomId)
    {
        MyState = new PersonState();
        memory = new PersonMemory();
        memory.Init(floor, transform);
        memory.SetMyRoom(myRoomId);
        walkingModule = new Walking(GetComponent<Rigidbody>());
        finderModule = new Finder(memory);
        doorExecutor = new DoorExecutor(GetComponent<PersonDoor>(), GetComponent<MyChat>(), gameObject);
        talkExecutor = new TalkExecutor(this);
        destroyerExecutor = new DestroyerExecutor(gameObject);
        fighterExecutor = new FighterExecutor(gameObject);
        actionExecutor = new ActionExecutor(walkingModule, memory, transform, finderModule, doorExecutor, talkExecutor, destroyerExecutor,
            fighterExecutor);
        CurrentAction = memory.MyActions.GetActionByIndex(0);
        actionExecutor.ExecuteAction(ref CurrentAction);
        waitingTasks = new TasksQueue();
        List<Behaviour> behaviours = new List<Behaviour>();
        behaviours.Add(new ImplementedBehaviours.Evacuate(PersonMemory.MyRoom));
        behaviours.Add(new ImplementedBehaviours.Fight());
        behaviours.Add(new ImplementedBehaviours.Work(PersonMemory.MyRoom));
        behaviours.Add(new ImplementedBehaviours.Hide());
        PersonalAttributes = new PersonPersonalAttributes();
        //behaviours.Add(new ImplementedBehaviours.FindAndKill()); test

        BehaviourSelector = new BehaviourSelector(behaviours);
        init = true;
    }

    private void FixedUpdate()
    {
        if (!init) return;

        if(transform.name.StartsWith("Employee Origin"))
        {
            return;
        }

        if(ImActiveShooter() && (FoundVictim() || Fighting()))
        {
            simulationTime += Time.deltaTime;
            timer += Time.deltaTime;
            actionTime += Time.deltaTime;
            return;
        }
        if(simulationTime >= 5f && !test)
        {
            test = true;
            //CurrentAction = new HideActions.BarricadeDoor();
            CurrentAction = new EvacuationActions.RunToExit();
        }

        //CurrentBehaviour = 

        simulationTime += Time.deltaTime;
        timer += Time.deltaTime;
        actionTime += Time.deltaTime;
        doorExecutor.UpdateTimer(Time.deltaTime);
        talkExecutor.UpdateTalkingTimer(Time.deltaTime);

        if (timer >= timerEdge && walkingModule.Executing)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().isKinematic = false;
            timer = 0f;
            walkingModule.CalculateMovement(transform);
        }
        if (walkingModule.Executing)
        {
            walkingModule.MakeMove(transform, memory);
        }
        if (actionTime >= actionTimeEdge)
        {
            actionTime = 0;
            if (waitingTasks.Tasks.Count > 0 && waitingTasks.WaitingTaskIsExecuted()) //if single task is executed but queue still full
            {
                actionExecutor.ExecuteSingleTask(waitingTasks.GetTaskToExecute());
            }
            else if (!waitingTasks.WaitingTaskIsExecuted()) //if single task is not done
            {
                actionExecutor.ExecuteSingleTask(waitingTasks.ExecutingTask);
            }
            else if (waitingTasks.Tasks.Count == 0 && waitingTasks.WaitingTaskIsExecuted()) //if single task is done and queue is empty
            {
                if(gameObject.name == "Informer" && PersonMemory.GetInformedRooms().Count >= 29)
                {
                    Debug.Log(simulationTime);
                }
                actionExecutor.ExecuteAction(ref CurrentAction);
            }
        }
    }

    public void Die(Transform activeShooter, Vector3 hitPoint)
    {
        float thrust = 100f;
        init = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        //GetComponent<ShootTester>().Speed = Resources.Stay; //test
        Vector3 direction = transform.position - activeShooter.position;
        GetComponent<Rigidbody>().AddForceAtPosition(direction.normalized * 1000f, hitPoint);
        GetComponent<Rigidbody>().useGravity = true;
    }

    public bool ImActiveShooter()
    {
        return GetComponent<Shooting>() != null;
    }

    public bool FoundVictim()
    {
        return GetComponent<Shooting>().ShootToVictim() || GetComponent<ShooterAwarness>().FoundVictim != null;
    }

    public bool Fighting()
    {
        return GetComponent<Fight>().FightIsStarted;
    }

}
