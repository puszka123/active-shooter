using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DoorController : MonoBehaviour
{

    private bool isOpen = false;
    public bool IsLockable;
    public bool IsToilet = false;
    private string doorKey;
    public GameObject MyRoom;
    public int Obstacles;
    public GameObject[] VisibleObstacles;
    public float Resistance;

    private float closeTime = 2f;

    public bool IsOpen { get { return isOpen; } }
    public bool IsLocked;

    public bool testLock = false;

    public List<GameObject> peopleToInform;

    BoxCollider myCollider;

    Renderer m_renderer;

    // Use this for initialization
    void Start()
    {
        Resistance = 100f;
        peopleToInform = new List<GameObject>();
        IsLocked = false; //TEST
        //SetLockColor();
        BoxCollider[] res = GetComponents<BoxCollider>();
        m_renderer = GetComponent<Renderer>();
        foreach (var item in res)
        {
            if (!item.isTrigger)
            {
                myCollider = item;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen && !Destroyed())
        {

            if (closeTime <= 0f)
            {
                isOpen = false;
                myCollider.enabled = true;
                m_renderer.enabled = true;
                SetCloseColor();
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
        GameObject gObject = (GameObject)args[1];
        if (Destroyed())
        {
            OpenDoor();
            return;
        }

        if (IsBarricaded())
        {
            if (gObject != null)
            {
                UpdateBlockedNodes(gObject);
            }
            return;
        }

        if (!IsLocked)
        {
            OpenDoor();
            return;
        }
        string[] keys = (string[])args[0];

        if ((keys != null && keys.Contains(doorKey)) || doorKey == null)
        {
            //Debug.Log(doorKey + " " + keys.Contains(doorKey));
            OpenDoor();
        }
        else if (gObject != null)
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
        closeTime = 2f;
        isOpen = true;
        m_renderer.enabled = false;
        IsLocked = false;
        InformRoomEmployees();
    }

    public void CloseDoor()
    {
        isOpen = false;
        myCollider.enabled = true;
        m_renderer.enabled = true;
        SetCloseColor();
    }

    //if opened then close and lock
    public void LockDoor()
    {
        if (!IsOpen)
        {
            IsLocked = true;
        }
        else
        {
            CloseDoor();
            IsLocked = true;
        }
        SetLockColor();
        InformRoomEmployees();
    }

    public void SetRoom(GameObject room)
    {
        MyRoom = room;
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            DoorTrigger doorTrigger = child.GetComponent<DoorTrigger>();
            if (doorTrigger != null)
            {
                doorTrigger.enabled = true;
            }
        }
    }

    public void AddObstacle(GameObject obstacle)
    {
        if (!obstacle.GetComponent<Obstacle>().InBlock)
        {
            if (VisibleObstacles.Length > Obstacles)
            {
                VisibleObstacles[Obstacles].GetComponent<Renderer>().enabled = true;
                VisibleObstacles[Obstacles].GetComponent<BoxCollider>().enabled = true;
                obstacle.GetComponent<Obstacle>().InBlock = true;
                ++Obstacles;
            }
        }
    }

    public void AddFirstObstacle()
    {
        VisibleObstacles[0].GetComponent<Renderer>().enabled = true;
        //VisibleObstacles[0].GetComponent<BoxCollider>().enabled = true;
        Obstacles = 1;
    }

    public void AddSecondObstacle()
    {
        VisibleObstacles[1].GetComponent<Renderer>().enabled = true;
        //VisibleObstacles[1].GetComponent<BoxCollider>().enabled = true;
        Obstacles = 2;
        InformRoomEmployees();
    }


    public void YouAreHit(object[] args)
    {
        RaycastHit hit = (RaycastHit)args[0];
        Transform activeShooter = (Transform)args[1];
        Resistance -= activeShooter.GetComponent<Shooting>().ShootStrength;
        if (Resistance <= 0f)
        {
            activeShooter.SendMessage("DoorDestroyed", gameObject);
            myCollider.enabled = false;
            m_renderer.enabled = false;
        }
    }

    public bool Destroyed()
    {
        return Resistance <= 0f;
    }

    public void InformRoomEmployees()
    {
        if (MyRoom != null)
        {
            foreach (var employee in Utils.GetEmployeesInRoom(MyRoom))
            {
                employee.SendMessage("SelectAction");
            }
        }
    }

    public void SetLockColor()
    {
        GetComponent<Renderer>().material.color = Color.red;
    }

    public void SetCloseColor()
    {
        GetComponent<Renderer>().material.color = Color.gray;
    }

    public bool IsBarricaded()
    {
        return Obstacles > 0;
    }
}
