using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EvacuationActions  {

    public class RunToExit : Action
    {
        public RunToExit()
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

            Type = ActionType.EVACUATE;
        }

        public override void TasksCleaner(PersonMemory memory)
        {
        }

        public override void UpdateLimit(PersonMemory memory)
        {

        }
    }

    public class RunToMyRoom : Action
    {
        public RunToMyRoom(Room myRoom)
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

            Tasks = new List<Task>(new Task[] { goUp, goDown, goToRoom });

            Type = ActionType.EVACUATE;
        }

        public override void TasksCleaner(PersonMemory memory)
        {

        }

        public override void UpdateLimit(PersonMemory memory)
        {

        }
    }

    public class RunToAnyRoom : Action
    {
        public RunToAnyRoom()
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

            Tasks = new List<Task>(new Task[] { findRoom, goToRoom, knockDoor, enterRoom });

            Type = ActionType.EVACUATE;
        }

        public override void TasksCleaner(PersonMemory memory)
        {

        }

        public override void UpdateLimit(PersonMemory memory)
        {
            Utils.UpdateLimitForTask(memory, Command.GO_TO_ROOM, Tasks);
            Utils.UpdateLimitForTask(memory, Command.KNOCK, Tasks);
            Utils.UpdateLimitForTask(memory, Command.ENTER_ROOM, Tasks);
        }
    }

    public class RunAway : Action
    {
        public RunAway()
        {
            Task runAway = new Task();
            runAway.Command = Command.RUN_AWAY;
            runAway.Limit = new Limit();
            runAway.Type = TaskType.MOVEMENT;
            runAway.RequiredTasks = new List<Task>();

            Tasks = new List<Task> { runAway };

            Type = ActionType.EVACUATE;
        }

        public override void TasksCleaner(PersonMemory memory)
        {

        }

        public override void UpdateLimit(PersonMemory memory)
        {
            
        }
    }
}
