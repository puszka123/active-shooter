using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class ImplementedBehaviours
{
    public class RunToExit : Behaviour
    {
        public RunToExit()
        {
            Action action1 = new Action();
            action1.Command = Command.GO_DOWN;
            action1.Type = ActionType.MOVEMENT;
            action1.RequiredActions = new List<Action>();

            Action action2 = new Action();
            action2.Command = Command.EXIT_BUILDING;
            action2.Type = ActionType.MOVEMENT;
            action2.RequiredActions = new List<Action> { action1 };

            Actions = new List<Action>(new Action[] { action1, action2 });
        }

        public override void ActionsCleaner(PersonMemory memory)
        {
            List<Action> actionsToDelete = Actions.
                FindAll(action => memory.IsOnTheFloor(0) ?
                action.Command == Command.GO_DOWN : false).ToList();
            if (actionsToDelete.Count == 0) return;
            Command commandToDelete = actionsToDelete[0].Command;
            Action goToExit = Actions.Where(action => action.Command == Command.EXIT_BUILDING).ToList()[0];
            goToExit.RequiredActions = goToExit.RequiredActions.Where(action => action.Command != commandToDelete).ToList();
            Actions = Actions.Where(action => action.Command != commandToDelete).ToList();
        }

        public override void UpdateActionsLimits(PersonMemory memory)
        {
            
        }
    }

    public class Work : Behaviour
    {
        public Work(string myRoomId)
        {
            Action action1 = new Action();
            action1.Command = Command.STAY;
            action1.Type = ActionType.MOVEMENT;

            Action action2 = new Action();
            action2.Command = Command.GO_TO_ROOM;
            action2.Limits = new List<Limit>() { new Limit() { LocationId = myRoomId } };
            action2.Type = ActionType.MOVEMENT;
            action2.RequiredActions = new List<Action>();
            action1.RequiredActions = new List<Action> { action2 };

            Actions = new List<Action>(new Action[] { action2, action1 });
        }

        public override void ActionsCleaner(PersonMemory memory)
        {
            List<Action> actionsToDelete = Actions.
                FindAll(action => memory.IsInMyRoom() ?
                action.Command == Command.GO_TO_ROOM : false).ToList();
            if (actionsToDelete.Count == 0) return;
            Command commandToDelete = actionsToDelete[0].Command;
            Action stayInRoom = Actions.Where(action => action.Command == Command.STAY).ToList()[0];
            stayInRoom.RequiredActions = stayInRoom.RequiredActions.Where(action => action.Command != commandToDelete).ToList();
            Actions = Actions.Where(action => action.Command != commandToDelete).ToList();
        }

        public override void UpdateActionsLimits(PersonMemory memory)
        {

        }
    }

    public class HideInMyRoom : Behaviour
    {
        public HideInMyRoom(string myRoomId)
        {
            Action goUp = new Action();
            goUp.Command = Command.GO_UP;
            goUp.Type = ActionType.MOVEMENT;
            goUp.RequiredActions = new List<Action>();

            Action goDown = new Action();
            goDown.Command = Command.GO_DOWN;
            goDown.Type = ActionType.MOVEMENT;
            goDown.RequiredActions = new List<Action>();

            Action goToRoom = new Action();
            goToRoom.Command = Command.GO_TO_ROOM;
            goToRoom.Limits = new List<Limit>() { new Limit() { LocationId = myRoomId } };
            goToRoom.Type = ActionType.MOVEMENT;
            goToRoom.RequiredActions = new List<Action>() { goUp, goDown };

            Actions = new List<Action>(new Action[] { goUp, goDown, goToRoom });
        }

        public override void ActionsCleaner(PersonMemory memory)
        {
            List<Action> actionsToDelete = Actions.
                FindAll(action => memory.MyRoomIsAboveMe() ? 
                action.Command == Command.GO_DOWN 
                : action.Command == Command.GO_UP).ToList();
            if (actionsToDelete.Count == 0) return;
            Command commandToDelete = actionsToDelete[0].Command;
            Action goToRoom = Actions.Where(action => action.Command == Command.GO_TO_ROOM).ToList()[0];
            goToRoom.RequiredActions = goToRoom.RequiredActions.Where(action => action.Command != commandToDelete).ToList();
            Actions = Actions.Where(action => action.Command != commandToDelete).ToList();
        }

        public override void UpdateActionsLimits(PersonMemory memory)
        {

        }
    }

    public class InformRoom : Behaviour
    {
        public InformRoom()
        {
            Action findRoom = new Action();
            findRoom.Command = Command.FIND_ROOM;
            findRoom.Limits = new List<Limit>();
            findRoom.Type = ActionType.FINDER;
            findRoom.RequiredActions = new List<Action>();

            Action goToRoom = new Action();
            goToRoom.Command = Command.GO_TO_ROOM;
            goToRoom.Limits = new List<Limit>();
            goToRoom.Type = ActionType.MOVEMENT;
            goToRoom.RequiredActions = new List<Action>() { findRoom };

            Action knockDoor = new Action();
            knockDoor.Command = Command.KNOCK;
            knockDoor.Limits = new List<Limit>();
            knockDoor.Type = ActionType.DOOR;
            knockDoor.RequiredActions = new List<Action>() { goToRoom };

            Actions = new List<Action>(new Action[] { findRoom, goToRoom, knockDoor });
        }

        public override void ActionsCleaner(PersonMemory memory)
        {
            
        }

        public override void UpdateActionsLimits(PersonMemory memory)
        {
            if (memory.FoundRoom != null)
            {
                Actions.
                    Where(action => action.Command == Command.GO_TO_ROOM).
                    ToArray()[0].
                    Limits.
                    Add(new Limit { LocationId = memory.FoundRoom.Id });               

                //Actions.
                //    Where(action => action.Command == Command.KNOCK).
                //    ToArray()[0].
                //    Limits.
                //    Add(new Limit { FoundRoom = memory.FoundRoom });
            }
        }
    }
}
