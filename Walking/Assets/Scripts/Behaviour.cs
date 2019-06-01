using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Behaviour {
    public List<Action> Actions;

    public void RunToExitBehaviour()
    {
        Action action1 = new Action();
        action1.Command = Command.GO_DOWN;
        action1.Condition = new Condition("if floor is not 0");

        Action action2 = new Action();
        action2.Command = Command.EXIT_BUILDING;
        action2.Condition = new Condition("if floor is 0");

        Actions = new List<Action>(new Action[] { action1, action2 });
    }
}
