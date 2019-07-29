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
                StaircaseManager.Join(person);
                Shooter.SendMessage("StaircaseEnter", person);
            }
            else
            {
                StaircaseManager.Leave(person);
            }
        }

        if (person.CompareTag("ActiveShooter"))
        {
            if (person.PersonMemory.IsAtStaircase)
            {
                person.GetComponent<ShooterStaircase>().OnStaircaseEnter();
            }
            else
            {
                person.GetComponent<ShooterStaircase>().OnStaircaseLeave();
            }
        }
    }
}
