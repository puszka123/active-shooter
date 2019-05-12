using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonDoor : MonoBehaviour
{

    public List<string> myKeys;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DoorMet(GameObject door)
    {
        door.SendMessage("TryToOpenDoor", new object[] { myKeys.ToArray(), gameObject });
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
}
