using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ImplementedBehaviours {
    
    public class Evacuate : Behaviour {
        public Evacuate(Room myRoom)
        {
            Actions = new List<Action>();
            Actions.Add(new EvacuationActions.RunToAnyRoom());
            Actions.Add(new EvacuationActions.RunToExit());
            Actions.Add(new EvacuationActions.RunToMyRoom(myRoom));
        }
    }

    public class Hide : Behaviour
    {
        public Hide()
        {
            Actions = new List<Action>();
            Actions.Add(new HideActions.HideInCurrentRoom());
            Actions.Add(new HideActions.LockCurrentRoom());
        }
    }

    public class Fight : Behaviour
    {
        public Fight()
        {
            //add all fight actions here
        }
    }

    public class Work : Behaviour
    {
        public Work(Room myRoom)
        {
            Actions = new List<Action>();
            Actions.Add(new WorkActions.GoToWork(myRoom));
            Actions.Add(new WorkActions.LeaveWork());
        }
    }

    public class FindAndKill: Behaviour
    {
        public FindAndKill()
        {
            Actions = new List<Action>();
        }
    }
}
