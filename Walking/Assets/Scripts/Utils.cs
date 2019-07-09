using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class Utils
{
    public static Room GetRoom(Task task)
    {
        Room[] rooms = task.Limits.
                    Select(limit => limit.FoundRoom).
                    Where(r => r != null).ToArray();
        return rooms.Length > 0 ? rooms[0] : null;
    }

    public static GameObject GetDoor(Task task)
    {
        GameObject[] doors = task.Limits.
                    Select(limit => limit.DoorToOpen).
                    Where(d => d != null).ToArray();
        return doors.Length > 0 ? doors[0] : null;
    }

    public static GameObject[] GetEmployees(Task task)
    {
        var employees = task.Limits.
                    Select(limit => limit.Employees).
                    Where(e => e != null).ToArray();
        return employees.Length > 0 ? employees[0] : null;
    }

    public static string NearestStairs(string stairsType, Transform transform, PersonMemory memory)
    {
        GameObject location = GameObject.Find("Checkpoints " + memory.CurrentFloor);
        List<GameObject> stairs = new List<GameObject>();
        foreach (Transform item in location.transform)
        {
            if (item.GetComponent<Stairs>() != null && item.GetComponent<PathLocation>().StairsDirection == stairsType)
            {
                stairs.Add(item.gameObject);
            }
            if (stairs.Count >= 2) break;
        }
        GameObject nearestStairs = stairs[0];
        foreach (var item in stairs)
        {
            if (Vector3.Distance(transform.position, nearestStairs.transform.position)
                >
                Vector3.Distance(transform.position, item.transform.position))
            {
                nearestStairs = item;
            }
        }
        return nearestStairs.name;
    }

    public static string NearestExit(Transform transform, PersonMemory memory)
    {
        GameObject location = GameObject.Find("Checkpoints " + memory.CurrentFloor);
        List<GameObject> exits = new List<GameObject>();
        foreach (Transform item in location.transform)
        {
            if (item.GetComponent<PathLocation>().IsExit)
            {
                exits.Add(item.gameObject);
            }
            if (exits.Count >= 2) break;
        }
        if (exits.Count == 0) return null;
        GameObject nearestExits = exits[0];
        foreach (var item in exits)
        {
            if (Vector3.Distance(transform.position, nearestExits.transform.position)
                >
                Vector3.Distance(transform.position, item.transform.position))
            {
                nearestExits = item;
            }
        }
        return nearestExits.name;
    }

    public static bool ToFar(GameObject a, GameObject b, float threshold = 2)
    {
        return Vector3.Distance(a.transform.position, b.transform.position) > threshold;
    }

    public static bool DoorIsOpened(GameObject door)
    {
        return door.GetComponent<DoorController>().IsOpen;
    }

    public static bool DoorIsLocked(GameObject door)
    {
        return door.GetComponent<DoorController>().IsLocked;
    }
}
