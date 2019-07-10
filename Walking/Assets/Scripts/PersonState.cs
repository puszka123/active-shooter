using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonState {
    bool isHiding;

    public PersonState()
    {
        IsHiding = false;
    }

    public bool IsHiding {
        get
        {
            return isHiding;
        }
        set
        {
            isHiding = value;
        }
    }
}
