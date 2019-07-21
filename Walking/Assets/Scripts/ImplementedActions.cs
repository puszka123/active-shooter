using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class ImplementedActions
{
    public class InformRoom : Action
    {
        public InformRoom()
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

            Task enterRoom = new Task();
            enterRoom.Command = Command.ENTER_ROOM;
            enterRoom.Limit = new Limit();
            enterRoom.Type = TaskType.MOVEMENT;
            enterRoom.RequiredTasks = new List<Task>() { knockDoor };

            Task askCloseDoor = new Task();
            askCloseDoor.Command = Command.ASK_CLOSE_DOOR;
            askCloseDoor.Limit = new Limit();
            askCloseDoor.Type = TaskType.DOOR;
            askCloseDoor.RequiredTasks = new List<Task>() { enterRoom };

            Task tellAboutShooter = new Task();
            tellAboutShooter.Command = Command.TELL_ABOUT_SHOOTER;
            tellAboutShooter.Limit = new Limit();
            tellAboutShooter.Type = TaskType.TALK;
            tellAboutShooter.RequiredTasks = new List<Task>() { askCloseDoor };


            Tasks = new List<Task>(new Task[] { findRoom, goToRoom, knockDoor, enterRoom, askCloseDoor, tellAboutShooter });
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
            Utils.UpdateLimitForTask(memory, Command.GO_TO_ROOM, Tasks);
            Utils.UpdateLimitForTask(memory, Command.KNOCK, Tasks);
            Utils.UpdateLimitForTask(memory, Command.ENTER_ROOM, Tasks);
            Utils.UpdateLimitForTask(memory, Command.ASK_CLOSE_DOOR, Tasks);
            Utils.UpdateLimitForTask(memory, Command.TELL_ABOUT_SHOOTER, Tasks);
        }
    }
}
