using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSelector
{
    public List<Action> AvailableActions;

    public ActionSelector(Behaviour behaviour)
    {
        AvailableActions = behaviour.Actions;
    }

    public Action SelectAction(Person person)
    {
        List<float> actionsProbabilities = new List<float>();
        float probabilitiesSum = 0f;
        AvailableActions.ForEach(a =>
        {
            actionsProbabilities.Add(a.ActionHappenProbability(person));
            probabilitiesSum += a.ActionHappenProbability(person);
        });
        float luckyNumber = Random.Range(0f, probabilitiesSum);
        float start = 0f;
        float end = 0f;

        //if(person.CompareTag("ActiveShooter"))
        //for (int i = 0; i < actionsProbabilities.Count; i++)
        //{
        //    Debug.Log(actionsProbabilities[i] + " " + AvailableActions[i]);
        //}

        for (int i = 0; i < actionsProbabilities.Count; i++)
        {
            if (actionsProbabilities[i] == 0) continue;

            start = end;
            end += actionsProbabilities[i];
            if (start <= luckyNumber && luckyNumber < end)
            {
                return AvailableActions[i];
            }
        }

        return null;
    }
}
