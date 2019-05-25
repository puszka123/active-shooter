using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DoorController : MonoBehaviour
{

    private bool isOpen = false;
    public bool IsLockable = true;
    private string doorKey;

    private float closeTime = 0.25f;

    public bool IsOpen { get { return isOpen; } }

    BoxCollider myCollider;

    Renderer m_renderer;

    // Use this for initialization
    void Start()
    {
        doorKey = IsLockable ? transform.name : null;
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
        string[] keys = (string[])args[0];
        GameObject person = (GameObject)args[1];

        if((keys != null && keys.Contains(doorKey)) || doorKey == null)
        {
            closeTime = 0.25f;
            isOpen = true;
            m_renderer.enabled = false;
        }
        else if(person != null)
        {
            Walking walking = person.GetComponent<Walking>();
            bool update = false;
            foreach (var item in walking.Path)
            {
                LayerMask layerMask = LayerMask.GetMask("Door");
                RaycastHit hit;
                bool blocked = Physics.Linecast(person.transform.position, item.Position, out hit, layerMask);
                LayerMask layerMask1 = LayerMask.GetMask("Wall");
                bool wall = Physics.Linecast(transform.position, item.Position, layerMask1);
                if (blocked && !wall && hit.collider.gameObject.name == gameObject.name)
                {
                    update = true;
                    walking.PersonMemory.AddBlockedNode(item);
                }
            }
            if (update)
            {
                walking.UpdatePathAfterBlockedNode();
            }
        }
    }
}
