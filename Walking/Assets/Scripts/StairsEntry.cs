using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsEntry : MonoBehaviour
{
    public StaircaseManager StaircaseManager;
    public GameObject Shooter;
    // Use this for initialization
    void Start()
    {
        StaircaseManager = GameObject.FindGameObjectWithTag("StaircaseManager").GetComponent<StaircaseManager>();
        Shooter = GameObject.FindGameObjectWithTag("ActiveShooter");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        Person person = other.GetComponent<Person>();
        if (person != null && person.CompareTag("Employee"))
        {
            person.PersonMemory.ToggleIsAtStaircase();
            if (person.PersonMemory.IsAtStaircase)
            {
                StaircaseManager.Join(new object[] { person, name });
                if (Shooter.GetComponent<ShooterStaircase>().staircase == name)
                {
                    Shooter.SendMessage("StaircaseEnter", person);
                }
            }
            else
            {
                StaircaseManager.Leave(new object[] { person, name });
                if (Shooter.GetComponent<ShooterStaircase>().staircase == name)
                {
                    Shooter.SendMessage("StaircaseLeave", person);
                }
            }
        }

        if (person.CompareTag("ActiveShooter"))
        {
            if (person.PersonMemory.IsAtStaircase)
            {
                person.GetComponent<ShooterStaircase>().OnStaircaseEnter(name);
            }
            else
            {
                person.GetComponent<ShooterStaircase>().OnStaircaseLeave();
            }
        }
    }
}
