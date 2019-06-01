using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public Walking walkingModule;
    PersonMemory memory;
    public PersonMemory PersonMemory { get { return memory; } }
    private float timer = 0.1f;
    private float timerEdge = 0.1f;
    public Behaviour CurrentBehaviour;

    bool init = false;

    public void Init(int floor)
    {
        memory = new PersonMemory();
        memory.Init(floor, transform.position);
        walkingModule = new Walking(GetComponent<Rigidbody>());
        CurrentBehaviour = memory.MyBehaviours.GetBehaviourByIndex(0);
        ExecuteBehaviour();
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
        else
        {
            ExecuteBehaviour();
        }
    }

    public void ExecuteBehaviour()
    {
        foreach (var item in CurrentBehaviour.Actions)
        {
            if (ConditionIsMet(item.Condition.Limit))
            {
                walkingModule.ExecuteCommand(item.Command, memory, transform);
            }
        }
    }

    public bool ConditionIsMet(string condition)
    {
        string[] elements = condition.Split(' ');
        if (elements[0] != "if") Debug.Log("Invalid condition - no 'if'");
        if (elements[1] == "floor")
        {
            if (elements[2] != "is") Debug.Log("Invalid condition - no 'is'");
            bool not = false;
            int floor = -999;
            if (!int.TryParse(elements[3], out floor))
            {
                if (!int.TryParse(elements[4], out floor)) Debug.Log("Invalid condition - floor number is invalid");
                if (elements[3] == "not") not = true;
            }
            return !not ? memory.CurrentFloor == floor : memory.CurrentFloor != floor;
        }
        Debug.Log("Condition is not recognized");
        return false;
    }
}
