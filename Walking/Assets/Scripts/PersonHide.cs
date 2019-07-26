using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonHide : MonoBehaviour
{
    Vector3 StartPosition;
    bool isHiding;
    // Use this for initialization
    void Start()
    {
        StartPosition = transform.position;
        isHiding = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Hide()
    {
        if (!isHiding)
        {
            transform.localScale = new Vector3(transform.localScale.x, 0.05f, transform.localScale.z);
            transform.position = transform.GetChild(0).position;
            isHiding = true;
            GetComponent<Person>().MyState.IsHiding = true;
        }

    }

    public void GetUp()
    {
        if (isHiding)
        {
            transform.localScale = new Vector3(transform.localScale.x, 0.1f, transform.localScale.z);
            transform.position = new Vector3(transform.position.x, StartPosition.y, transform.position.z);
            isHiding = false;
            GetComponent<Person>().MyState.IsHiding = false;
        }
    }
}
