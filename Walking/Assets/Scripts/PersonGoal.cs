using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonGoal {
    public Goal MyCurrentGoal;

    public void SetCurrentGoal(Goal newGoal)
    {
        MyCurrentGoal = newGoal;
    }
}
