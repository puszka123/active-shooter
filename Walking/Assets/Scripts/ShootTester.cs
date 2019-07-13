using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTester : MonoBehaviour {
    public float Speed;
    Rigidbody m_Rigidbody;

    private void Start()
    {
        Speed = Resources.Sprint;
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        m_Rigidbody.MovePosition(transform.position + transform.forward * Speed * Time.deltaTime);
    }
}
