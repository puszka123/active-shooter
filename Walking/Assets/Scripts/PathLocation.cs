﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathLocation : MonoBehaviour {
    public List<GameObject> NearestNeighbours;

    public void FindMyNeighbours()
    {
        NearestNeighbours = new List<GameObject>();
        GameObject[] pathLocations = GameObject.FindGameObjectsWithTag("PathLocation");
        foreach (GameObject item in pathLocations)
        {
            int layerMask = 1 << 9;
            if(!Physics.Linecast(transform.position, item.transform.position, layerMask))
            {
                NearestNeighbours.Add(item);
            }

        }
    }
}