using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonDoor : MonoBehaviour
{

    public List<string> myKeys;
    public GameObject door;
    public GameObject Door { get; }

    public void DoorMet(GameObject door)
    {
        if (WantToOpenDoor(door))
        {
            this.door = door;
            TryToOpenDoor();
        }
    }

    public void TryToOpenDoor()
    {
        door.SendMessage("TryToOpenDoor", new object[] { myKeys.ToArray(), gameObject });
    }

    public void TryToOpenDoor(GameObject door)
    {
        door.SendMessage("TryToOpenDoor", new object[] { myKeys.ToArray(), gameObject });
    }

    public void TryToCloseDoor(GameObject door)
    {
        door.SendMessage("TryToCloseDoor", new object[] { myKeys.ToArray(), gameObject });
    }

    public void TryToLockDoor(GameObject door)
    {
        door.SendMessage("TryToLockDoor", new object[] { myKeys.ToArray(), gameObject });
    }

    public void AddKey(string key)
    {
        if (key == null)
        {
            Debug.Log(transform.name + " AddKey: key is null. => return null");
            return;
        }
        if (myKeys == null)
        {
            myKeys = new List<string>(new string[] { key });
        }
        else
        {
            myKeys.Add(key);
        }
    }

    public bool WantToOpenDoor(GameObject door)
    {
        Person person = GetComponent<Person>();
        Walking walking = person.walkingModule;

        if (walking.Path == null
            || walking.Path.Count == 0
            || walking.currentNodeIndex >= walking.Path.Count) return false;

        Node target = walking.Path[walking.currentNodeIndex];
        return IsTargetBehindDoor(target, door);

    }

    public bool IsTargetBehindDoor(Node target, GameObject door)
    {
        if (target.Name == door.name)
        {
            return false;
        }
        int layerMask = 1 << 10;
        RaycastHit hit;
        if (Physics.Linecast(transform.position, target.Position, out hit, layerMask))
        {
            return hit.transform.name == door.name;
        }
        else
        {
            return false;
        }
    }

    public void OpenDoor(GameObject door)
    {
        Task gotoDoor = new Task();
        gotoDoor.Command = Command.GO_TO_DOOR;
        gotoDoor.Limit = new Limit() { DoorToOpen = door };
        gotoDoor.Type = TaskType.MOVEMENT;
        gotoDoor.RequiredTasks = new List<Task>();
        GetComponent<Person>().waitingTasks.AddTask(gotoDoor);

        Task openDoor = new Task();
        openDoor.Command = Command.OPEN_DOOR;
        openDoor.Limit = new Limit() { DoorToOpen = door };
        openDoor.Type = TaskType.DOOR;
        openDoor.RequiredTasks = new List<Task>() { gotoDoor };
        GetComponent<Person>().waitingTasks.AddTask(openDoor);
    }

    public void CloseDoor(GameObject door)
    {
        Task closeDoor = new Task();
        closeDoor.Command = Command.CLOSE_DOOR;
        closeDoor.Limit = new Limit() { DoorToOpen = door };
        closeDoor.Type = TaskType.DOOR;
        closeDoor.RequiredTasks = new List<Task>();
        GetComponent<Person>().waitingTasks.AddTask(closeDoor);
    }

    public void YouEnterRoom(GameObject door)
    {
        GameObject room = door.GetComponent<DoorController>().MyRoom;
        if (room != null)
        {
            GetComponent<Person>().PersonMemory.SaveCurrentRoom(room);
        }
    }

    public void YouExitRoom(GameObject door)
    {
        GameObject room = door.GetComponent<DoorController>().MyRoom;
        if (room != null)
        {
            GetComponent<Person>().PersonMemory.ClearCurrentRoom();
        }
    }
}
