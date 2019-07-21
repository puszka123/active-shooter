using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ShooterActions
{

    public class GoToAnyRoom : Action
    {
        public GoToAnyRoom()
        {
            Task findRoom = new Task();
            findRoom.Command = Command.FIND_ROOM;
            findRoom.Limit = new Limit();
            findRoom.Type = TaskType.FINDER;
            findRoom.RequiredTasks = new List<Task>();

            Task goToRoom = new Task();
            goToRoom.Command = Command.GO_TO_ROOM;
            goToRoom.Limit = new Limit();
            goToRoom.Type = TaskType.MOVEMENT;
            goToRoom.RequiredTasks = new List<Task>() { findRoom };

            Task knockDoor = new Task();
            knockDoor.Command = Command.KNOCK;
            knockDoor.Limit = new Limit();
            knockDoor.Type = TaskType.DOOR;
            knockDoor.RequiredTasks = new List<Task>() { goToRoom };

            Task destroyDoor = new Task();
            destroyDoor.Command = Command.DESTROY_DOOR;
            destroyDoor.Limit = new Limit();
            destroyDoor.Type = TaskType.DESTROYER;
            destroyDoor.RequiredTasks = new List<Task>() { knockDoor };

            Task enterRoom = new Task();
            enterRoom.Command = Command.ENTER_ROOM;
            enterRoom.Limit = new Limit();
            enterRoom.Type = TaskType.MOVEMENT;
            enterRoom.RequiredTasks = new List<Task>() { destroyDoor };

            Tasks = new List<Task>(new Task[] { findRoom, goToRoom, knockDoor, destroyDoor, enterRoom });

            Type = ActionType.FIND_AND_KILL;
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
            Utils.UpdateLimitForTask(memory, Command.GO_TO_ROOM, Tasks);
            Utils.UpdateLimitForTask(memory, Command.KNOCK, Tasks);
            Utils.UpdateLimitForTask(memory, Command.DESTROY_DOOR, Tasks);
            Utils.UpdateLimitForTask(memory, Command.ENTER_ROOM, Tasks);
        }
    }

    public class GoUp : Action
    {
        public GoUp()
        {
            Task goUp = new Task();
            goUp.Command = Command.GO_UP;
            goUp.Limit = new Limit();
            goUp.Type = TaskType.MOVEMENT;
            goUp.RequiredTasks = new List<Task>();


            Tasks = new List<Task>(new Task[] {  goUp, });

            Type = ActionType.FIND_AND_KILL;
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

    public class GoDown : Action
    {
        public GoDown()
        {
            Task goDown = new Task();
            goDown.Command = Command.GO_DOWN;
            goDown.Limit = new Limit();
            goDown.Type = TaskType.MOVEMENT;
            goDown.RequiredTasks = new List<Task>();


            Tasks = new List<Task>(new Task[] { goDown, });

            Type = ActionType.FIND_AND_KILL;
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
