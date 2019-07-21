using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public GameObject Shooter;
    public float seeShooterTimer = 0f;

    bool init = false;
    bool test = false;

    public void Init(int floor, string myRoomId)
    {
        Shooter = GameObject.FindGameObjectWithTag("ActiveShooter");
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
       // CurrentAction = memory.MyActions.GetActionByIndex(0);
       // actionExecutor.ExecuteAction(ref CurrentAction);
        waitingTasks = new TasksQueue();
        List<Behaviour> behaviours = new List<Behaviour>();
        if (!CompareTag("ActiveShooter"))
        {
            behaviours.Add(new ImplementedBehaviours.Evacuate(PersonMemory.MyRoom));
            behaviours.Add(new ImplementedBehaviours.Fight());
            behaviours.Add(new ImplementedBehaviours.Work(PersonMemory.MyRoom));
            behaviours.Add(new ImplementedBehaviours.Hide());
        }
        else
        {
            behaviours.Add(new ImplementedBehaviours.FindAndKill());
        }
        PersonalAttributes = new PersonPersonalAttributes();
        //behaviours.Add(new ImplementedBehaviours.FindAndKill()); test

        BehaviourSelector = new BehaviourSelector(behaviours);
        CurrentBehaviour = BehaviourSelector.SelectBehaviour(this);
        ActionSelector = new ActionSelector(CurrentBehaviour);
        CurrentAction = ActionSelector.SelectAction(this);
        if (CompareTag("ActiveShooter"))
        {
            actionExecutor.ExecuteAction(ref CurrentAction);
        }

        init = true;
    }

    private void FixedUpdate()
    {
        if (!init) return;
        if (GetComponent<PersonStats>() != null && GetComponent<PersonStats>().GetHealth() <= 0f) return;

        if (transform.name.StartsWith("Employee Origin"))
        {
            return;
        }

        if (!CompareTag("ActiveShooter"))
        {
            SeeShooterCheck();
        }

        if (ImActiveShooter() && (FoundVictim() || Fighting()))
        {
            simulationTime += Time.deltaTime;
            timer += Time.deltaTime;
            actionTime += Time.deltaTime;
            return;
        }
        if (simulationTime >= 5f && !test && !CompareTag("ActiveShooter"))
        {
            test = true;
            //CurrentAction = new HideActions.BarricadeDoor();
            CurrentAction = new EvacuationActions.RunToExit();
        }

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
                if (gameObject.name == "Informer" && PersonMemory.GetInformedRooms().Count >= 29)
                {
                    Debug.Log(simulationTime);
                }
                actionExecutor.ExecuteAction(ref CurrentAction);
            }
        }
    }

    public void Die(Transform activeShooter, Vector3 hitPoint)
    {
        init = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
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

    public void PersonStateChanged()
    {
        CurrentBehaviour = BehaviourSelector.SelectBehaviour(this);
        //if(CurrentBehaviour == null)
        //{
        //    CurrentBehaviour = BehaviourSelector.AvailableBehaviours.Find(b => b.GetType() == typeof(ImplementedBehaviours.Fight));
        //}
        ActionSelector = new ActionSelector(CurrentBehaviour);
        CurrentAction.ResetTasks();
        CurrentAction = ActionSelector.SelectAction(this);
    }

    public void SeeShooterCheck()
    {
        if (gameObject.CompareTag("ActiveShooter")) return;
        if (Shooter.GetComponent<Person>().PersonMemory == null) return;
        if (PersonMemory.CurrentFloor == Shooter.GetComponent<Person>().PersonMemory.CurrentFloor)
        {
            seeShooterTimer += Time.deltaTime;

            if (seeShooterTimer >= timerEdge)
            {
                seeShooterTimer = 0f;
                if (Utils.CanSee(gameObject, Shooter))
                {
                    PersonMemory.UpdateActiveShooterInfo(Shooter);
                    if (!MyState.SeeShooter)
                    {
                        MyState.SeeShooter = true;
                        PersonStateChanged();
                    }
                }
                else
                {
                    if (MyState.SeeShooter)
                    {
                        MyState.SeeShooter = false;
                        PersonStateChanged();
                    }
                }
            }
        }
    }

}
