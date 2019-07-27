using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InformActions
{
    public class TellAboutShooter : Action
    {
        public TellAboutShooter()
        {
            Task tellAboutShooter = new Task();
            tellAboutShooter.Command = Command.SAY_ACTIVE_SHOOTER;
            tellAboutShooter.Type = TaskType.TALK;
            tellAboutShooter.RequiredTasks = new List<Task>();

            Tasks = new List<Task> { tellAboutShooter };
        }

        public override float ActionHappenProbability(Person person)
        {
            return 1f;
        }

        public override void TasksCleaner(PersonMemory memory)
        {
        }

        public override void UpdateLimit(PersonMemory memory)
        {
        }
    }
}
