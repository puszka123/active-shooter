using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DoorController : MonoBehaviour
{

    private bool isOpen = false;
    public bool IsLockable = true;
    public bool IsToilet = false;
    private string doorKey;
    public GameObject MyRoom;
    public int Obstacles;

    private float closeTime = 1f;

    public bool IsOpen { get { return isOpen; } }
    public bool IsLocked;

    public bool testLock = false;

    public List<GameObject> peopleToInform;

    BoxCollider myCollider;

    Renderer m_renderer;

    // Use this for initialization
    void Start()
    {
        peopleToInform = new List<GameObject>();
        IsLocked = false; //TEST
        BoxCollider[] res = GetComponents<BoxCollider>();
        m_renderer = GetComponent<Renderer>();
        foreach (var item in res)
        {
            if(!item.isTrigger)
            {
                myCollider = item;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isOpen)
        {

            if (closeTime <= 0f)
            {
                isOpen = false;
                myCollider.enabled = true;
                m_renderer.enabled = true;
            }
            else
            {
                closeTime -= Time.deltaTime;
                myCollider.enabled = false;
            }
        }
    }

    public void TryToOpenDoor(object[] args)
    {
        if(Obstacles > 0)
        {
            return;
        }
        GameObject gObject = (GameObject)args[1];
        if (!IsLocked)
        {
            OpenDoor();
            return;
        }
        string[] keys = (string[])args[0];

        if((keys != null && keys.Contains(doorKey)) || doorKey == null)
        {
            OpenDoor();
        }
        else if(gObject != null)
        {
            UpdateBlockedNodes(gObject);
        }
    }

    private void UpdateBlockedNodes(GameObject gObject)
    {
        Person person = gObject.GetComponent<Person>();
        bool update = false;
        foreach (var item in person.walkingModule.Path)
        {
            LayerMask layerMask = LayerMask.GetMask("Door");
            RaycastHit hit;
            bool blocked = Physics.Linecast(person.transform.position, item.Position, out hit, layerMask);
            LayerMask layerMask1 = LayerMask.GetMask("Wall");
            bool wall = Physics.Linecast(transform.position, item.Position, layerMask1);
            if (blocked && !wall && hit.collider.gameObject.name == gameObject.name)
            {
                update = true;
                person.PersonMemory.AddBlockedNode(gameObject, item);
            }
        }
        if (update)
        {
            person.walkingModule.UpdatePathAfterBlockedNode(gObject.transform, person.PersonMemory);
        }
    }

    public void TryToCloseDoor(object[] args)
    {
        //GameObject gObject = (GameObject)args[1];
        //string[] keys = (string[])args[0];
        CloseDoor();
        
    }

    public void TryToLockDoor(object[] args)
    {
        //GameObject gObject = (GameObject)args[1];
        string[] keys = (string[])args[0];

        if ((keys != null && keys.Contains(doorKey)) || doorKey == null)
        {
            LockDoor();
        }
    }

    public void SetDoorKey(string key)
    {
        doorKey = IsLockable ? key : null;
    }

    public void OpenDoor()
    {
        closeTime = 1f;
        isOpen = true;
        m_renderer.enabled = false;
        IsLocked = false;
    }

    public void CloseDoor()
    {
        closeTime = 0f;
    }

    //if opened then close and lock
    public void LockDoor()
    {
        if(!IsOpen)
        {
            IsLocked = true;
        }
        else
        {
            CloseDoor();
            IsLocked = true;
        }
    }

    public void SetRoom(GameObject room)
    {
        MyRoom = room;
    }

    public void AddObstacle(GameObject obstacle)
    {
        if (!obstacle.GetComponent<Obstacle>().InBlock)
        {
            obstacle.GetComponent<Obstacle>().InBlock = true;
            ++Obstacles;
        }
    }

}
