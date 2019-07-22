using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EvacuationActions
{

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

        public override float ActionHappenProbability(Person person)
        {
            if (person.MyState.SeeShooter)
            {
                return 0f;
            }
            int shooterFloor = person.PersonMemory.ShooterInfo.Floor;
            int myFloor = person.PersonMemory.CurrentFloor;
            bool isBelowMe = shooterFloor < myFloor;
            bool isOnMyFloor = shooterFloor == myFloor;
            bool isAboveMe = shooterFloor > myFloor;

            float chances = 0.3f;
            if (myFloor == 0)
            {
                chances += 0.8f;
                return chances;
            }

            float floorWeight = 0.5f;

            if (isAboveMe || isOnMyFloor)
            {
                chances += floorWeight;
            }

            return chances;
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

            Task enterRoom = new Task();
            enterRoom.Command = Command.ENTER_ROOM;
            enterRoom.Limit = new Limit() { FoundRoom = myRoom };
            enterRoom.Type = TaskType.MOVEMENT;
            enterRoom.RequiredTasks = new List<Task> { goToRoom };

            Tasks = new List<Task>(new Task[] { goUp, goDown, goToRoom, enterRoom });

            Type = ActionType.EVACUATE;
        }

        public override float ActionHappenProbability(Person person)
        {
            if (person.MyState.SeeShooter)
            {
                return 0f;
            }
            if (!person.MyState.CanRunToMyRoom)
            {
                return 0f;
            }
            Room currentRoom = person.PersonMemory.CurrentRoom;
            if (currentRoom != null && currentRoom.Id == person.PersonMemory.MyRoom.Id)
            {
                return 0f;
            }
            int shooterFloor = person.PersonMemory.ShooterInfo.Floor;
            int myFloor = person.PersonMemory.CurrentFloor;
            bool isBelowMe = shooterFloor < myFloor;
            bool isOnMyFloor = shooterFloor == myFloor;
            bool isAboveMe = shooterFloor > myFloor;

            float chances = 0.3f;
            if (myFloor == person.PersonMemory.GetRoomFloor(person.PersonMemory.MyRoom.Id))
            {
                chances += 0.8f;
                return chances;
            }

            float floorWeight = 0.5f;

            if (isBelowMe || isOnMyFloor)
            {
                chances += floorWeight;
            }

            return chances;
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

        public override float ActionHappenProbability(Person person)
        {
            return 0f; //test
            if (person.MyState.SeeShooter)
            {
                return 0f;
            }
            int shooterFloor = person.PersonMemory.ShooterInfo.Floor;
            int myFloor = person.PersonMemory.CurrentFloor;
            bool isBelowMe = shooterFloor < myFloor;
            bool isOnMyFloor = shooterFloor == myFloor;
            bool isAboveMe = shooterFloor > myFloor;

            float chances = 0.0f;

            float floorWeight = 0.5f;

            if (isBelowMe || isOnMyFloor)
            {
                chances += floorWeight;
            }

            return chances;
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

        public override float ActionHappenProbability(Person person)
        {
            if (person.MyState.SeeShooter)
            {
                return 1f;
            }
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
