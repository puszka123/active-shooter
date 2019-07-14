using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShooterActions  {

    public class SearchForVictim : Action
    {
        public SearchForVictim()
        {
            Tasks = new List<Task>();
        }

        public override void TasksCleaner(PersonMemory memory)
        {
            
        }

        public override void UpdateLimit(PersonMemory memory)
        {
            
        }
    }
}
