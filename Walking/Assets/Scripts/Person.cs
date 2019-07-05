using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public Walking walkingModule;
    public Finder finderModule;
    PersonMemory memory;
    public PersonMemory PersonMemory { get { return memory; } }
    private float timer = 0.1f;
    private float timerEdge = 0.1f;
    public Behaviour CurrentBehaviour;
    BehaviourExecutor behaviourExecutor;

    bool init = false;

    public void Init(int floor, string myRoomId)
    {
        memory = new PersonMemory();
        memory.Init(floor, transform);
        memory.SetMyRoom(myRoomId);
        walkingModule = new Walking(GetComponent<Rigidbody>());
        finderModule = new Finder(memory);
        behaviourExecutor = new BehaviourExecutor(walkingModule, memory, transform, finderModule);
        CurrentBehaviour = memory.MyBehaviours.GetBehaviourByIndex(0);
        behaviourExecutor.ExecuteBehaviour(ref CurrentBehaviour);
        init = true;
    }

    private void FixedUpdate()
    {
        if (!init) return;
        timer += Time.deltaTime;
        if (timer >= timerEdge && walkingModule.Executing)
        {
            timer = 0f;
            walkingModule.CalculateMovement(transform);
        }
        if (walkingModule.Executing)
        {
            walkingModule.MakeMove(transform, memory);
        }
        behaviourExecutor.ExecuteBehaviour(ref CurrentBehaviour);
        
    }
}
