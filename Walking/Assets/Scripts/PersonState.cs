﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonState {
    bool isHiding;
    bool seeShooter;
    bool canRunToMyRoom;
    bool canGoDown;
    //bool canRunToAnyRoom;

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

    public bool SeeShooter
    {
        get
        {
            return seeShooter;
        }
        set
        {
            seeShooter = value;
        }
    }

    public bool CanRunToMyRoom
    {
        get
        {
            return canRunToMyRoom;
        }
        set
        {
            canRunToMyRoom = value;
        }
    }

    public bool CanGoDown
    {
        get
        {
            return canGoDown;
        }
        set
        {
            canGoDown = value;
        }
    }
}
