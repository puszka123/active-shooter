using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testDistance : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        LayerMask layerMask = LayerMask.GetMask("Wall");

        //front----------------------------
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Debug.Log(hit.distance);
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.blue);
        }
    }
}
