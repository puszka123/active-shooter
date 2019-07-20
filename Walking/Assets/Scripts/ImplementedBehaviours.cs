using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ImplementedBehaviours
{

    public class Evacuate : Behaviour
    {
        public Evacuate(Room myRoom)
        {
            Actions = new List<Action>();
            Actions.Add(new EvacuationActions.RunToAnyRoom());
            Actions.Add(new EvacuationActions.RunToExit());
            Actions.Add(new EvacuationActions.RunToMyRoom(myRoom));
            Actions.Add(new EvacuationActions.RunAway());
        }

        public override float BehaviourHappenProbability(Person person)
        {
            if (person.PersonMemory.ShooterInfo == null) return 0f;
            bool seeShooter = Utils.CanSee(person.gameObject, GameObject.FindGameObjectWithTag("ActiveShooter"));
            bool isInRoom = Utils.IsInAnyRoom(person.PersonMemory);
            bool aboveMe = person.PersonMemory.ShooterInfo.Floor > person.PersonMemory.CurrentFloor;

            float aboveMeValue = aboveMe ? 1f : 0f;
            float isInRoomValue = isInRoom ? 1f : 0f;
            float altruism = person.PersonalAttributes.Altruism;

            float altruismWeight = 1f;
            float aboveMeWeight = 0.25f;
            float isInRoomWeight = 0.25f;
            float chances;
            if (!seeShooter)
            {
                chances = aboveMeValue * aboveMeWeight + isInRoomValue * isInRoomWeight;
            }
            else
            {
                chances = (1 - altruism) * altruismWeight;
            }

            return chances;
        }
    }

    public class Hide : Behaviour
    {
        public Hide()
        {
            Actions = new List<Action>();
            Actions.Add(new HideActions.HideInCurrentRoom());
            Actions.Add(new HideActions.LockCurrentRoom());
            Actions.Add(new HideActions.BarricadeDoor());
        }

        public override float BehaviourHappenProbability(Person person)
        {
            if (person.PersonMemory.ShooterInfo == null) return 0f;
            bool seeShooter = Utils.CanSee(person.gameObject, GameObject.FindGameObjectWithTag("ActiveShooter"));
            bool isInRoom = Utils.IsInAnyRoom(person.PersonMemory);
            bool aboveMe = person.PersonMemory.ShooterInfo.Floor > person.PersonMemory.CurrentFloor;

            float notAboveMeValue = !aboveMe ? 1f : 0f;
            float isInRoomValue = isInRoom ? 1f : 0f;

            float aboveMeWeight = 0.25f;
            float isInRoomWeight = 0.25f;
            float chances;
            if (seeShooter)
            {
                chances = 0f;
            }
            else
            {
                chances = notAboveMeValue * aboveMeWeight + isInRoomValue * isInRoomWeight;
            }

            return chances;
        }
    }

    public class Fight : Behaviour
    {
        public Fight()
        {
            Actions = new List<Action>();
            Actions.Add(new FightActions.Fight());
        }

        public override float BehaviourHappenProbability(Person person)
        {
            if (person.PersonMemory.ShooterInfo == null) return 0f;
            bool seeShooter = Utils.CanSee(person.gameObject, GameObject.FindGameObjectWithTag("ActiveShooter"));

            float altruism = person.PersonalAttributes.Altruism;


            float altruismWeight = 1f;

            float chances;
            if (seeShooter)
            {
                chances = altruism * altruismWeight;
            }
            else
            {
                chances = 0f;
            }
            return chances;
        }
    }

    public class Work : Behaviour
    {
        public Work(Room myRoom)
        {
            Actions = new List<Action>();
            Actions.Add(new WorkActions.GoToWork(myRoom));
            Actions.Add(new WorkActions.LeaveWork());
        }

        public override float BehaviourHappenProbability(Person person)
        {
            if (person.PersonMemory.ShooterInfo == null) return 1f;
            else return 0f;
        }
    }

    public class FindAndKill : Behaviour
    {
        public FindAndKill()
        {
            Actions = new List<Action>();
            Actions.Add(new ShooterActions.GoDown());
            Actions.Add(new ShooterActions.GoUp());
            Actions.Add(new ShooterActions.GoToAnyRoom());
        }

        public override float BehaviourHappenProbability(Person person)
        {
            return 1f;
        }
    }

    public class Inform : Behaviour
    {
        public Inform()
        {
            Actions = new List<Action>();
        }

        public override float BehaviourHappenProbability(Person person)
        {
            return 0f;
        }
    }
}
