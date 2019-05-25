using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testSpeed : MonoBehaviour
{
    public float Speed;
    private Rigidbody m_Rigidbody;
    private float timer = 0.0f;

    // Use this for initialization
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_Rigidbody.MovePosition(transform.position + transform.forward * Speed * Time.deltaTime);
        timer += Time.deltaTime;
    }
}
