using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Keys
{
    LEVEL, ROOM
}

public class Limit
{
    public string LocationId;
    public Room FoundRoom;
    public GameObject DoorToOpen;
    public ChatRoom chatRoom;
    public GameObject[] Employees;
    public GameObject Obstacle;
}