using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public string Id;
    public GameObject Door;
    public GameObject[] Employees;
    public List<GameObject> Obstacles
    {
        get
        {
            return Reference.GetComponent<PathLocation>().Obstacles;
        }
    }
    public GameObject Reference;
}
