using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HideActions {

    public class HideInCurrentRoom : Action
    {
        public HideInCurrentRoom()
        {
            Task hide = new Task();
            hide.Command = Command.HIDE_IN_CURRENT_ROOM;
            hide.Limit = new Limit();
            hide.Type = TaskType.MOVEMENT;
            hide.RequiredTasks = new List<Task>();

            Tasks = new List<Task>(new Task[] { hide });

            Type = ActionType.HIDE;
        }

        public override float ActionHappenProbability(Person person)
        {
            PersonStats stats = person.GetComponent<PersonStats>();
            float chances = stats.HideChance;
            if(person.MyState.IsHiding)
            {
                chances = 0f;
                return chances;
            }
            else if(!person.PersonMemory.CurrentRoom.Door.GetComponent<DoorController>().IsLocked)
            {
                chances = 1f - stats.LockDoorChance;
            }
            return chances;
        }

        public override void TasksCleaner(PersonMemory memory)
        {
        }

        public override void UpdateLimit(PersonMemory memory)
        {
            Utils.UpdateLimitForTask(memory, Command.HIDE_IN_CURRENT_ROOM, Tasks);
        }    
    }

    public class LockCurrentRoom: Action
    {
        public LockCurrentRoom()
        {
            Task goToDoor = new Task();
            goToDoor.Command = Command.GO_TO_DOOR;
            goToDoor.Limit = new Limit();
            goToDoor.Type = TaskType.MOVEMENT;
            goToDoor.RequiredTasks = new List<Task>();

            Task lockCurrent = new Task();
            lockCurrent.Command = Command.LOCK_DOOR;
            lockCurrent.Limit = new Limit();
            lockCurrent.Type = TaskType.DOOR;
            lockCurrent.RequiredTasks = new List<Task>();

            Tasks = new List<Task>(new Task[] { goToDoor, lockCurrent });

            Type = ActionType.HIDE;
        }

        public override float ActionHappenProbability(Person person)
        {
            PersonStats stats = person.GetComponent<PersonStats>();
            float chances = stats.LockDoorChance;
            if (person.PersonMemory.CurrentRoom.Door.GetComponent<DoorController>().IsLocked)
            {
                chances = 0f;
            }
            return chances;
        }

        public override void TasksCleaner(PersonMemory memory)
        {
        }

        public override void UpdateLimit(PersonMemory memory)
        {
            Utils.UpdateLimitForTask(memory, Command.GO_TO_DOOR, Tasks);
            Utils.UpdateLimitForTask(memory, Command.LOCK_DOOR, Tasks);
        }
    }

    public class BarricadeDoor : Action
    {
        public BarricadeDoor()
        {
            Task goToDoor = new Task();
            goToDoor.Command = Command.GO_TO_DOOR;
            goToDoor.Limit = new Limit();
            goToDoor.Type = TaskType.MOVEMENT;
            goToDoor.RequiredTasks = new List<Task>();

            Task barricade = new Task();
            barricade.Command = Command.BARRICADE_DOOR;
            barricade.Limit = new Limit();
            barricade.Type = TaskType.DOOR;
            barricade.RequiredTasks = new List<Task>();

            Tasks = new List<Task> { goToDoor, barricade };
        }

        public override float ActionHappenProbability(Person person)
        {
            return person.GetComponent<PersonStats>().BarricadeDoorChance;
        }

        public override void TasksCleaner(PersonMemory memory)
        {
            
        }

        public override void UpdateLimit(PersonMemory memory)
        {
            Utils.UpdateLimitForTask(memory, Command.GO_TO_DOOR, Tasks);
            Utils.UpdateLimitForTask(memory, Command.BARRICADE_DOOR, Tasks);
        }
    }
}
