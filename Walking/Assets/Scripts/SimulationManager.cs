using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour {
    int respawnNameGenerator = 1;
    int roomLocationGenerator = 1;
    int doorNameGenerator = 1;
    int pathLocationNameGenerator = 1;

    public List<GameObject> workplaces;

    public Dictionary<int, Graph> graphs;

    bool shootersInitied = false;
    float time = 0f;

    public bool StartShooterInitiation = false;

	// Use this for initialization
	void Start () {
        GameObject[] roomLocations = GameObject.FindGameObjectsWithTag("RoomLocation");
        foreach (var item in roomLocations)
        {
            item.name = "RoomLocation " + roomLocationGenerator++;
            item.GetComponent<Renderer>().enabled = false;
        }

        foreach (var item in GameObject.FindGameObjectsWithTag("PathLocation"))
        {
            item.name = "CheckPoint " + pathLocationNameGenerator++;
            PathLocation itemPathlocation = item.GetComponent<PathLocation>();
            itemPathlocation.InitFloor();
            itemPathlocation.FindRoomObstacles();
            if(itemPathlocation.IsRoom)
            {
                item.AddComponent<RoomManager>();
                item.GetComponent<RoomManager>().Init(itemPathlocation.Floor);
            }
            item.GetComponent<Renderer>().enabled = false;
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
        workplaces = new List<GameObject>();
        foreach (var item in roomLocations)
        {
            if (item.GetComponent<RoomLocation>().Workplace)
            {
                //item.name = "RoomLocationWorkplace " + respawnNameGenerator++;
                workplaces.Add(item);
                item.AddComponent<Respawn>();
            }
        }
        GetComponent<Menu>().InitWorkplaces();
        graphs = new Dictionary<int, Graph>();
        foreach (var item in GameObject.FindGameObjectsWithTag("Floor"))
        {
            int f = int.Parse(item.name.Split(' ')[1]);
            graphs.Add(f, new Graph(f));
        }
    }

    // Update is called once per frame
    void Update () {
        if(Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            Time.timeScale += 1f;
        }
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            Time.timeScale -= 1f;
        }

        if (!shootersInitied && StartShooterInitiation)
        {
            time += Time.deltaTime;
        }
        if (!shootersInitied && time >= 3f && StartShooterInitiation)
        {
            GetComponent<FPSDisplayScript>().simulationTime = 0f;
            shootersInitied = true;
            GameObject[] activeShooters = GameObject.FindGameObjectsWithTag("ActiveShooter");

            foreach (var shooter in activeShooters)
            {
                shooter.GetComponent<Person>().Init(3, ""); //test floor
                shooter.GetComponent<Shooting>().enabled = true; 
                shooter.GetComponent<ShooterAwarness>().enabled = true; 
                shooter.GetComponent<FollowVictim>().enabled = true; 
            }
        }
	}

    public void InitActiveShooter()
    {
        StartShooterInitiation = true;
    }
}
