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
            PersonStats stats = person.GetComponent<PersonStats>();
            GameObject shooter = GameObject.FindGameObjectWithTag("ActiveShooter");
            if (person.PersonMemory.ShooterInfo == null) return 0f;
            bool seeShooter = Utils.CanSee(person.gameObject, shooter);
            bool isInRoom = Utils.IsInAnyRoom(person.PersonMemory);
            bool aboveMe = person.PersonMemory.ShooterInfo.Floor > person.PersonMemory.CurrentFloor;

            float aboveMeValue = aboveMe ? 1f : 0f;
            float isInRoomValue = isInRoom ? 1f : 0f;
            float distanceToShooter = Utils.Distance(person.gameObject, shooter);
            float shooterFar = Resources.Far[0] < distanceToShooter ? 1f : 0f;

            float chances = stats.basicEvacuationChance;
            float lastResort = stats.basicFightChance;

            if (seeShooter && isInRoom)
            {
                chances = 1 - (stats.basicFightChance + lastResort);
                if (chances < 0) chances = 0;
            }
            else if (!seeShooter && isInRoom)
            {
                chances += aboveMeValue * stats.aboveMeWeight;
            }
            else if (seeShooter && !isInRoom)
            {
                chances = (1 - stats.basicFightChance) + shooterFar * stats.distanceWeight;
            }
            else
            {
                chances = 1f;
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
            PersonStats stats = person.GetComponent<PersonStats>();

            if (person.PersonMemory.ShooterInfo == null) return 0f;
            bool seeShooter = Utils.CanSee(person.gameObject, GameObject.FindGameObjectWithTag("ActiveShooter"));
            bool isInRoom = Utils.IsInAnyRoom(person.PersonMemory);
            bool aboveMe = person.PersonMemory.ShooterInfo.Floor > person.PersonMemory.CurrentFloor;

            float notAboveMeValue = !aboveMe ? 1f : 0f;
            float isInRoomValue = isInRoom ? 1f : 0f;


            float chances = stats.basicHideChance;
            if (seeShooter || !isInRoom)
            {
                chances = 0f;
            }
            else if (isInRoom)
            {
                chances += notAboveMeValue * stats.notAboveMeWeight;
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
            PersonStats stats = person.GetComponent<PersonStats>();

            if (person.PersonMemory.ShooterInfo == null) return 0f;

            GameObject shooter = GameObject.FindGameObjectWithTag("ActiveShooter");
            bool seeShooter = Utils.CanSee(person.gameObject, shooter);
            bool isInRoom = Utils.IsInAnyRoom(person.PersonMemory);

            float distanceToShooter = Utils.Distance(person.gameObject, shooter);
            float shooterNear = Resources.Near[1] >= distanceToShooter ? 1f : 0f;

            float chances;
            if (seeShooter && !isInRoom)
            {
                chances = stats.basicFightChance + shooterNear * stats.distanceWeight;
            }
            else if (seeShooter && isInRoom)
            {
                chances = stats.basicFightChance;
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
            if (person.CompareTag("ActiveShooter"))
            {
                return 1f;
            }
            else
            {
                return 0f;
            }
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
