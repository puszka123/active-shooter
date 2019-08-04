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
            findRoom.Command = Command.FIND_NOT_CHECKED_ROOM;
            findRoom.Limit = new Limit();
            findRoom.Type = TaskType.FINDER;
            findRoom.RequiredTasks = new List<Task>();

            Task goToRoom = new Task();
            goToRoom.Command = Command.GO_TO_ROOM;
            goToRoom.Limit = new Limit();
            goToRoom.Type = TaskType.MOVEMENT;
            goToRoom.RequiredTasks = new List<Task>() {  };

            Task knockDoor = new Task();
            knockDoor.Command = Command.KNOCK;
            knockDoor.Limit = new Limit();
            knockDoor.Type = TaskType.DOOR;
            knockDoor.RequiredTasks = new List<Task>() {  };

            Task destroyDoor = new Task();
            destroyDoor.Command = Command.DESTROY_DOOR;
            destroyDoor.Limit = new Limit();
            destroyDoor.Type = TaskType.DESTROYER;
            destroyDoor.RequiredTasks = new List<Task>() {  };

            Task enterRoom = new Task();
            enterRoom.Command = Command.ENTER_ROOM;
            enterRoom.Limit = new Limit();
            enterRoom.Type = TaskType.MOVEMENT;
            enterRoom.RequiredTasks = new List<Task>() {  };

            Task checkRoom = new Task();
            checkRoom.Command = Command.CHECK_ROOM;
            checkRoom.Limit = new Limit();
            checkRoom.Type = TaskType.MOVEMENT;
            checkRoom.RequiredTasks = new List<Task>() {  };

            Tasks = new List<Task>(new Task[] { findRoom, goToRoom, destroyDoor, enterRoom, checkRoom });

            Type = ActionType.FIND_AND_KILL;
        }

        public override float ActionHappenProbability(Person person)
        {
            PersonStats stats = person.GetComponent<PersonStats>();
            ShooterStaircase shooterStaircase = person.GetComponent<ShooterStaircase>();
            int myFloor = person.PersonMemory.CurrentFloor;
            bool allChecked = Utils.AllChecked(person, myFloor);
            int floorCheckedRooms = Utils.CheckedRooms(person, myFloor);

            if (allChecked) return 0f;

            float victimStairs = shooterStaircase.PotentialVictim != null ? 1f : 0f;
            float chances = stats.shooterSearchFloorChance;

            chances -= floorCheckedRooms * stats.shooterCheckRoomWeight;


            chances -= victimStairs * stats.victimStairsWeight;
            Debug.Log(chances + " " + floorCheckedRooms + " " + stats.shooterCheckRoomWeight);
            return chances;
        }

        public override void TasksCleaner(PersonMemory memory)
        {

        }

        public override void UpdateLimit(PersonMemory memory)
        {
            Utils.UpdateLimitForTask(memory, Command.GO_TO_ROOM, Tasks);
            //Utils.UpdateLimitForTask(memory, Command.KNOCK, Tasks);
            Utils.UpdateLimitForTask(memory, Command.DESTROY_DOOR, Tasks);
            Utils.UpdateLimitForTask(memory, Command.ENTER_ROOM, Tasks);
            Utils.UpdateLimitForTask(memory, Command.CHECK_ROOM, Tasks);
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
            PersonStats stats = person.GetComponent<PersonStats>();
            ShooterStaircase shooterStaircase = person.GetComponent<ShooterStaircase>();
            float myFloor = person.PersonMemory.CurrentFloor;
            if (myFloor == Resources.MAX_FLOOR) return 0f;

            float victimStairs = shooterStaircase.PotentialVictim != null ? 1f : 0f;
            float chances = stats.shooterGoUpChance;


            if (victimStairs == 1f)
            {
                int victimFloor = shooterStaircase.PotentialVictim.PersonMemory.CurrentFloor;
                if (victimFloor > myFloor)
                {
                    chances += victimStairs * stats.victimStairsWeight;
                }
                else
                {
                    chances = 0f;
                }
            }
            //Debug.Log(string.Format("{0} {1}", person.simulationTime, chances));
            return chances;
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
            PersonStats stats = person.GetComponent<PersonStats>();
            ShooterStaircase shooterStaircase = person.GetComponent<ShooterStaircase>();
            float myFloor = person.PersonMemory.CurrentFloor;
            if (myFloor == 0)
            {
                return 0f;
            }

            float victimStairs = shooterStaircase.PotentialVictim != null ? 1f : 0f;
            float chances = stats.shooterGoDownChance;

            if (victimStairs == 1f)
            {
                int victimFloor = shooterStaircase.PotentialVictim.PersonMemory.CurrentFloor;
                if (victimFloor < myFloor)
                {
                    chances += victimStairs * stats.victimStairsWeight;
                }
                else
                {
                    chances = 0f;
                }
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
}
