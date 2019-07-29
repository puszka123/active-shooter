using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShooterStaircase : MonoBehaviour
{
    public Person PotentialVictim;
    public Person Shooter;
    public StaircaseManager StaircaseManager;

    private void Start()
    {
        Shooter = GetComponent<Person>();
        StaircaseManager = GetComponent<StaircaseManager>();
    }

    public void StaircaseEnter(Person person)
    {
        if (PotentialVictim == null && Shooter.PersonMemory.IsAtStaircase)
        {
            PotentialVictim = person;
            transform.SendMessage("SelectAction");
        }
    }

    public void StaircaseLeave(Person person)
    {
        PotentialVictim = null;
        if (PotentialVictim != null && PotentialVictim.name == person.name && Shooter.PersonMemory.IsAtStaircase)
        {
            transform.SendMessage("SelectAction");
        }
    }

    public void OnStaircaseEnter()
    {
        Person victim = GetFirstPotentialVictim();
        if(victim != null)
        {
            PotentialVictim = victim;
            transform.SendMessage("SelectAction");
        }

    }

    public Person GetFirstPotentialVictim()
    {
        Person victim = StaircaseManager.People.FirstOrDefault();
        return victim;
    }

    public void OnStaircaseLeave()
    {
        PotentialVictim = null;
    }
}
