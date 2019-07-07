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

    private float closeTime = 1f;

    public bool IsOpen { get { return isOpen; } }
    public bool IsLocked { get; set; }

    public List<GameObject> peopleToInform;

    BoxCollider myCollider;

    Renderer m_renderer;

    // Use this for initialization
    void Start()
    {
        peopleToInform = new List<GameObject>();
        IsLocked = true; //TEST
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
        GameObject gObject = (GameObject)args[1];
        if (!IsLocked)
        {
            OpenDoor();
        }
        string[] keys = (string[])args[0];

        if((keys != null && keys.Contains(doorKey)) || doorKey == null)
        {
            OpenDoor();
        }
        else if(gObject != null)
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
                    person.PersonMemory.AddBlockedNode(item);
                }
            }
            if (update)
            {
                person.walkingModule.UpdatePathAfterBlockedNode(gObject.transform, person.PersonMemory);
            }
        }
    }

    public void TryToCloseDoor(object[] args)
    {
        GameObject gObject = (GameObject)args[1];
        string[] keys = (string[])args[0];

        if ((keys != null && keys.Contains(doorKey)) || doorKey == null)
        {
            CloseDoor();
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

}
