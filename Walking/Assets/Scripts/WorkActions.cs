using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorkActions {

    public class GoToWork : Action
    {
        public GoToWork(Room myRoom)
        {
            Task goUp = new Task();
            goUp.Command = Command.GO_UP;
            goUp.Limit = new Limit() { FoundRoom = myRoom };
            goUp.Type = TaskType.MOVEMENT;
            goUp.RequiredTasks = new List<Task>();

            Task goDown = new Task();
            goDown.Command = Command.GO_DOWN;
            goDown.Limit = new Limit() { FoundRoom = myRoom };
            goDown.Type = TaskType.MOVEMENT;
            goDown.RequiredTasks = new List<Task>();

            Task goToRoom = new Task();
            goToRoom.Command = Command.GO_TO_ROOM;
            goToRoom.Limit = new Limit() { FoundRoom = myRoom };
            goToRoom.Type = TaskType.MOVEMENT;
            goToRoom.RequiredTasks = new List<Task>();

            Task work = new Task();
            work.Command = Command.STAY;
            work.Type = TaskType.MOVEMENT;
            work.RequiredTasks = new List<Task>();

            Tasks = new List<Task>(new Task[] { goDown, goUp, goToRoom, work });

            Type = ActionType.WORK;
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

    public class LeaveWork : Action
    {
        public LeaveWork()
        {
            Task goDown = new Task();
            goDown.Command = Command.GO_DOWN;
            goDown.Type = TaskType.MOVEMENT;
            goDown.RequiredTasks = new List<Task>();

            Task exitBuilding = new Task();
            exitBuilding.Command = Command.EXIT_BUILDING;
            exitBuilding.Type = TaskType.MOVEMENT;
            exitBuilding.RequiredTasks = new List<Task>();

            Tasks = new List<Task>(new Task[] { goDown, exitBuilding });

            Type = ActionType.WORK;
        }

        public override float ActionHappenProbability(Person person)
        {
            return 0f;
        }

        public override void TasksCleaner(PersonMemory memory)
        {
        }

        public override void UpdateLimit(PersonMemory memory)
        {

        }
    }
}
