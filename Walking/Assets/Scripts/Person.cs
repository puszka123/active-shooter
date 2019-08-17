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
    public float actionTime = 0.1f;
    public float actionTimeEdge = 0.1f;
    public float simulationTime = 0f;
    public BehaviourSelector BehaviourSelector;
    public ActionSelector ActionSelector;
    public GameObject Shooter;
    public float seeShooterTimer = 0f;
    public bool FirstDecision;

    public float talkTime;
    public float talk;

    bool init = false;
    bool test = false;

    public float freezeTimer;
    public bool ShouldFreeze;
    public bool AlreadyFrozen;

    Action informAboutShooter;

    Renderer rend;

    public void Init(int floor, string myRoomId)
    {
        freezeTimer = 0f;
        ShouldFreeze = false;
        AlreadyFrozen = false;
        FirstDecision = false;
        rend = GetComponent<Renderer>();
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

        BehaviourSelector = new BehaviourSelector(behaviours);
        CurrentBehaviour = BehaviourSelector.SelectBehaviour(this);
        SelectColour();
        ActionSelector = new ActionSelector(CurrentBehaviour);
        CurrentAction = ActionSelector.SelectAction(this);
        actionExecutor.ExecuteAction(ref CurrentAction);

        informAboutShooter = new InformActions.TellAboutShooter();
        talk = 1f;
        init = true;
    }

    private void FixedUpdate()
    {
        if (!init) return;
        if (Shooter == null)
        {
            Shooter = GameObject.FindGameObjectWithTag("ActiveShooter");
        }
        if (GetComponent<PersonStats>() != null && GetComponent<PersonStats>().GetHealth() <= 0f) return;

        if (transform.name.StartsWith("Employee Origin"))
        {
            return;
        }
        if (!CompareTag("ActiveShooter"))
        {
            if (ShouldFreeze)
            {
                freezeTimer += Time.deltaTime;
                if (freezeTimer >= GetComponent<PersonStats>().freeze)
                {
                    ShouldFreeze = false;
                }
                else
                {
                    return;
                }
            }

            SeeShooterCheck();
            if (FirstDecision)
            {
                GetComponent<PersonStats>().FirstDecisionTime -= Time.deltaTime;
                if (GetComponent<PersonStats>().FirstDecisionTime <= 0f)
                {
                    FirstDecision = false;
                    PersonMemory.UpdateNodesBlockedByShooter();
                    SelectBehaviour();
                }
            }
        }
        talkTime += Time.deltaTime;
        if (ImActiveShooter() && (FoundVictim() || Fighting() || FollowVictim()))
        {
            //simulationTime += Time.deltaTime;
            timer += Time.deltaTime;
            actionTime += Time.deltaTime;
            PersonMemory.clearBlockedByDoors();
            PersonMemory.ClearFoundRoom();
            CurrentAction.ResetTasks();
            //SelectBehaviour();
            return;
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
                if (CurrentBehaviour.GetType() != typeof(ImplementedBehaviours.Work))
                {
                    waitingTasks.ClearQueue();
                }
                else
                {
                    Task task = waitingTasks.GetTaskToExecute();
                    actionExecutor.ExecuteSingleTask(task);
                }
            }
            else if (!waitingTasks.WaitingTaskIsExecuted()) //if single task is not done
            {
                actionExecutor.ExecuteSingleTask(waitingTasks.ExecutingTask);
            }
            else if (waitingTasks.Tasks.Count == 0 && waitingTasks.WaitingTaskIsExecuted()) //if single task is done and queue is empty
            {
                actionExecutor.ExecuteAction(ref CurrentAction);
            }
        }

        if (talkTime >= talk && !CompareTag("ActiveShooter"))
        {
            talkTime = 0f;
            informAboutShooter.Tasks[0].IsDone = false;
            actionExecutor.ExecuteSingleTask(informAboutShooter.Tasks[0]);
        }
    }

    public void Die(Transform activeShooter, Vector3 hitPoint)
    {
        init = false;
        GameObject.FindGameObjectWithTag("SimulationManager").SendMessage("UpdateDeathInfo", this);
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
        //Vector3 direction = transform.position - activeShooter.position;
        //GetComponent<Rigidbody>().AddForceAtPosition(direction.normalized * 1000f, hitPoint);
        //GetComponent<Rigidbody>().useGravity = true;
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<Person>().enabled = false;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        rend.material.color = Color.white;
    }

    public bool ImActiveShooter()
    {
        return CompareTag("ActiveShooter");
    }

    public bool FoundVictim()
    {
        return GetComponent<Shooting>().ShootToVictim() || GetComponent<ShooterAwarness>().FoundVictim != null;
    }

    public bool Fighting()
    {
        return GetComponent<Fight>().FightIsStarted;
    }

    public bool FollowVictim()
    {
        return GetComponent<FollowVictim>().Executing;
    }

    public void SelectBehaviour()
    {
        bool wasHiding = false;
        if (CurrentBehaviour.GetType() == typeof(ImplementedBehaviours.Hide))
        {
            wasHiding = true;
        }
        CurrentBehaviour = BehaviourSelector.SelectBehaviour(this);
        if (CompareTag("Employee") && CurrentBehaviour.GetType() == typeof(ImplementedBehaviours.Fight) && wasHiding)
        {
            if (PersonMemory?.CurrentRoom != null)
            {
                Utils.InviteOthersToFight(PersonMemory.CurrentRoom.Id);
            }
        }
        SelectColour();
        SelectAction();
    }

    public void SelectAction()
    {
        ActionSelector = new ActionSelector(CurrentBehaviour);
        CurrentAction.ResetTasks();
        Action action = ActionSelector.SelectAction(this);
        if (action != null)
        {
            CurrentAction = action;
        }
        if (!CompareTag("ActiveShooter"))
        {
            if (Utils.CanSee(gameObject, Shooter)
                && !AlreadyFrozen
                && !Utils.IsInAnyRoom(PersonMemory)
                && Random.Range(0f, 1f) < GetComponent<PersonStats>().freezeChance)
            {
                AlreadyFrozen = true;
                ShouldFreeze = true;
            }
        }
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
                    if (!MyState.SeeShooter)
                    {
                        MyState.SeeShooter = true;
                        SelectBehaviour();
                    }
                }
                else
                {
                    if (MyState.SeeShooter)
                    {
                        PersonMemory.UpdateActiveShooterInfo(Shooter, true);
                        MyState.SeeShooter = false;
                        SelectBehaviour();
                    }
                }
            }
        }
    }

    public void ISeeYou()
    {

    }

    public void ShotSound(float numberOfShots)
    {
        if (transform.name.StartsWith("Employee Origin"))
        {
            return;
        }
        if (RecognizedShots(numberOfShots))
        {
            if (PersonMemory.ShooterInfo == null)
            {
                PersonMemory.UpdateActiveShooterInfo(Shooter,
                Utils.CanSee(gameObject, Shooter)
                || (PersonMemory.CurrentFloor == Shooter.GetComponent<Person>().PersonMemory.CurrentFloor
                &&
                !Utils.ToFar(gameObject, Shooter, GetComponent<PersonStats>().ShooterLocationDistance)));

                FirstDecision = true;
            }
            else
            {
                PersonMemory.UpdateActiveShooterInfo(Shooter,
                Utils.CanSee(gameObject, Shooter)
                || (PersonMemory.CurrentFloor == Shooter.GetComponent<Person>().PersonMemory.CurrentFloor
                &&
                !Utils.ToFar(gameObject, Shooter, GetComponent<PersonStats>().ShooterLocationDistance)));

                PersonMemory.UpdateNodesBlockedByShooter();
            }
        }
    }

    public void SelectColour()
    {
        if (CurrentBehaviour.GetType() == typeof(ImplementedBehaviours.Work))
        {
            rend.material.color = Color.green;
        }
        else if (CurrentBehaviour.GetType() == typeof(ImplementedBehaviours.Hide))
        {
            rend.material.color = Color.gray;
        }
        else if (CurrentBehaviour.GetType() == typeof(ImplementedBehaviours.Fight))
        {
            rend.material.color = Color.red;
        }
        else if (CurrentBehaviour.GetType() == typeof(ImplementedBehaviours.Evacuate))
        {
            rend.material.color = Color.blue;
        }
    }

    public bool RecognizedShots(float shots)
    {
        float floorWeight = 0.1f;
        float shotWeight = 0.01f;
        float shotDetection = GetComponent<PersonStats>().ShotDetection;
        int floorDiff = Mathf.Abs(PersonMemory.CurrentFloor - Shooter.GetComponent<Person>().PersonMemory.CurrentFloor);
        float chance = shotDetection - (floorWeight * floorDiff) + (shots * shotWeight);
        if (chance <= 0f)
        {
            return false;
        }
        float random = Random.Range(0f, 1f);
        return random <= chance;
    }

    public void InviteToFight(string roomId)
    {
        if (CurrentBehaviour.GetType() == typeof(ImplementedBehaviours.Fight))
        {
            return;
        }
        PersonStats stats = GetComponent<PersonStats>();
        float lastResort = stats.basicFightChance;
        float chances = stats.basicFightChance + lastResort;
        float random = Random.Range(0f, 1f);


        if (random <= chances)
        {
            CurrentBehaviour = BehaviourSelector.AvailableBehaviours.Find(b => b.GetType() == typeof(ImplementedBehaviours.Fight));
            SelectAction();
        }
    }
}
