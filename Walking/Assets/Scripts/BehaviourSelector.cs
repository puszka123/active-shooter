﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourSelector
{
    public List<Behaviour> AvailableBehaviours;

    public BehaviourSelector(List<Behaviour> behaviours)
    {
        AvailableBehaviours = behaviours;
    }

    public Behaviour SelectBehaviour(Person person)
    {
        List<float> behavioursProbabilities = new List<float>();
        float probabilitiesSum = 0f;
        AvailableBehaviours.ForEach(b =>
        {
            probabilitiesSum += b.BehaviourHappenProbability(person);
            behavioursProbabilities.Add(b.BehaviourHappenProbability(person));
        });
        float luckyNumber = Random.Range(0f, probabilitiesSum);
        float start = 0f;
        float end = 0f;
        for (int i = 0; i < behavioursProbabilities.Count; i++)
        {
            if (behavioursProbabilities[i] == 0) continue;

            start = end;
            end += behavioursProbabilities[i];
            if (start <= luckyNumber && luckyNumber < end)
            {
                return AvailableBehaviours[i];
            }
        }

        return null;
    }
}
