using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class Node : FastPriorityQueueNode {
    private static int IdGenerator = 0;

    public string Name;
    public Vector3 Position;
}
