using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour {
    int respawnNameGenerator = 1;
    int roomLocationGenerator = 1;
    int doorNameGenerator = 1;
    int pathLocationNameGenerator = 1;

    bool shootersInitied = false;
    float time = 0f;

	// Use this for initialization
	void Start () {
        foreach (var item in GameObject.FindGameObjectsWithTag("RoomLocation"))
        {
            item.name = "RoomLocation " + roomLocationGenerator++;
        }

        foreach (var item in GameObject.FindGameObjectsWithTag("PathLocation"))
        {
            item.name = "CheckPoint " + pathLocationNameGenerator++;
            item.GetComponent<PathLocation>().InitFloor();
            item.GetComponent<PathLocation>().FindRoomObstacles();
            if(item.GetComponent<PathLocation>().IsRoom)
            {
                item.AddComponent<RoomManager>();
                item.GetComponent<RoomManager>().Init();
            }
        }
        foreach (var item in GameObject.FindGameObjectsWithTag("Door"))
        {
            item.name = "Door " + doorNameGenerator++;
            item.GetComponent<DoorController>().SetDoorKey(item.name);
            if (item.GetComponent<RoomLocation>() != null)
            {
                item.GetComponent<RoomLocation>().UpdateNode(item.name);
            }
        }
        //foreach (var item in GameObject.FindGameObjectsWithTag("Respawn"))
        //{
        //    item.name = "Respawn " + respawnNameGenerator++;
        //    item.GetComponent<Respawn>().enabled = true;
        //}
        foreach (var item in GameObject.FindGameObjectsWithTag("RoomLocation"))
        {
            if (item.GetComponent<RoomLocation>().Workplace)
            {
                //item.name = "RoomLocationWorkplace " + respawnNameGenerator++;
                item.AddComponent<Respawn>();
            }
        }
    }

    // Update is called once per frame
    void Update () {
        if (!shootersInitied)
        {
            time += Time.deltaTime;
        }
        if (!shootersInitied && time >= 3f)
        {
            shootersInitied = true;
            GameObject[] activeShooters = GameObject.FindGameObjectsWithTag("ActiveShooter");

            foreach (var shooter in activeShooters)
            {
                shooter.GetComponent<Person>().Init(3, ""); //test floor
                shooter.GetComponent<Shooting>().enabled = true; 
                shooter.GetComponent<ShooterAwarness>().enabled = true; 
                shooter.GetComponent<FollowVictim>().enabled = true; 
            }

            GameObject[] fighters = GameObject.FindGameObjectsWithTag("Employee");

            foreach (var fighter in fighters)
            {
                if (fighter.name.StartsWith("Fighter"))
                {
                    fighter.GetComponent<Person>().Init(3, ""); //test floor
                }
            }
        }
	}
}
