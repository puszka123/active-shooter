using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FightActions {

    public class Fight : Action
    {
        public Fight()
        {
            Task getToEnemy = new Task();
            getToEnemy.Command = Command.RUN_TO_ENEMY;
            getToEnemy.Limit = new Limit();
            getToEnemy.Type = TaskType.MOVEMENT;
            getToEnemy.RequiredTasks = new List<Task>();

            Task fight = new Task();
            fight.Command = Command.FIGHT;
            fight.Limit = new Limit();
            fight.Type = TaskType.FIGHTER;
            fight.RequiredTasks = new List<Task>();

            Tasks = new List<Task> { getToEnemy, fight };
        }

        public override void TasksCleaner(PersonMemory memory)
        {
            
        }

        public override void UpdateLimit(PersonMemory memory)
        {
            
        }
    }
}
