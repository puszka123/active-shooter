using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class ImplementedActions
{
    public class RunToExit : Action
    {
        public RunToExit()
        {
            Task task1 = new Task();
            task1.Command = Command.GO_DOWN;
            task1.Type = TaskType.MOVEMENT;
            task1.RequiredTasks = new List<Task>();

            Task task2 = new Task();
            task2.Command = Command.EXIT_BUILDING;
            task2.Type = TaskType.MOVEMENT;
            task2.RequiredTasks = new List<Task> { task1 };

            Tasks = new List<Task>(new Task[] { task1, task2 });
        }

        public override void TasksCleaner(PersonMemory memory)
        {
            List<Task> tasksToDelete = Tasks.
                FindAll(task => memory.IsOnTheFloor(0) ?
                task.Command == Command.GO_DOWN : false).ToList();
            if (tasksToDelete.Count == 0) return;
            Command commandToDelete = tasksToDelete[0].Command;
            Task goToExit = Tasks.Where(task => task.Command == Command.EXIT_BUILDING).ToList()[0];
            goToExit.RequiredTasks = goToExit.RequiredTasks.Where(task => task.Command != commandToDelete).ToList();
            Tasks = Tasks.Where(task => task.Command != commandToDelete).ToList();
        }

        public override void UpdateLimit(PersonMemory memory)
        {

        }
    }

    public class Work : Action
    {
        public Work(string myRoomId)
        {
            Task task1 = new Task();
            task1.Command = Command.STAY;
            task1.Type = TaskType.MOVEMENT;

            Task task2 = new Task();
            task2.Command = Command.GO_TO_ROOM;
            task2.Limit = new Limit() { LocationId = myRoomId };
            task2.Type = TaskType.MOVEMENT;
            task2.RequiredTasks = new List<Task>();
            task1.RequiredTasks = new List<Task> { task2 };

            Tasks = new List<Task>(new Task[] { task2, task1 });
        }

        public override void TasksCleaner(PersonMemory memory)
        {
            List<Task> tasksToDelete = Tasks.
                FindAll(task => memory.IsInMyRoom() ?
                task.Command == Command.GO_TO_ROOM : false).ToList();
            if (tasksToDelete.Count == 0) return;
            Command commandToDelete = tasksToDelete[0].Command;
            Task stayInRoom = Tasks.Where(task => task.Command == Command.STAY).ToList()[0];
            stayInRoom.RequiredTasks = stayInRoom.RequiredTasks.Where(task => task.Command != commandToDelete).ToList();
            Tasks = Tasks.Where(task => task.Command != commandToDelete).ToList();
        }

        public override void UpdateLimit(PersonMemory memory)
        {

        }
    }

    public class HideInMyRoom : Action
    {
        public HideInMyRoom(string myRoomId)
        {
            Task goUp = new Task();
            goUp.Command = Command.GO_UP;
            goUp.Type = TaskType.MOVEMENT;
            goUp.RequiredTasks = new List<Task>();

            Task goDown = new Task();
            goDown.Command = Command.GO_DOWN;
            goDown.Type = TaskType.MOVEMENT;
            goDown.RequiredTasks = new List<Task>();

            Task goToRoom = new Task();
            goToRoom.Command = Command.GO_TO_ROOM;
            goToRoom.Limit = new Limit() { LocationId = myRoomId };
            goToRoom.Type = TaskType.MOVEMENT;
            goToRoom.RequiredTasks = new List<Task>() { goUp, goDown };

            Tasks = new List<Task>(new Task[] { goUp, goDown, goToRoom });
        }

        public override void TasksCleaner(PersonMemory memory)
        {
            List<Task> tasksToDelete = Tasks.
                FindAll(task => memory.MyRoomIsAboveMe() ?
                task.Command == Command.GO_DOWN
                : task.Command == Command.GO_UP).ToList();
            if (tasksToDelete.Count == 0) return;
            Command commandToDelete = tasksToDelete[0].Command;
            Task goToRoom = Tasks.Where(task => task.Command == Command.GO_TO_ROOM).ToList()[0];
            goToRoom.RequiredTasks = goToRoom.RequiredTasks.Where(task => task.Command != commandToDelete).ToList();
            Tasks = Tasks.Where(task => task.Command != commandToDelete).ToList();
        }

        public override void UpdateLimit(PersonMemory memory)
        {

        }
    }

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

    public class InformRoomAndHide : Action
    {
        public InformRoomAndHide()
        {
            Task findRoom = new Task();
            findRoom.Command = Command.FIND_ANY_ROOM;
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

            Task hideInRoom = new Task();
            hideInRoom.Command = Command.HIDE_IN_CURRENT_ROOM;
            hideInRoom.Limit = new Limit();
            hideInRoom.Type = TaskType.MOVEMENT;
            hideInRoom.RequiredTasks = new List<Task>() { tellAboutShooter };


            Tasks = new List<Task>(new Task[] { findRoom, goToRoom, knockDoor, enterRoom, askCloseDoor, tellAboutShooter, hideInRoom });
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
            Utils.UpdateLimitForTask(memory, Command.HIDE_IN_CURRENT_ROOM, Tasks);
        }
    }
}
